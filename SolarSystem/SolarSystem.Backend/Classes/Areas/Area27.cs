using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSystem.Interfaces;

namespace SolarSystem.Classes.Areas
{
    class Area27 : Area, IRecieveOrder
    {
        //Constructer from Area
        public Area27(string areaName, ImmutableArray<ItemType> availableWares, Station[] stations, ShelfSpace shelfSpace) : base(areaName, availableWares, stations, shelfSpace)
        {
        }

        //ReciedOrder from IReceiveOrder
        public Order _orderRecieved => throw new NotImplementedException();

        //ReceiveOrder method from IReceiveOrder
        public void RecieveOrder(OrderboxProgress order)
        {
            //mapping the input to the _orderReceived variable


        }

        


    }
}
