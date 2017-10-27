﻿using System;
using System.Collections.Generic;

namespace SolarSystem.Classes
{
    public class Station
    {
        public string Name { get; }
        private readonly List<ShelfBox> _shelfBoxes;
        public IEnumerable<ShelfBox> ShelfBoxes => _shelfBoxes.AsReadOnly();
        private readonly List<OrderBox> _orderBoxes;
        public IEnumerable<OrderBox> OrderBoxes => _orderBoxes.AsReadOnly();
        public int MaxShelfBoxes { get; }
        public int MaxOrderBoxes { get; }


        public Station(string name, int maxShelfBoxes,int maxOrderBoxes)
        {
            _shelfBoxes = new List<ShelfBox>(maxShelfBoxes);
            _orderBoxes = new List<OrderBox>(maxOrderBoxes);
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
                    _orderBoxes.Add(orderBox);
                    break;
                case null:
                    throw new ArgumentNullException(nameof(box));
                default:
                    throw new ArgumentOutOfRangeException(nameof(box));
            }
            return StationResult.Success;
        }

        // TODO: Add timing
        public void Step()
        {
            foreach (var shelfBox in ShelfBoxes)
            {
                foreach (var orderBox in OrderBoxes)
                {
                    if (orderBox.LinesNotPicked().Contains(shelfBox.Line))
                    {
                        orderBox.PutLineIntoBox(shelfBox.Line);
                        return;
                    }
                }
            }
        }
        
        
        
    }
}