using System;

namespace SolarSystem.Classes
{
    public class OrderBoxProgress : IComparable
    {
        public Order Order { get; }
        public DateTime TimeOfArrival { get; }
        public int SecondsToSpend { get; private set; }

        public OrderBoxProgress(Order order, DateTime timeOfArrival, int secondsToSpend)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
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