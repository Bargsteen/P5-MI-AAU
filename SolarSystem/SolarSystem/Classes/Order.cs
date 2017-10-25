using System;
using System.Collections.Generic;

namespace SolarSystem.Classes
{
    public class Order
    {
        public string OrderName { get; }
        public int TimeToFinish { get; }
        public DateTime OrderTime { get;}
        public List<Line> Lines { get;}

        public Order(string orderName, int timeToFinish, DateTime orderTime, List<Line> lines)
        {
            OrderName = orderName ?? throw new ArgumentNullException(nameof(orderName));
            TimeToFinish = timeToFinish;
            OrderTime = orderTime;
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
        }

        public override string ToString()
        {
            return $"({OrderName}, {TimeToFinish})";
        }
    }
}
