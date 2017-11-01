using System;

namespace SolarSystem.Classes
{
    public class OrderBoxProgress : IComparable
    {
        public OrderBox OrderBox { get; }
        public DateTime TimeOfArrival { get; }
        public int SecondsToSpend { get; private set; }

        public OrderBoxProgress(OrderBox order, DateTime timeOfArrival, int secondsToSpend)
        {
            OrderBox = order ?? throw new ArgumentNullException(nameof(order));
            TimeOfArrival = timeOfArrival;
            SecondsToSpend = secondsToSpend;

            TimeKeeper.Tick += OneSecondSpent;
        }

        private void OneSecondSpent()
        {
            SecondsToSpend -= 1;
        }

        public int CompareTo(object obj)
        {
            if (obj is OrderBoxProgress orderBoxProgressObj)
            {
                return SecondsToSpend < orderBoxProgressObj.SecondsToSpend ? -1 :
                    SecondsToSpend > orderBoxProgressObj.SecondsToSpend ? 1 : 0;
            }
            return 0;
        }
    }
}