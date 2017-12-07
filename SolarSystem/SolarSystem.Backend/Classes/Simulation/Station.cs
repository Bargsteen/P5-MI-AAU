using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes
{
    public class Station
    {
        public event Action<OrderBox> OnOrderBoxFinishedAtStation;
        public event Action<OrderBox> OnOrderBoxReceivedAtStation;
        public event Action<Station, List<Article>> OnShelfBoxNeededRequest;
        
        public string Name { get; }
        
        public List<ShelfBox> ShelfBoxes;

        private readonly AreaCode _areaCode;
        
        public List<OrderBox> OrderBoxes;
        
        public int MaxShelfBoxes { get; }
        public int MaxOrderBoxes { get; }

        public bool StationIsFull => OrderBoxes.Capacity == OrderBoxes.Count;

        private OrderBoxPickingContainer _orderBoxBeingPacked = null;

        private readonly Storage _storage;

        private int _shelfBoxWaitCount;
        
        


        public Station(string name, int maxShelfBoxes,int maxOrderBoxes, AreaCode areaCode)
        {
            ShelfBoxes = new List<ShelfBox>(maxShelfBoxes);
            OrderBoxes = new List<OrderBox>(maxOrderBoxes);
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MaxShelfBoxes = maxShelfBoxes;
            MaxOrderBoxes = maxOrderBoxes;
            _areaCode = areaCode;
            _shelfBoxWaitCount = 0;

            TimeKeeper.Tick += TickLoop;

        }

        public void ReceiveOrderBox(OrderBox orderBox)
        {
            // If the station is full but still received an orderBox -> throw error
            if (OrderBoxes.Count >= MaxOrderBoxes)
            {
                throw new AccessViolationException($"Station {this} in should not have received an orderBox.");
            }
            
            // Add orderBox to orderBoxes
            OrderBoxes.Add(orderBox);
            
            // Maybe request more shelfboxes - depends on if there is more space left for shelfBoxes
            MaybeRequestShelfBoxes();    
            
            // Invoke event stating this happened
            OnOrderBoxReceivedAtStation?.Invoke(orderBox);
        }

        private void TickLoop()
        {
            // If no orderBoxes -> skip
            if (!OrderBoxes.Any()) return;
            
            // Else call ChooseOrderBoxToPack
            if (_orderBoxBeingPacked == null)
            {
                ChooseOrderBoxToPack();
            }
            
            MaybeEvictShelfBoxes();
            MaybeRequestShelfBoxes();
        }

        private void MaybeRequestShelfBoxes()
        {
            // If waiting for shelfboxes already then skip
            if (_shelfBoxWaitCount > 0) return;
            
            // Find needed articles
            var neededArticles = FindNeededArticlesInOrderBoxes();
            // If any articles are needed then 
            if (neededArticles.Any())
            {
                // Set StorageReceiveWatingTime to articlesNeeded.length
                _shelfBoxWaitCount = neededArticles.Count;
                // invoke NeedTheseArticles(articlesNeeded, Station)
                OnShelfBoxNeededRequest?.Invoke(this, neededArticles);
            }
        }

        private List<Article> FindNeededArticlesInOrderBoxes()
        {
            // Calculate spaces left for shelfBoxes, call it spacesLeft
            var spacesLeft = ShelfBoxes.Capacity - ShelfBoxes.Count;
            // Create neededArticlesList with size spacesLeft
            var neededArticlesList = new List<Article>(spacesLeft);
            
            // Loop through orderBoxes in FIFO.
            foreach (var orderBox in OrderBoxes)
            {
                // Loop through unpicked lines in orderbox
                foreach (var unpickedLine in orderBox.LinesNotPickedIn(_areaCode))
                {
                    // If neededArticlesList is full then return neededArticlesList
                    if (neededArticlesList.Count == neededArticlesList.Capacity)
                    {
                        return neededArticlesList;
                    }
                    
                    // Else if this line does not have a corresponding shelfbox then
                    if (!ShelfBoxes.Select(s => s.Line.Article).Contains(unpickedLine.Article))
                    {
                        // then add its article to neededArticlesList
                        neededArticlesList.Add(unpickedLine.Article);
                    }
                }
            }
            
            return neededArticlesList;
        }

        private void EvictOrderBox(OrderBox orderBox)
        {
            // Remove orderBox from orderBoxes
            OrderBoxes.Remove(orderBox);
            // Invoke OnOrderBoxFinishedAtStation with orderBox
            OnOrderBoxFinishedAtStation?.Invoke(orderBox);
        }

        public void MaybeEvictShelfBoxes()
        {
            // Create evictionList
            var evictionList = new List<ShelfBox>();
            // Loop through all shelfBoxes
            foreach (var shelfBox in ShelfBoxes)
            {
                // Check if any unpacked lines of this type exist
                bool shelfBoxIsNeeded = OrderBoxes.SelectMany(o => o.LinesNotPickedIn(_areaCode)).Any(l => Equals(l.Article.Id, shelfBox.Line.Article.Id));
                // if not, add to evictionList
                if ( ! shelfBoxIsNeeded)
                {
                    evictionList.Add(shelfBox);
                }
            }
                
            // Evict all shelfBoxes in evictionList by sending them back to Storage
            evictionList.ForEach(SendShelfBoxToStorage);
            
            // If any shelfBoxes were evicted, call MaybeRequestShelfBoxes
            if (evictionList.Any())
            {
                MaybeRequestShelfBoxes();
            }
            
            // If there is room for more shelfboxes, retrieve those.
            if(ShelfBoxes.Count < MaxShelfBoxes) MaybeRequestShelfBoxes();
        }

        private void SendShelfBoxToStorage(ShelfBox shelfBox)
        {
            // Remove shelfBox from ShelfBoxes
            ShelfBoxes.Remove(shelfBox);
            
            // faked sending: beep, beep
        }

        private void OrderBoxLineFinishedPacking(OrderBox orderBox)
        {
            
            // Check if any shelfBoxes can be evicted
            MaybeEvictShelfBoxes();
            
            // Set orderBoxLineBeingPicked to null
            _orderBoxBeingPacked = null;
            
            // If orderBox is fully packed then
            if (!orderBox.LinesNotPickedIn(_areaCode).Any())
            {
                // Evict orderBox
                EvictOrderBox(orderBox);
                string s = orderBox.Id + ";" + TimeKeeper.CurrentDateTime.Hour + ":" + TimeKeeper.CurrentDateTime.Minute + ":" + TimeKeeper.CurrentDateTime.Second;
                Outputter.WriteLineToFile(s);
            }
            
        }

        private void ChooseOrderBoxToPack()
        {
            // If orderBoxBeingPacked != null -> throw error
            if (_orderBoxBeingPacked != null)
            {
                throw new InvalidDataException($"{nameof(_orderBoxBeingPacked)} should be null when this is called.");
            }
            
            // Loop through orderBoxes
            foreach (var orderBox in OrderBoxes)
            {
                // Loop through unpacked lines
                foreach (var line in orderBox.LinesNotPickedIn(_areaCode))
                {
                    // If line matches shelfbox then
                    if (ShelfBoxes.Any(s => Equals(s.Line.Article, line.Article)))
                    {
                        // Create new OrderBoxLineBeingPicked with orderBox and line to be picked
                        _orderBoxBeingPacked = new OrderBoxPickingContainer(orderBox, line);
                        // Listen to orderBoxLineBeingPicked's finishedEvent
                        _orderBoxBeingPacked.OnLinePickedForOrderBox += OrderBoxLineFinishedPacking;

                        // Return, since we have found a pickable line
                        return;
                    }
                }
            }
            
            MaybeEvictShelfBoxes();
        }
        
        public void ReceiveShelfBoxFromStorage(ShelfBox shelfBox)
        {
            // Decrement StorageReceiveWaitingTime by one
            _shelfBoxWaitCount -= 1;
            // Add received shelfbox to shelfboxes
            ShelfBoxes.Add(shelfBox);
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}