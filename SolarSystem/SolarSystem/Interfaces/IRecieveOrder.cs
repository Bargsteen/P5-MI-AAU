using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSystem.Classes;

namespace SolarSystem.Interfaces
{
    interface IRecieveOrder
    {
        //RecieveOrder. Takes an order as input and maps it to the temporary Order within the class.

        void RecieveOrder(OrderBoxProgress order); 

        
    }
}
