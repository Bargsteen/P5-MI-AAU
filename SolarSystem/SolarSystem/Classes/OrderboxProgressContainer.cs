using System.Linq;
using C5;

namespace SolarSystem.Classes
{
    public class OrderboxProgressContainer
    {
        private IntervalHeap<OrderBoxProgress> _orderboxHeap;

        public OrderboxProgressContainer()
        {
            _orderboxHeap = new IntervalHeap<OrderBoxProgress>();
        }

        public void AddOrderBoxProgress(OrderBoxProgress orderBoxProgress)
        {
            _orderboxHeap.Add(orderBoxProgress);
        }

        public OrderBoxProgress GetNext()
        {
            return _orderboxHeap.Min();
        }
        
        
    }
}