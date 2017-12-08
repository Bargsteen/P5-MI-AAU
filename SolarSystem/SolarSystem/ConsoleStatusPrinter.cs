using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem
{
    public class ConsoleStatusPrinter
    {
        private int _totalFinishedOrders;
        private int _finishedOrdersPerHour;
        private DateTime _currentHour;
        private readonly List<Tuple<int, int>> _ordersFinishedPerHour;
        private readonly Dictionary<AreaCode, int> _finishedBoxesInAreas;
        
        private readonly Runner _runner;

        public ConsoleStatusPrinter(Runner runner)
        {
            _runner = runner;

            _currentHour = _runner.StartTime;
            _ordersFinishedPerHour = new List<Tuple<int, int>>();
            
            TimeKeeper.SimulationFinished += PrintSimulationFinished;
            
            _finishedBoxesInAreas = new Dictionary<AreaCode, int>
            {
                {AreaCode.Area21, 0},
                {AreaCode.Area25, 0},
                {AreaCode.Area27, 0},
                {AreaCode.Area28, 0},
                {AreaCode.Area29, 0}
            };
        }


        private int totalFinishedOrders = 0;
        private int finishedOrdersPerHour = 0;
        DateTime currentHour;
        private readonly List<Tuple<int, int>> ordersFinishedPerHour = new List<Tuple<int, int>>();
        Dictionary<Area, List<Tuple<DateTime, int>>> areaLines = new Dictionary<Area, List<Tuple<DateTime, int>>>();
        Dictionary<Area, double> areaStandartDeviation = new Dictionary<Area, double>();
        bool firstIteration = true;
        double average;
        double sumOfSquaresOfDifferences;
        double sd;

            //runner.Handler.OnOrderBoxFinished += o => PrintStatus($"Handler: Orderbox Finished {o} -- TimeSpend = {o.TimeInSystem}");

        public void StartPrinting()
        {
            Console.WriteLine("Starting simulation!");


            _runner.Handler.OnOrderBoxFinished += orderBox =>
            {
                _totalFinishedOrders += orderBox.LineIsPickedStatuses.Keys.Count;
                _finishedOrdersPerHour += orderBox.LineIsPickedStatuses.Keys.Count;
            };
            
            var index = 0;
            TimeKeeper.Tick += () =>
            {
                if (index++ > 60)
                    if (TimeKeeper.CurrentDateTime.Hour == currentHour.Hour + 1 || TimeKeeper.CurrentDateTime.Hour == 0)

                    {
                        PrintFullStatus();
                        
                        _ordersFinishedPerHour.Add(Tuple.Create(_currentHour.Hour, _finishedOrdersPerHour));


                        currentHour = TimeKeeper.CurrentDateTime;
                        finishedOrdersPerHour = 0;

                        foreach (var _a in areaLines.Keys)
                        {
                            //using (StreamWriter dataWriter = new StreamWriter(@"Data/" + _a.Key.ToString() + ".xml"))
                            using (StreamWriter dataWriter = new StreamWriter("Data/" + _a + ".xml", firstIteration))
                            {
                                areaLines[_a].ForEach(x => dataWriter.WriteLine(x.Item1 + ", " + x.Item2));
                                dataWriter.Close();
                            }

                            using (StreamWriter dataWriter = new StreamWriter("Data/StandartDeviation" + _a + ".txt", firstIteration))
                            {
                                average = 0;
                                sumOfSquaresOfDifferences = 0;
                                if (!areaStandartDeviation.ContainsKey(_a))
                                {
                                    average = areaLines[_a].Average(v => v.Item2);
                                    sumOfSquaresOfDifferences = areaLines[_a].Select(val => (val.Item2 - average) * (val.Item2 - average)).Sum();
                                    sd = Math.Sqrt(sumOfSquaresOfDifferences / areaLines[_a].Count());
                                    areaStandartDeviation[_a] = sd;
                                }
                                else
                                {
                                    average = areaLines[_a].Average(v => v.Item2);
                                    sumOfSquaresOfDifferences = areaLines[_a].Select(val => (val.Item2 - average) * (val.Item2 - average)).Sum();
                                    sd = Math.Sqrt(sumOfSquaresOfDifferences / areaLines[_a].Count());
                                    areaStandartDeviation[_a] = ((areaStandartDeviation[_a] + sd) / 2);
                                }

                                dataWriter.WriteLine("This is the sd: " + areaStandartDeviation[_a]);
                                dataWriter.Close();
                            }
                        }
                        areaLines.Clear();
                        firstIteration = false;
                    }
            };


            foreach (var area in _runner.Areas)
                area.OnOrderBoxInAreaFinished += (orderBox, areaCode) =>
                {

                    //Console.Clear();
                    IncrementBoxPerAreaCount(_finishedBoxesInAreas, areaCode);
                    PrintBoxDict(_finishedBoxesInAreas);
                    PrintLinesFinishedPerHour(_runner.StartTime, TimeKeeper.CurrentDateTime, totalFinishedOrders);
                    ordersFinishedPerHour.ForEach(x =>
                        Console.Write("[ " + x.Item1 + " - " + (x.Item1 + 1) + " : " + x.Item2 + " ] "));
                    Console.WriteLine();
                    Console.WriteLine("Lines between " + TimeKeeper.CurrentDateTime.Hour + " - " +
                                      (TimeKeeper.CurrentDateTime.Hour + 1) + ": " + finishedOrdersPerHour +
                                      " lines");

                    if (areaLines.ContainsKey(area))
                    {
                        areaLines[area].Add(new Tuple<DateTime, int>(TimeKeeper.CurrentDateTime, orderBox.LineIsPickedStatuses.Keys.Count()));
                    }
                    else
                    {
                        areaLines.Add(area, new List<Tuple<DateTime, int>> { new Tuple<DateTime, int>(TimeKeeper.CurrentDateTime, orderBox.LineIsPickedStatuses.Keys.Count()) });
                    }

                    IncrementBoxPerAreaCount(_finishedBoxesInAreas, areaCode);

                };
        }

        private void PrintFullStatus()
        {
            Console.Clear();
            PrintBoxDict(_finishedBoxesInAreas);
            PrintLinesFinishedPerHour(_runner.StartTime, TimeKeeper.CurrentDateTime, _totalFinishedOrders);
            Console.WriteLine("Lines between " + TimeKeeper.CurrentDateTime.Hour + " - " +
                              (TimeKeeper.CurrentDateTime.Hour + 1) + ": " + _finishedOrdersPerHour +
                              " lines");
            _ordersFinishedPerHour.ForEach(x =>
                Console.Write("[ " + x.Item1 + " - " + (x.Item1 + 1) + " : " + x.Item2 + " ] "));           
        }

        private static void IncrementBoxPerAreaCount(IDictionary<AreaCode, int> dict, AreaCode areaCode)
        {
            dict[areaCode]++;
        }

        private static void PrintBoxDict(Dictionary<AreaCode, int> dict)
        {
            var str = dict.Aggregate("", (current, kvp) => current + $"[{kvp.Key}: {kvp.Value}] ");
            PrintStatus(str);
        }

        private static void PrintLinesFinishedPerHour(DateTime startTime, DateTime currentTime,
            int totalFinishedLines)
        {
            var timeSpent = currentTime - startTime;
            var linesPerHour = totalFinishedLines / timeSpent.TotalHours;
            Console.WriteLine($"Total lines: {totalFinishedLines}\nLines per hour: {linesPerHour}");
        }

        private static void PrintStatus(string status)
        {
            Console.WriteLine($"{TimeKeeper.CurrentDateTime} :: {status}");
        }

        private void PrintSimulationFinished()
        {
            PrintFullStatus();
            Console.WriteLine("\nSimulation Finished!");
        }
    }
}