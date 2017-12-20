/*
 * Line Shortest Time Scheduler
 * Finds the order with the fewest amount of articles.
 */


using System;
using System.Collections.Generic;
using SolarSystem.Backend.Solution.Simulation.Orders;
using SolarSystem.Backend.Solution.Simulation.Warehouse;

namespace SolarSystem.Backend.Solution.Schedulers
{
    public class LstScheduer : Scheduler
    {
        public LstScheduer(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(orderGenerator, handler, poolMoverTime)
        {
        }

        protected override Order ChooseNextOrder()
        {
            // Loop through all ActualOrders
            // Sort by Line quantity sum
            // Return the first one
            Order returnOrder = new Order(0, DateTime.Now, new List<Line>());
            var maxQuantity = 0;
            foreach (Order order in ActualOrderPool)
            {
                foreach (Line line in order.Lines)
                {
                    if (line.Quantity > maxQuantity)
                    {
                        returnOrder = order;
                    }
                }
            }
            return returnOrder;
        }
    }
}