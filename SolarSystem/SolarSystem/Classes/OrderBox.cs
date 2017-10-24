using System;

namespace SolarSystem.Classes
{
    public class OrderBox : Box
    {
        public Order Order { get; }
        public int TimeRemaining { get; set; }

        public OrderBox(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            TimeRemaining = order.TimeToFinish;
        }

        public override string ToString()
        {
            return $"({Order}, {TimeRemaining})";
        }
    }
}
