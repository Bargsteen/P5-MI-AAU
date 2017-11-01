using System;

namespace SolarSystem.Backend.Classes
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

        public override bool Equals(object obj)
        {
            if (!(obj is Line lineObj))
            {
                return false;
            }
            return Equals(lineObj);
        }

        private bool Equals(Line other)
        {
            return Equals(ItemType, other.ItemType) && Count == other.Count;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ItemType != null ? ItemType.GetHashCode() : 0) * 397) ^ Count;
            }
        }

        public override string ToString()
        {
            return ($@"({ItemType}, {Count}");
        }
    }
}
