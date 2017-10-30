using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class OrderBoxEventArgs: EventArgs
    {
        public OrderBoxEventArgs(OrderBox orderBox)
        {
            OrderBox = orderBox;
        }

        public OrderBox OrderBox { get; set; }
    }
}
