using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public static class OrderBoxPicker
    {
        public enum PickingOrder
        {
            FirstInFirstOut,
            LastInFirstOut
            //ActiveArticlesFirst
        };

        public static Order GetNext (PickingOrder pOrder, List<Order> orders)
        {
            int desireredOrder = 0;
            switch (pOrder)
            {
                case PickingOrder.FirstInFirstOut:
                    foreach(Order o in orders)
                    {
                        if(o.TimeToFinish < orders[desireredOrder].TimeToFinish) //FIX, TimeToFinish should be ordertime?
                        {
                            desireredOrder = orders.FindIndex(x => x.OrderName == o.OrderName);
                        } 
                    }
                break;
            }


            switch (pOrder)
            {
                case PickingOrder.LastInFirstOut:
                    foreach (Order o in orders)
                    {
                        if (o.TimeToFinish > orders[desireredOrder].TimeToFinish) //FIX, TimeToFinish should be ordertime?
                        {
                            desireredOrder = orders.FindIndex(x => x.OrderName == o.OrderName);
                        }
                    }
                break;
            }

            return orders[desireredOrder];
        }

    }
}
