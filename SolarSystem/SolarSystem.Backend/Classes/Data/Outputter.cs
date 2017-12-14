using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SolarSystem.Backend.Classes.Simulation
{
    public class Outputter
    {


        public Outputter(Runner runner)
        {
            runner.Handler.OnOrderBoxFinished += orderBox => 
                LinesFinished.Add(orderBox.Order.OrderId + ";" +
                                            TimeKeeper.CurrentDateTime.Hour + ":" + 
                                            TimeKeeper.CurrentDateTime.Minute + ":" + 
                                            TimeKeeper.CurrentDateTime.Second);
            TimeKeeper.SimulationFinished += WriteLineToFile;
        }
        
        
        public static List<string> LinesFromScrape = new List<string>();
        public static List<string> LinesFinished = new List<string>();
        
        private static StreamWriter _file = new StreamWriter(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
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
                        output += ";" + lineFinished.Split(';')[1];

                        DateTime plannedFinishTime = new DateTime(1,
                            1,
                            1,
                            int.Parse(s.Split(';')[1].Split(':')[0]),
                            int.Parse(s.Split(';')[1].Split(':')[1]),
                            int.Parse(s.Split(';')[1].Split(':')[2]));
                        DateTime actualFinishTime = new DateTime(1,
                            1,
                            1,
                            int.Parse(lineFinished.Split(';')[1].Split(':')[0]),
                            int.Parse(lineFinished.Split(';')[1].Split(':')[1]),
                            int.Parse(lineFinished.Split(';')[1].Split(':')[2]));

                        TimeSpan deltaTime = plannedFinishTime - actualFinishTime;

                        //if (lineFinished.Split(';')[0] == "150350")
                        //    throw new Exception();


                        output += ";" + TimeSpan.FromMinutes(deltaTime.TotalMinutes);

                        _file.WriteLine(output);
                    }   
                }
            }
        }
    }
}
