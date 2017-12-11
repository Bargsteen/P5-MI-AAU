﻿using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.Classes.Data;

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

        public void StartPrinting()
        {
            Console.WriteLine("Starting simulation!");

            _runner.Handler.OnOrderBoxFinished += orderBox =>
            {
                _totalFinishedOrders += orderBox.LineIsPickedStatuses.Keys.Count;
                _finishedOrdersPerHour += orderBox.LineIsPickedStatuses.Keys.Count;

                DataSaving.orders.Where(o => o.Order.OrderId == orderBox.Order.OrderId)
                    .Select(o => o.finishedOrderTime = TimeKeeper.CurrentDateTime);
            };

            var index = 0;
            TimeKeeper.Tick += () =>
            {


                if (index++ > 60)
                    PrintFullStatus();
                if (TimeKeeper.CurrentDateTime.Hour == _currentHour.Hour + 1)
                {
                    _ordersFinishedPerHour.Add(Tuple.Create(_currentHour.Hour, _finishedOrdersPerHour));

                    _currentHour = TimeKeeper.CurrentDateTime;
                    _finishedOrdersPerHour = 0;
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
            //Console.Clear();
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