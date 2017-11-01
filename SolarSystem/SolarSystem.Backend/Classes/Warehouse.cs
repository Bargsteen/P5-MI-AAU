using System;
using System.Collections.Generic;

namespace SolarSystem.Backend.Classes
{
    public class Warehouse
    {
        public string Name { get; set; }
        public List<Area> Areas { get; set; }

        public Warehouse(string name, List<Area> areas)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Areas = areas ?? throw new ArgumentNullException(nameof(areas));
        }
    }
}