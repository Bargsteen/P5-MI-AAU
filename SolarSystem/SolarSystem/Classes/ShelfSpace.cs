using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    public class ShelfSpace
    {
        public string Name;
        public List<ShelfBox> ListOfBoxes;

        public ShelfSpace(string name, List<ShelfBox> listOfBoxes)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ListOfBoxes = listOfBoxes ?? throw new ArgumentNullException(nameof(listOfBoxes));
        }
    }
}
