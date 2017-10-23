using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class Line
    {
        public ItemType ItemType { get; }
        public int Count { get; }


        public Line(ItemType itemType, int count)
        {
            ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
            Count = count;
            
        }


        public override string ToString()
        {
            return ($@"({ItemType}, {Count}");
        }
    }
}
