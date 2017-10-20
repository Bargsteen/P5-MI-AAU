using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SolarSystem.Picking_and_ERP
{
    class PickingScrape
    {
        public PickingScrape(string Path)
        {
            _path = Path;
        }

        static string _path;



        var reader = new StreamReader(@_path);
    }
}
