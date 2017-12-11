using System;
using System.Collections.Generic;
using System.Linq;
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

        DateTime currentHour;
        Dictionary<Area, List<Tuple<DateTime, int>>> areaLines = new Dictionary<Area, List<Tuple<DateTime, int>>>();

            //runner.Handler.OnOrderBoxFinished += o => PrintStatus($"Handler: Orderbox Finished {o} -- TimeSpend = {o.TimeInSystem}");

        public void StartPrinting()
        {
            Console.WriteLine("Starting simulation!");

            _runner.Handler.OnOrderBoxFinished += orderBox =>
            {
                _totalFinishedOrders += orderBox.LineIsPickedStatuses.Keys.Count;
                _finishedOrdersPerHour += orderBox.LineIsPickedStatuses.Keys.Count;

                DataSavingOrder order = new DataSavingOrder(orderBox.Order);
                order.finishedOrderTime = TimeKeeper.CurrentDateTime;
                //orderBox.Order.Lines.ForEach(l => order.lines.Add(l);
                DataSaving.orders.Add(order);

            };

            var index = 0;
            TimeKeeper.Tick += () =>
            {
                if (index++ > 60)
                    if (TimeKeeper.CurrentDateTime.Hour == currentHour.Hour + 1 || TimeKeeper.CurrentDateTime.Hour == 0)
                    {
                        PrintFullStatus();
                        
                        _ordersFinishedPerHour.Add(Tuple.Create(_currentHour.Hour, _finishedOrdersPerHour));

                        _currentHour = TimeKeeper.CurrentDateTime;

                        _finishedOrdersPerHour = 0;
                    }
            };


            foreach (var area in _runner.Areas)
                area.OnOrderBoxInAreaFinished += (orderBox, areaCode) =>
                {
                    //Console.Clear();
                    IncrementBoxPerAreaCount(_finishedBoxesInAreas, areaCode);
                    PrintBoxDict(_finishedBoxesInAreas);
                    PrintLinesFinishedPerHour(_runner.StartTime, TimeKeeper.CurrentDateTime, _totalFinishedOrders);
                    _ordersFinishedPerHour.ForEach(x =>
                        Console.Write("[ " + x.Item1 + " - " + (x.Item1 + 1) + " : " + x.Item2 + " ] "));
                    Console.WriteLine();
                    Console.WriteLine("Lines between " + TimeKeeper.CurrentDateTime.Hour + " - " +
                                      (TimeKeeper.CurrentDateTime.Hour + 1) + ": " + _finishedOrdersPerHour +
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