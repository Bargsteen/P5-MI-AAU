using System;
using System.Collections.Generic;

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
                case PickingOrder.LastInFirstOut:
                    foreach (Order o in orders)
                    {
                        if (o.TimeToFinish > orders[desireredOrder].TimeToFinish) //FIX, TimeToFinish should be ordertime?
                        {
                            desireredOrder = orders.FindIndex(x => x.OrderName == o.OrderName);
                        }
                    }
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(pOrder), pOrder, null);
            }

            return orders[desireredOrder];
        }

    }
}
