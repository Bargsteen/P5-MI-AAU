using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    class Line
    {
        public ItemType ItemType { get; }
        public int Count { get; }

        
        public Line(ItemType itemtype, int count)
        {
            ItemType = itemtype;
            Count = count;
        }

        
        public override string ToString()
        {
            return ($@"({ItemType}, {Count}");
        }
    }
}
