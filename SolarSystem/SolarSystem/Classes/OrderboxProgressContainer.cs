using System.Linq;
using C5;

namespace SolarSystem.Classes
{
    public class OrderboxProgressContainer
    {
        private readonly IntervalHeap<OrderBoxProgress> _orderboxHeap;

        public OrderboxProgressContainer()
        {
            _orderboxHeap = new IntervalHeap<OrderBoxProgress>();
        }

        public void AddOrderBoxProgress(OrderBoxProgress orderBoxProgress)
        {
            _orderboxHeap.Add(orderBoxProgress);
        }

        public OrderBoxProgress Pop() => _orderboxHeap.DeleteMin();

        public OrderBoxProgress GetNext()
        {
            return _orderboxHeap.Min();
        }
        
        
    }
}