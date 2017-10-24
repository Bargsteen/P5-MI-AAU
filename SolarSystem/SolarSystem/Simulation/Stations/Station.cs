using System;
using System.Configuration;
using SolarSystem.Classes;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SolarSystem.Simulation.Stations
{
    public class Station
    {
        public string Name { get; set; }
        public ShelfBox[] ShelfBoxes { get; set; }
        public OrderBox[] OrderBoxes { get; set; }

        public Station(string name, ShelfBox[] shelfBoxes, OrderBox[] orderBoxes)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ShelfBoxes = shelfBoxes ?? throw new ArgumentNullException(nameof(shelfBoxes));
            OrderBoxes = orderBoxes ?? throw new ArgumentNullException(nameof(orderBoxes));
        }
    }
}