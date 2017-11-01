namespace SolarSystem.Classes
{
    public class ItemType
    {
        private string ItemName { get; }

        public ItemType(string itemName)
        {
            ItemName = itemName;
        }
        
        public override string ToString()
        {
            return ItemName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ItemType lineObj))
            {
                return false;
            }
            return Equals(lineObj);
        }

        private bool Equals(ItemType other)
        {
            return string.Equals(ItemName, other.ItemName);
        }

        public override int GetHashCode()
        {
            return (ItemName != null ? ItemName.GetHashCode() : 0);
        }
    }

}
