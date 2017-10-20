using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    class OrderBox
    {
        public Order Order { get; }
        public int TimeRemaining { get; }

        public OrderBox(Order order)
        {
            Order = order;
            TimeRemaining = order.TimeToFinish;
        }

        public override string ToString()
        {
            return $@"({Order}, {TimeRemaining}";
        }
    }
}
