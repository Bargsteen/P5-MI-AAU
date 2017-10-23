using System;

namespace SolarSystem.Classes
{
    public class Order
    {
        public string OrderName { get; }
        public int TimeToFinish { get; }
        public DateTime OrderTime { get; set; }

        public Order(string orderName, int timeToFinish, DateTime orderTime)
        {
            OrderName = orderName ?? throw new ArgumentNullException(nameof(orderName));
            TimeToFinish = timeToFinish;
            OrderTime = orderTime;
        }

        public override string ToString()
        {
            return $"({OrderName}, {TimeToFinish})";
        }
    }
}
