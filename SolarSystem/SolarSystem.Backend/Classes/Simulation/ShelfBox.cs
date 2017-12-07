using System;

namespace SolarSystem.Backend.Classes.Simulation
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
