using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace SolarSystem.Backend.Classes
{
    public class Station
    {
        public event Action<OrderBox> OnOrderBoxFinished;
        public event Action OnOrderBoxReceivedAtStationEvent;
        public event Action<Station, List<Article>> OnShelfBoxNeededRequest;
        
        public string Name { get; }
        
        private readonly List<ShelfBox> _shelfBoxes;
        public IEnumerable<ShelfBox> ShelfBoxes => _shelfBoxes.AsReadOnly();
        
        private readonly List<OrderBox> _orderBoxes;
        
        public int MaxShelfBoxes { get; }
        public int MaxOrderBoxes { get; }

        private int RemainingPickingTime = 0;

        private OrderBoxPickingContainer _orderBoxBeingPacked = null;

        private readonly Storage _storage;

        private int _shelfBoxWaitCount;
        
        


        public Station(string name, int maxShelfBoxes,int maxOrderBoxes)
        {
            
            _shelfBoxes = new List<ShelfBox>(maxShelfBoxes);
            _orderBoxes = new List<OrderBox>(maxOrderBoxes);
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MaxShelfBoxes = maxShelfBoxes;
            MaxOrderBoxes = maxOrderBoxes;
            _shelfBoxWaitCount = 0;

  //          _storage = new Storage();

//            _storage.OnShelfBoxReadyFromStorage += ReceiveShelfBoxFromStorage;

            TimeKeeper.Tick += TickLoop;

        }
        
        /// <summary>
        /// Receives a ShelfBox or a OrderBox and tries to add it to appropriate list.
        /// </summary>
        /// <param name="box">ShelfBox or OrderBox</param>
        /// <returns>StationResult with description</returns>
        /// <exception cref="ArgumentNullException">If box is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the Box is not the right type.</exception>
        public StationResult ReceiveBox(Box box)
        {
            switch (box)
            {
                case ShelfBox shelfBox:
                    if (_shelfBoxes.Count >= MaxShelfBoxes) return StationResult.FullError;
                    _shelfBoxes.Add(shelfBox);
                    break;
                case OrderBox orderBox:
                    if (_orderBoxes.Count >= MaxOrderBoxes) return StationResult.FullError;
                    _orderBoxes.Add(orderBox);
                    MaybeRequestShelfBoxes();
                    OnOrderBoxReceivedAtStationEvent?.Invoke();
                    break;
                case null:
                    throw new ArgumentNullException(nameof(box));
                default:
                    throw new ArgumentOutOfRangeException(nameof(box));
            }
            return StationResult.Success;
        }

        private void TickLoop()
        {
            // If no orderBoxes -> skip
            if (!_orderBoxes.Any()) return;
            // Else call ChooseOrderBoxToPack
            if (_orderBoxBeingPacked == null)
            {
                ChooseOrderBoxToPack();
            }
            

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
            var spacesLeft = _shelfBoxes.Capacity - _shelfBoxes.Count;
            // Create neededArticlesList with size spacesLeft
            var neededArticlesList = new List<Article>(spacesLeft);
            
            // Loop through orderBoxes in FIFO.
            foreach (var orderBox in _orderBoxes)
            {
                // Loop through unpicked lines in orderbox
                foreach (var unpickedLine in orderBox.LinesNotPicked())
                {
                    // If neededArticlesList is full then return neededArticlesList
                    if (neededArticlesList.Count == neededArticlesList.Capacity)
                    {
                        return neededArticlesList;
                    }
                    
                    // Else if this line does not have a corresponding shelfbox then
                    if (!_shelfBoxes.Select(s => s.Line).Contains(unpickedLine))
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
            _orderBoxes.Remove(orderBox);
            // Invoke OnOrderBoxFinished with orderBox
            OnOrderBoxFinished?.Invoke(orderBox);
        }

        private void MaybeEvictShelfBoxes()
        {
            // Create evictionList
            var evictionList = new List<ShelfBox>();
            // Loop through all shelfBoxes
            foreach (var shelfBox in _shelfBoxes)
            {
                // Check if any unpacked lines of this type exist
                bool shelfBoxIsNeeded = _orderBoxes.SelectMany(o => o.LinesNotPicked()).Any(l => Equals(l, shelfBox.Line));
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
        }

        private void SendShelfBoxToStorage(ShelfBox shelfBox)
        {
            // Remove shelfBox from ShelfBoxes
            _shelfBoxes.Remove(shelfBox);
            // faked sending: beep, beep
        }

        private void OrderBoxLineFinishedPacking(OrderBox orderBox)
        {
            // Check if any shelfBoxes can be evicted
            MaybeEvictShelfBoxes();
            // If orderBox is fully packed then
            if (!orderBox.LinesNotPicked().Any())
            {
                // Evict orderBox
                EvictOrderBox(orderBox);
                
                // Set orderBoxLineBeingPicked to null
                _orderBoxBeingPacked = null;
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
            foreach (var orderBox in _orderBoxes)
            {
                // Loop through unpacked lines
                foreach (var line in orderBox.LinesNotPicked())
                {
                    // If line matches shelfbox then
                    if (_shelfBoxes.Any(s => Equals(s.Line.Article, line.Article)))
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
        }
        
        public void ReceiveShelfBoxFromStorage(ShelfBox shelfBox)
        {
            // Decrement StorageReceiveWaitingTime by one
            _shelfBoxWaitCount -= 1;
            // Add received shelfbox to shelfboxes
            _shelfBoxes.Add(shelfBox);
        }
       

    }
}