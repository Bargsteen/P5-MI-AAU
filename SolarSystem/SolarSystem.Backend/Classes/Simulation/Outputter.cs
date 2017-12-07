using System.Collections.Generic;
using System.IO;

namespace SolarSystem.Backend.Classes.Simulation
{
    public static class Outputter
    {

        public static List<string> LinesFromScrape = new List<string>();
        public static List<string> LinesFinished = new List<string>();

        private static StreamWriter file = new StreamWriter(@"C:\output.csv");
        public static void WriteLineToFile()
        {
                //string output = s;
                //output += ";" + LinesFinished.Where(l => l.Split(';')[1] == s.Split(';')[1]);

                

            LinesFromScrape.ForEach(s => Console.WriteLine(s + LinesFinished.Single(f => f.Split(';')[0] == s.Split(';')[0] && f.Split(';')[1] == s.Split(';')[1]).Split(';')[2] ));


        
        }
    }
}
