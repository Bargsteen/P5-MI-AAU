using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class OrderBox
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
