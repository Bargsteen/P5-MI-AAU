using System;
using System.Collections.Generic;
using System.Linq;


namespace SolarSystem.Classes
{
    public class Station
    {
        public string Name { get; }
        private readonly List<ShelfBox> _shelfBoxes;
        public IEnumerable<ShelfBox> ShelfBoxes => _shelfBoxes.AsReadOnly();
        private readonly List<OrderBoxProgress> _orderBoxes;
        public IEnumerable<OrderBoxProgress> OrderBoxes => _orderBoxes.AsReadOnly();
        public int MaxShelfBoxes { get; }
        public int MaxOrderBoxes { get; }
        public event EventHandler<OrderBoxEventArgs> OrderBoxCompleteEvent;
        


        public Station(string name, int maxShelfBoxes,int maxOrderBoxes)
        {
            _shelfBoxes = new List<ShelfBox>(maxShelfBoxes);
            _orderBoxes = new List<OrderBoxProgress>(maxOrderBoxes);
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MaxShelfBoxes = maxShelfBoxes;
            MaxOrderBoxes = maxOrderBoxes;
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
                    //Wrap OderBox in Progress
                    // TODO: Proper secondsToSpend, And Datetime from Timekeeper
                    _orderBoxes.Add(new OrderBoxProgress(orderBox, DateTime.Now, 10));
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

            OrderBoxProgress result = _orderBoxes.First(x => x.SecondsToSpend <= 0);

            foreach (OrderBoxProgress item in _orderBoxes)
            {
                if (item.SecondsToSpend < result.SecondsToSpend)
                {
                    result = item;
                }
            }

            OrderBoxCompleteEvent(this, new OrderBoxEventArgs(result.OrderBox));
        }

        // TODO: Add timing
        //public void Step()
        //{
        //    foreach (var shelfBox in ShelfBoxes)
        //    {
        //        foreach (var orderBox in OrderBoxes)
        //        {
        //            if (orderBox.LinesNotPicked().Contains(shelfBox.Line))
        //            {
        //                orderBox.PutLineIntoBox(shelfBox.Line);
        //                return;
        //            }
        //        }
        //    }
        //}
        
        
        
    }
}