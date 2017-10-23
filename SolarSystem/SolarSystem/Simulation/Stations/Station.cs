using System.Configuration;
using SolarSystem.Classes;
using System.Collections
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SolarSystem.Simulation.Stations
{
    public class Station
    {
        public Area Area { get; set; }
        public ImmutableArray<ItemType> AvailableItems { get; set; }
        public ItemType[] ShelfBoxes { get; set; }
        public OrderBox[] OrderBoxes { get; set; }

        public Station(Area area, ImmutableArray<ItemType> availableItems, uint maxShelfBoxes, uint maxOrderBoxes)
        {
            Area = area;
            AvailableItems = availableItems;
            ShelfBoxes = new ItemType[maxOrderBoxes];
            OrderBoxes = new OrderBox[maxOrderBoxes];
        }
        
        
    }

    public enum Area
    {
        Area27,
        Area28,
        Area29
    }
}