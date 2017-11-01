using SolarSystem.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Interfaces
{
    interface IReceiveOrder
    {
        void ReceiveOrder(OrderBox order);
    }
}
