using System.Collections.Generic;
using System.IO;

namespace SolarSystem.Backend.Classes.Simulation
{
    public static class Outputter
    {

        public static List<string> LinesFinished = new List<string>();

        private static StreamWriter file = new StreamWriter(@"C:\output.csv");
        public static void WriteLineToFile(string s)
        {
               //File.Wr
        }
    }
}
