using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class ShelfSpace
    {
        public string Name;
        public List<ShelfBox> ListOfBoxes;

        public ShelfSpace(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        public ShelfSpace(string name, List<ShelfBox> listOfBoxes) : this(name)
        {
            ListOfBoxes = listOfBoxes ?? throw new ArgumentNullException(nameof(listOfBoxes));
        }

        public ShelfSpace(string name, IEnumerable<ItemType> itemTypes) : this(name)
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
