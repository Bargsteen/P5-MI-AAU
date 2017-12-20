using System;
using SolarSystem.Backend.Classes.Simulation.Orders;

namespace SolarSystem.Backend.Classes.Simulation.Boxes
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
