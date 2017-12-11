using System;
using System.Collections;
using System.Collections.Generic;
using Accord.Math;

namespace SolarSystem.Backend.Classes.Simulation
{
    public class Order : IEnumerable<Line>
    {
        public int OrderId { get; }
        public DateTime OrderTime { get;}
        public List<Line> Lines { get;}
        public Dictionary<AreaCode, bool> Areas { get; set; }

        public Order(int orderId, DateTime orderTime, List<Line> lines)
        {
            OrderId = orderId;
            OrderTime = orderTime;
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
        }

        public IEnumerator<Line> GetEnumerator()
        {
            return Lines.GetEnumerator();
        }

        public override string ToString()
        {
            return $"{OrderId}";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
   
}
