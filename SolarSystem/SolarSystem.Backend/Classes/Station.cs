using System;
using System.Collections.Generic;
using SolarSystem.Backend.Interfaces;

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
        
        private OrderboxProgressContainer _orderboxProgressContainer;


        public Station(string name, int maxShelfBoxes,int maxOrderBoxes)
        {
            
            _orderboxProgressContainer = new OrderboxProgressContainer();
            
            _shelfBoxes = new List<ShelfBox>(maxShelfBoxes);
            _orderBoxes = new List<OrderBox>(maxOrderBoxes);
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MaxShelfBoxes = maxShelfBoxes;
            MaxOrderBoxes = maxOrderBoxes;
            
            TimeKeeper.Tick += OrderBoxProgressChecker;
            
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
            OnOrderBoxReceivedAtStationEvent?.Invoke();
            Console.WriteLine("Station: Received box");
            switch (box)
            {
                case ShelfBox shelfBox:
                    if (_shelfBoxes.Count >= MaxShelfBoxes) return StationResult.FullError;
                    _shelfBoxes.Add(shelfBox);
                    break;
                case OrderBox orderBox:
                    if (_orderBoxes.Count >= MaxOrderBoxes) return StationResult.FullError;
                    _orderboxProgressContainer.AddOrderBoxProgress(PackToOrderboxProgress(orderBox));
                    break;
                case null:
                    throw new ArgumentNullException(nameof(box));
                default:
                    throw new ArgumentOutOfRangeException(nameof(box));
            }
            return StationResult.Success;
        }

        public void OrderBoxProgressChecker()
        {
            var minOrderBoxProgress = _orderboxProgressContainer.GetNext();
            if (minOrderBoxProgress?.SecondsToSpend <= 0)
            {
                _orderboxProgressContainer.Pop(); // Removes
                Console.WriteLine("Station: Sending back to Area");
                OnOrderBoxFinished?.Invoke(minOrderBoxProgress.OrderBox);
                
            }
            
        }
        
        private OrderBoxProgress PackToOrderboxProgress(OrderBox orderBox)
        {
            // Estimate time based on Loop Flow and areas
            int timeToSpend = EstimateTime();

            // Create new OrderBoxProgress based on orderbox and time.
            var orderBoxProgress =
                new OrderBoxProgress(orderBox, EstimateTime());

            // Return the new OrderBoxProgress.
            return orderBoxProgress;
        }

        private int EstimateTime()
        {
            return 10;
        }
    }
}