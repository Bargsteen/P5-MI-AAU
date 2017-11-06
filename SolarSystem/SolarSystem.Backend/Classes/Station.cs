using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
{
    public class Station
    {
        public event Action<OrderBox> OnOrderBoxFinished;
        public delegate void OnBoxReceived();
        public event OnBoxReceived OnOrderBoxReceivedAtStationEvent;
        
        public string Name { get; }
        
        private readonly List<ShelfBox> _shelfBoxes;
        public IEnumerable<ShelfBox> ShelfBoxes => _shelfBoxes.AsReadOnly();
        
        private readonly List<OrderBox> _orderBoxes;
        public IEnumerable<OrderBox> OrderBoxes => _orderBoxes.AsReadOnly();
        
        public int MaxShelfBoxes { get; }
        public int MaxOrderBoxes { get; }

        private int RemainingPickingTime = 0;

        private (OrderBox, Line, int) OrderBoxLineTime = (null, null, 0);

        private readonly Storage _storage;


        public Station(string name, int maxShelfBoxes,int maxOrderBoxes)
        {
            
            _shelfBoxes = new List<ShelfBox>(maxShelfBoxes);
            _orderBoxes = new List<OrderBox>(maxOrderBoxes);
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MaxShelfBoxes = maxShelfBoxes;
            MaxOrderBoxes = maxOrderBoxes;
            
            _storage = new Storage();

            _storage.OnShelfBoxReadyFromStorage += ReceiveShelfBoxFromStorage;

            TimeKeeper.Tick += TickToDo;

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
            Console.WriteLine("Station: Received box");
            switch (box)
            {
                case ShelfBox shelfBox:
                    if (_shelfBoxes.Count >= MaxShelfBoxes) return StationResult.FullError;
                    _shelfBoxes.Add(shelfBox);
                    break;
                case OrderBox orderBox:
                    if (_orderBoxes.Count >= MaxOrderBoxes) return StationResult.FullError;
                    _orderBoxes.Add(orderBox);
                    OnOrderBoxReceivedAtStationEvent?.Invoke();
                    break;
                case null:
                    throw new ArgumentNullException(nameof(box));
                default:
                    throw new ArgumentOutOfRangeException(nameof(box));
            }
            return StationResult.Success;
        }


        public void TickToDo()
        {
            if (_orderBoxes.Any())
            {
                PackLineIntoBox();
            }
            
        }


        private void CheckAndSendUnnecessaryShelfBoxes()
        {
            // Check each shelfbox
            foreach (var shelfBox in _shelfBoxes)
            {
                // Find all orderBoxes needing this shelfbox
                var orderBoxesNeedingThisShelfBox = _orderBoxes.Where(o => o.LinesNotPicked().Contains(shelfBox.Line));
                
                // If an unnecessary shelfbox is found
                if (!orderBoxesNeedingThisShelfBox.Any())
                {
                    // Send it to storage
                    SendShelfBoxToStorage(shelfBox);
                    
                    // Break, so only one shelfbox can be send at each tick
                    break;
                }
            }
        }

        private void PackLineIntoBox()
        {
            if (OrderBoxLineTime.Item3 > 0)
            {
                OrderBoxLineTime.Item3--;
                return;
            }
            
            // If packing is done, change the picked line status
            if (OrderBoxLineTime.Item1 != null && OrderBoxLineTime.Item3 == 0)
            {
                var orderBoxToUpdateProgressOn = _orderBoxes.First(o => o.Equals(OrderBoxLineTime.Item1));
                orderBoxToUpdateProgressOn.PutLineIntoBox(OrderBoxLineTime.Item2);
                CheckIfOrderBoxIsFinished(orderBoxToUpdateProgressOn);
            }

            bool pickableLineFound = false;
            
            // Find a box we can actually pack (the corresponding shelfbox is present)
            foreach (var orderBox in _orderBoxes)
            {
                var pickableLines = orderBox.LinesNotPicked()
                    .Where(l => _shelfBoxes.Select(s => s.Line)
                    .Contains(l)).ToList();
                
                if (pickableLines.Any())
                {
                    pickableLineFound = true;
                    var lineToPick = pickableLines.First();
                    var timeToPick = orderBox.LinesNotPicked().First(l => Equals(l, lineToPick)).Quantity;
                    OrderBoxLineTime = (orderBox, pickableLines.First(), timeToPick);
                }
            }

            if (!pickableLineFound)
            {
                CheckAndSendUnnecessaryShelfBoxes();
                
                RequestShelfBoxFromStorage(_orderBoxes
                    .First()
                    .LinesNotPicked()
                    .First()
                    .Article);
            }
        }
        
        
        public void CheckIfOrderBoxIsFinished(OrderBox orderBox)
        {
            if (!orderBox.LinesNotPicked().Any())
            {
                OnOrderBoxFinished?.Invoke(orderBox);
            }
            
            /*var minOrderBoxProgress = OBPContainer.GetNext();
            if (minOrderBoxProgress?.SecondsToSpend <= 0)
            {
                OBPContainer.Pop(); // Removes
                OnOrderBoxFinished?.Invoke(minOrderBoxProgress.OrderBox);
                
            }*/
        }


        private void RequestShelfBoxFromStorage(Article articleNeeded)
        {
            // Send request to storage
            _storage.ReceiveRequestForOrderBox(articleNeeded);
            
        }

        private void ReceiveShelfBoxFromStorage(ShelfBox shelfBox)
        {
            // If shelfBox is null throw an exception
            if(shelfBox == null) throw new ArgumentNullException(nameof(shelfBox));
            // If shelfBoxList is full then throw an exception
            if(_shelfBoxes.Count >= MaxShelfBoxes) throw new InvalidOperationException("Too many shelfboxes");
            // Add shelfBox to list of shelfboxes
            _shelfBoxes.Add(shelfBox);
        }
        
        private void SendShelfBoxToStorage(ShelfBox shelfBox)
        {
            // Send shelfBox to storage
            
            _shelfBoxes.Remove(shelfBox); // FAKED
        }
    }

    class Storage
    {
        public event Action<ShelfBox> OnShelfBoxReadyFromStorage;
        
        public void ReceiveRequestForOrderBox(Article articleNeeded)
        {
            var line = new Line(articleNeeded, 100000);
            var shelfBox = new ShelfBox(line);
            OnShelfBoxReadyFromStorage?.Invoke(shelfBox);   
        }
    }
}