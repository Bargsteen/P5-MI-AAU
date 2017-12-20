using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem
{
    public class ConsoleStatusPrinter
    {
        private int _totalFinishedLines;
        private int _finishedLinesPerHour;
        private int _totalFinishedOrders;
        private DateTime _currentHour;
        private readonly List<Tuple<int, int>> _linesFinishedPerHour;
        private readonly Dictionary<AreaCode, int> _finishedBoxesInAreas;

        private readonly Runner _runner;

        private readonly Statistics _stats;
        private readonly DateTime _schedulerStartTime;
        private readonly SchedulerType _schedulerType;
        private readonly OrderGenerationConfiguration _orderGenerationConfiguration;

        public ConsoleStatusPrinter(Runner runner, Statistics stats, DateTime schedulerStartTime, 
            SchedulerType schedulerType, OrderGenerationConfiguration orderGenerationConfiguration)
        {
            _schedulerType = schedulerType;
            _orderGenerationConfiguration = orderGenerationConfiguration;
            _runner = runner;
            _stats = stats;
            _schedulerStartTime = schedulerStartTime;

            _currentHour = _runner.StartTime;
            _linesFinishedPerHour = new List<Tuple<int, int>>();

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

        public void StartPrinting()
        {
            Console.WriteLine("Starting simulation!");

            _runner.Handler.OnOrderBoxFinished += orderBox =>
            {
                _totalFinishedLines += orderBox.LineIsPickedStatuses.Keys.Count;
                _finishedLinesPerHour += orderBox.LineIsPickedStatuses.Keys.Count;
                _totalFinishedOrders += 1;

            };

            var index = 0;
            TimeKeeper.Tick += () =>
            {
                
                if (TimeKeeper.CurrentDateTime.Hour == _currentHour.Hour + 1)
                {
                    PrintFullStatus();
                    _linesFinishedPerHour.Add(Tuple.Create(_currentHour.Hour, _finishedLinesPerHour));
                    _currentHour = TimeKeeper.CurrentDateTime;
                    _finishedLinesPerHour = 0;
                }
            };


            foreach (var area in _runner.Areas)
                area.OnOrderBoxInAreaFinished += (orderBox, areaCode) =>
                {
                    IncrementBoxPerAreaCount(_finishedBoxesInAreas, areaCode);
                };
        }

        private void PrintFullStatus()
        {
            Console.Clear();
            Console.WriteLine($"Scheduler: {_schedulerType} || OrderGeneration: {_orderGenerationConfiguration}");
            PrintBoxDict(_finishedBoxesInAreas);
            PrintLinesFinishedPerHour(_schedulerStartTime, TimeKeeper.CurrentDateTime, _totalFinishedLines);
            Console.WriteLine($"Total finished orders: {_totalFinishedOrders}");
            Console.WriteLine("Lines between " + TimeKeeper.CurrentDateTime.Hour + " - " +
                              (TimeKeeper.CurrentDateTime.Hour + 1) + ": " + _finishedLinesPerHour +
                              " lines");
            _linesFinishedPerHour.ForEach(x =>
                Console.Write("[ " + x.Item1 + " - " + (x.Item1 + 1) + " : " + x.Item2 + " ] "));
        }

        private static void IncrementBoxPerAreaCount(IDictionary<AreaCode, int> dict, AreaCode areaCode)
        {
            dict[areaCode]++;
        }

        private void PrintLinesPerHourForStatistics()
        {
            _linesFinishedPerHour.ForEach( x => Console.WriteLine($"{x.Item2}"));
            Console.WriteLine();
        }

        private static void PrintBoxDict(Dictionary<AreaCode, int> dict)
        {
            var str = dict.Aggregate("", (current, kvp) => current + $"[{kvp.Key}: {kvp.Value}] ");
            PrintStatus(str);
        }

        private void PrintLinesFinishedPerHour(DateTime startTime, DateTime currentTime,
            int totalFinishedLines)
        {
            var hoursWithFinishedOrders = _linesFinishedPerHour.Count(x => x.Item2 != 0);


            double hoursSpent = hoursWithFinishedOrders;
            double linesPerHour = totalFinishedLines / hoursSpent;
            Console.WriteLine($"Total lines: {totalFinishedLines}\nLines per hour: {linesPerHour}");
        }

        private static void PrintStatus(string status)
        {
            Console.WriteLine($"{TimeKeeper.CurrentDateTime} :: {status}");
        }

        private void PrintSimulationFinished()
        {
            var ordersSentIn = _runner.OrdersSentIn;
            var ordersInMainLoop = _runner.Handler.MainLoop.BoxesInMainLoop;
            var ordersInStations = _runner.Handler.Areas.SelectMany(kvp => kvp.Value.Stations)
                .Sum(station => station.OrderBoxes.Count);
            
            PrintFullStatus();
            Console.WriteLine("\nSimulation Finished!");
            PrintLinesPerHourForStatistics();
            
            Console.WriteLine($"SOLAR :: Average Minutes Per Area: {_stats.CalcAverageTimePerAreaSolar()}");
            Console.WriteLine($"OURS :: Average Minutes Per Area: {_stats.GetFinalAverageTimePerAreaSim()}");
            Console.WriteLine($"OURS :: Slowest Ord: {_stats.GetSlowestOrderTime()}");
            Console.WriteLine($"Solar :: Avg line Count: {_stats.GetAverageLinesPerOrderSolar()}");
            Console.WriteLine($"OURS :: Avg line Count: {_stats.GetAverageLinesPerOrderSim()}");
            Console.WriteLine($"SOLAR :: Avg quantity per line: {_stats.GetAverageQuantityPerLineSolar()}");
            Console.WriteLine($"OURS :: Avg quantity per line: {_stats.GetAverageQuantityPerLineSim()}");

            Console.WriteLine(
                $"Orders In : {ordersSentIn} -- Orders Out : {_totalFinishedOrders}\nLeft in system: {ordersInMainLoop + ordersInStations}\n" + 
                $"Total out + left: {_totalFinishedOrders + ordersInMainLoop + ordersInStations}");

        }
    }
}