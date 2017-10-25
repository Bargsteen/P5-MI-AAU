using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class ShelfSpace
    {
        public string Name { get; set; }
        public List<ShelfBox> ListOfBoxes { get; set; }

        public ShelfSpace(string name, List<ShelfBox> listOfBoxes)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ListOfBoxes = listOfBoxes ?? throw new ArgumentNullException(nameof(listOfBoxes));
        }
    }
}
