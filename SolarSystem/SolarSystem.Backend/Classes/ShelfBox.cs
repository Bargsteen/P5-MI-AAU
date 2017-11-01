using System;

namespace SolarSystem.Backend.Classes
{
    public class ShelfBox : Box
    {
        public ShelfBox(Line line)
        {
            Line = line ?? throw new ArgumentNullException(nameof(line));
        }

        public Line Line { get;}

        


    }
}
