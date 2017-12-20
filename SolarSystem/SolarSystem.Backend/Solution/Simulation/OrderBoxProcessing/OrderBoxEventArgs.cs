using System;
using SolarSystem.Backend.Solution.Simulation.Boxes;

namespace SolarSystem.Backend.Solution.Simulation.OrderBoxProcessing
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
