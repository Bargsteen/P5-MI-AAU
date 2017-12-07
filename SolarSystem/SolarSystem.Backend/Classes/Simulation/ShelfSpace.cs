using System;
using System.Collections.Generic;

namespace SolarSystem.Backend.Classes.Simulation
{
    public class ShelfSpace
    {
        public string Name { get; set; }
        public List<ShelfBox> ListOfBoxes { get; set; }

        public ShelfSpace(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public ShelfSpace(string name, List<ShelfBox> listOfBoxes) : this(name)
        {
            ListOfBoxes = listOfBoxes ?? throw new ArgumentNullException(nameof(listOfBoxes));
        }

        public ShelfSpace(string name, IEnumerable<Article> itemTypes) : this(name)
        {
            ListOfBoxes = new List<ShelfBox>();
            foreach (var itemType in itemTypes)
            {
                var line = new Line(itemType, 100);
                var shelfBox = new ShelfBox(line);
                ListOfBoxes.Add(shelfBox);
            }
            
        }
    }
}
