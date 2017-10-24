using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using SolarSystem.Classes;

namespace SolarSystem.Simulation.Stations
{
    public class Area
    {
        public string AreaName;
        public ImmutableArray<ItemType> AvaibleWares;
        public Station[] Stations;
        public ShelfSpace ShelfSpace;

        public Area(string areaName, ImmutableArray<ItemType> avaibleWares, Station[] stations, ShelfSpace shelfSpace)
        {
            AreaName = areaName ?? throw new ArgumentNullException(nameof(areaName));
            AvaibleWares = avaibleWares;
            Stations = stations ?? throw new ArgumentNullException(nameof(stations));
            ShelfSpace = shelfSpace ?? throw new ArgumentNullException(nameof(shelfSpace));
        }
    }
}
