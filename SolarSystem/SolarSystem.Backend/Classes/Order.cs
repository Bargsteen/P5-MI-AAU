using System;
using System.Collections.Generic;

namespace SolarSystem.Backend.Classes
{
    public class Order
    {
        public string OrderId { get; }
        public int TimeToFinish { get; }
        public DateTime OrderTime { get;}
        public List<Line> Lines { get;}
        public AreaCode StartAreaCode { get; set; }
        public Dictionary<AreaCode, bool> Areas { get; set; }

        public Order(string orderName, int timeToFinish, DateTime orderTime, List<Line> lines)
        {
           // OrderID = orderID ?? throw new ArgumentNullException(nameof(orderID));
            TimeToFinish = timeToFinish;
            OrderTime = orderTime;
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
        }
        
        public override string ToString()
        {
            return $"({OrderId}, {TimeToFinish})";
        }
    }
   
}
