using System;

namespace SolarSystem.Classes
{
    public class OrderBoxProgressAndArea : OrderBoxProgress
    {
        public Area Area { get; }
        public OrderBoxProgressAndArea(Order order, DateTime timeOfArrival, int secondsToSpend, Area area) : base(order,
            timeOfArrival, secondsToSpend)
        {
            Area = area ?? throw new ArgumentNullException(nameof(area));
        }
    }
}