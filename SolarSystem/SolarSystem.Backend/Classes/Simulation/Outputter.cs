using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolarSystem.Backend.Classes.Simulation
{
    public static class Outputter
    {

        public static List<string> LinesFromScrape = new List<string>();
        public static List<string> LinesFinished = new List<string>();
        
        private static StreamWriter file = new StreamWriter(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
        .ToString()) + "/SolarSystem.Backend/SolarData/Output.csv");
        public static void WriteLineToFile()
        {
            foreach (string s in LinesFromScrape)
            {
                foreach (string lineFinished in LinesFinished)
                {
                    if (s.Split(';')[0] == lineFinished.Split(';')[0])
                    {
                        string output = s;
                        output += ";" + lineFinished.Split(';')[2];

                        DateTime PlannedFinishTime = new DateTime(1,
                            1,
                            1,
                            int.Parse(s.Split(';')[2].Split(':')[0]),
                            int.Parse(s.Split(';')[2].Split(':')[1]),
                            int.Parse(s.Split(';')[2].Split(':')[2]));
                        DateTime ActualFinishTime = new DateTime(1,
                            1,
                            1,
                            int.Parse(lineFinished.Split(';')[2].Split(':')[0]),
                            int.Parse(lineFinished.Split(';')[2].Split(':')[1]),
                            int.Parse(lineFinished.Split(';')[2].Split(':')[2]));

                        TimeSpan DeltaTime = PlannedFinishTime - ActualFinishTime;

                        output += ";" + TimeSpan.FromMinutes(Math.Abs(DeltaTime.TotalMinutes));

                        file.WriteLine(output);
                    }   
                }
            }
        }
    }
}
