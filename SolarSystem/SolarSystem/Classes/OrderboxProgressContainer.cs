using System.Linq;
using C5;

namespace SolarSystem.Classes
{
    public class OrderboxProgressContainer
    {
        private IntervalHeap<OrderBoxProgress> OrderboxHeap;

        public OrderboxProgressContainer()
        {
            OrderboxHeap = new IntervalHeap<OrderBoxProgress>();
        }

        public void AddOrderBoxProgress(OrderBoxProgress orderBoxProgress)
        {
            OrderboxHeap.Add(orderBoxProgress);
        }

        public OrderBoxProgress GetNext()
        {
            return OrderboxHeap.Min();
        }
        
        
    }
}