using System;
using System.Collections.Generic;

namespace SolarSystem.Backend.Classes
{
    public class Order
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

        public override string ToString()
        {
            return $"{OrderId}";
        }
    }
   
}
