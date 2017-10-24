namespace SolarSystem.Classes
{
    public class ItemType
    {
        public string ItemName { get; }

        public ItemType(string itemName)
        {
            ItemName = itemName;
        }
        
        public override string ToString()
        {
            return ItemName;
        }
    }

}
