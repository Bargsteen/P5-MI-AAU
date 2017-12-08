/*
 * Line Shortest Time Scheduler
 * Finds the order with the fewest amount of articles.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public class LSTScheduer : Scheduler
    {
        public LSTScheduer(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(orderGenerator, handler, poolMoverTime)
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
                foreach (Line line in order)
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