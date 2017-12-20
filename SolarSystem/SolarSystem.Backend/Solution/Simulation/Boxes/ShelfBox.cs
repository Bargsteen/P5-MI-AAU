using System;
using SolarSystem.Backend.Solution.Simulation.Orders;

namespace SolarSystem.Backend.Solution.Simulation.Boxes
{
    public class ShelfBox : Box
    {
        public ShelfBox(Line line)
        {
            Line = line ?? throw new ArgumentNullException(nameof(line));
        }

        public Line Line { get;}

        public override string ToString()
        {
            return Line.ToString();
        }
    }
}
