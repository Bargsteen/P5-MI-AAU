using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class ShelfBox : Box
    {
        public ShelfBox(Line line)
        {
            Line = line ?? throw new ArgumentNullException(nameof(line));
        }

        public Line Line { get; set; }

        


    }
}
