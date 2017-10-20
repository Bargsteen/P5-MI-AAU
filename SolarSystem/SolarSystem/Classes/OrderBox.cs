using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    class OrderBox
    {
        Order _order;
        int _timeRemaining;

        public OrderBox(Order order)
        {
            _order = order;
            _timeRemaining = order.TimeToFinish;
        }

        public Order Order { get { return _order; } }
        public int TimeRemaining { get { return _timeRemaining; } }

        public override string ToString()
        {
            return String.Format(@"({0}, {1}", Order, TimeRemaining );
        }
    }
}
