using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Backend.Classes.Simulation
{
    public static class Outputter
    {
        private static StreamWriter file = new StreamWriter(@"C:\output.csv");
        public static void WriteLineToFile(string s)
        {
                file.WriteLine(s);
        }
    }
}
