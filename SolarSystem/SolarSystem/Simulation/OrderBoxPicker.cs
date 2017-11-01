using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Classes;

namespace SolarSystem.Simulation
{
    public static class OrderBoxPicker
    {
        public enum PickingOrder
        {
            FirstInFirstOut,
            LastInFirstOut
            //ActiveArticlesFirst
        };


        public static Order GetNextOrder (PickingOrder pickingAlgorithm, IEnumerable<Order> orders)
        {
            switch (pickingAlgorithm)
            {
                case PickingOrder.FirstInFirstOut:
                    return orders.OrderBy(order => order.OrderTime).FirstOrDefault();
                case PickingOrder.LastInFirstOut:
                    return orders.OrderByDescending(order => order.OrderTime).FirstOrDefault();
                default:
                    throw new ArgumentOutOfRangeException(nameof(pickingAlgorithm), pickingAlgorithm, null);
            }
        }

    }
}
