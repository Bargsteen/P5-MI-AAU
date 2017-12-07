using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SolarSystem.Backend.Classes
{
    public class ConsoleStatusPrinter
    {
        private readonly Runner _runner;

        public ConsoleStatusPrinter(Runner runner)
        {
            _runner = runner;
        }

        public void StartPrinting()
        {
            Console.WriteLine("Starting simulation!");

            var FinishedBoxesInAreas = new Dictionary<AreaCode, int>
            {
                {AreaCode.Area21, 0},
                {AreaCode.Area25, 0},
                {AreaCode.Area27, 0},
                {AreaCode.Area28, 0},
                {AreaCode.Area29, 0}
            };

            var totalFinishedOrders = 0;
            var finishedOrdersPerHour = 0;
            DateTime currentHour = _runner.StartTime;
            var ordersFinishedPerHour = new List<Tuple<int, int>>();
            Dictionary<Area, List<Tuple<DateTime, int>>> areaLines = new Dictionary<Area, List<Tuple<DateTime, int>>>();
            Dictionary<AreaCode, double> areaStandartDeviation = new Dictionary<AreaCode, double>()
            {
                {AreaCode.Area21, 0},
                {AreaCode.Area25, 0},
                {AreaCode.Area27, 0},
                {AreaCode.Area28, 0},
                {AreaCode.Area29, 0}
            };
            bool firstIteration = true;
            double average;
            double sumOfSquaresOfDifferences;
            double sd;

            //runner.Handler.OnOrderBoxFinished += o => PrintStatus($"Handler: Orderbox Finished {o} -- TimeSpend = {o.TimeInSystem}");

            _runner.Handler.OnOrderBoxFinished += orderBox =>
            {
                totalFinishedOrders += orderBox.LineIsPickedStatuses.Keys.Count;
                finishedOrdersPerHour += orderBox.LineIsPickedStatuses.Keys.Count;
            };
            
            var index = 0;
            TimeKeeper.Tick += () =>
            {
                if (index++ > 60)
                    if (TimeKeeper.CurrentDateTime.Hour == currentHour.Hour + 1 || TimeKeeper.CurrentDateTime.Hour == 0)
                    {
                        ordersFinishedPerHour.Add(Tuple.Create(currentHour.Hour, finishedOrdersPerHour));

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
                                if (!areaStandartDeviation.ContainsKey(_a.AreaCode))
                                {
                                    average = areaLines[_a].Average(v => v.Item2);
                                    sumOfSquaresOfDifferences = areaLines[_a].Select(val => (val.Item2 - average) * (val.Item2 - average)).Sum();
                                    sd = Math.Sqrt(sumOfSquaresOfDifferences / areaLines[_a].Count());
                                    areaStandartDeviation[_a.AreaCode] = sd;
                                }
                                else
                                {
                                    average = areaLines[_a].Average(v => v.Item2);
                                    sumOfSquaresOfDifferences = areaLines[_a].Select(val => (val.Item2 - average) * (val.Item2 - average)).Sum();
                                    sd = Math.Sqrt(sumOfSquaresOfDifferences / areaLines[_a].Count());
                                    areaStandartDeviation[_a.AreaCode] = ((areaStandartDeviation[_a.AreaCode] + sd) / 2);
                                }

                                dataWriter.WriteLine("This is the sd: " + areaStandartDeviation[_a.AreaCode]);
                                dataWriter.Close();
                            }
                        }
                        areaLines.Clear();
                        firstIteration = false;
                    }
            };


            foreach (var area in _runner.Areas)
                //area.OnOrderBoxReceivedAtAreaEvent += (orderBox, areaCode) => PrintStatus($"{areaCode} << received orderBox {orderBox}");
                //area.OnOrderBoxInAreaFinished += (orderBox, areaCode) => PrintStatus($"{areaCode} >> finished orderBox {orderBox} - MainLoopCount: {runner.Handler.MainLoop.BoxesInMainLoop}");
                area.OnOrderBoxInAreaFinished += (orderBox, areaCode) =>
                {
                    //Console.Clear();
                    IncrementBoxPerAreaCount(FinishedBoxesInAreas, areaCode);
                    PrintBoxDict(FinishedBoxesInAreas);
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
                };
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
    }
}