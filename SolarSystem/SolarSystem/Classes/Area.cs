using System;
using System.Collections.Immutable;

namespace SolarSystem.Classes
{
    public class Area
    {
        public string AreaName { get; set; }
        public ImmutableArray<ItemType> AvailableWares { get; set; }
        public Station[] Stations { get; }
        public ShelfSpace ShelfSpace { get; }

        public Area(string areaName, ImmutableArray<ItemType> availableWares, Station[] stations, ShelfSpace shelfSpace)
        {
            AreaName = areaName ?? throw new ArgumentNullException(nameof(areaName));
            AvailableWares = availableWares;
            Stations = stations ?? throw new ArgumentNullException(nameof(stations));
            ShelfSpace = shelfSpace ?? throw new ArgumentNullException(nameof(shelfSpace));
        }
    }
}
