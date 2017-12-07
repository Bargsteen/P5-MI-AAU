using System;
using System.Collections.Generic;
using System.Linq;

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
                    if (TimeKeeper.CurrentDateTime.Hour == currentHour.Hour + 1)
                    {
                        ordersFinishedPerHour.Add(Tuple.Create(currentHour.Hour, finishedOrdersPerHour));

                        currentHour = TimeKeeper.CurrentDateTime;
                        finishedOrdersPerHour = 0;
                    }
            };


            foreach (var area in _runner.Areas)
                //area.OnOrderBoxReceivedAtAreaEvent += (orderBox, areaCode) => PrintStatus($"{areaCode} << received orderBox {orderBox}");
                //area.OnOrderBoxInAreaFinished += (orderBox, areaCode) => PrintStatus($"{areaCode} >> finished orderBox {orderBox} - MainLoopCount: {runner.Handler.MainLoop.BoxesInMainLoop}");
                area.OnOrderBoxInAreaFinished += (orderBox, areaCode) =>
                {
                    Console.Clear();
                    IncrementBoxPerAreaCount(FinishedBoxesInAreas, areaCode);
                    PrintBoxDict(FinishedBoxesInAreas);
                    PrintLinesFinishedPerHour(_runner.StartTime, TimeKeeper.CurrentDateTime, totalFinishedOrders);
                    ordersFinishedPerHour.ForEach(x =>
                        Console.Write("[ " + x.Item1 + " - " + (x.Item1 + 1) + " : " + x.Item2 + " ] "));
                    Console.WriteLine();
                    Console.WriteLine("Lines between " + TimeKeeper.CurrentDateTime.Hour + " - " +
                                      (TimeKeeper.CurrentDateTime.Hour + 1) + ": " + finishedOrdersPerHour +
                                      " lines");
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