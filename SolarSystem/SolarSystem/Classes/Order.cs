using System;

namespace SolarSystem.Classes
{
    public class Order
    {
        public string OrderName { get; }
        public int TimeToFinish { get; }
        

        public Order(string orderName, int timeToFinish)
        {
            OrderName = orderName ?? throw new ArgumentNullException(nameof(orderName));
            TimeToFinish = timeToFinish;
        }


        public override string ToString()
        {
            return $"({OrderName}, {TimeToFinish})";
        }
    }
}
