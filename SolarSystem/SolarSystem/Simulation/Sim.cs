using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using SolarSystem.Classes;


namespace SolarSystem.Simulation
{
    public class Sim
    {
        public Sim()
        {
            ImmutableArray<ItemType> area51Items =
            new ImmutableArray<ItemType> {new ItemType("Skruetrækker"), new ItemType("BoreMaskine")};
            Station area51Station = new Station("Area51Station", 5, 5);
            ShelfSpace shelfSpace = new ShelfSpace("Area51ShelfSpace", area51Items);
            Area area51 = new Area("Area51", area51Items, new [] {area51Station}, shelfSpace);
            Warehouse warehouse = new Warehouse("Ma WareHouse", new List<Area> {area51});
            

        }
           
       
    }
}