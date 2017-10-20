using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    class Line
    {
        ItemType _itemType;
        int _count;

        public Line(ItemType itemtype, int count)
        {
            _itemType = itemtype;
            _count = count;
        }

        public ItemType ItemType { get { return _itemType; } }
        public int Count { get { return _count; } }

        public override string ToString()
        {
            return (String.Format(@"({0}, {1}", _itemType, _count.ToString()));
        }
    }
}
