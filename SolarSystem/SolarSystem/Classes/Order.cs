using System;

namespace SolarSystem.Classes
{
    public class Order
    {
        public string OrderID { get; }
        public int TimeToFinish { get; }
        public DateTime OrderTime { get; set; }

        public Order(string orderID, int timeToFinish, DateTime orderTime)
        {
            OrderID = orderID ?? throw new ArgumentNullException(nameof(orderID));
            TimeToFinish = timeToFinish;
            OrderTime = orderTime;
        }

        public override string ToString()
        {
            return $"({OrderID}, {TimeToFinish})";
        }
    }
}
