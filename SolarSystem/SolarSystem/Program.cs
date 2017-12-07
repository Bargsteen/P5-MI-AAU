using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Ploeh.AutoFixture;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes;

namespace SolarSystem
{
    public class Program
    {


        public static void Main(string[] args)
        {
            /* var t = new Thread(() => TimeKeeper.StartTicking(100, DateTime.Now));
             t.Start();
             
             var fixture = new Fixture();
             var ob1 = fixture.Create<OrderBox>();
             var ob2 = fixture.Create<OrderBox>();
             var sut1 = new OrderBoxPickingContainer(ob1, ob1.LineIsPickedStatuses.Keys.First());
             sut1.OnLinePickedForOrderBox += box => PrintStatus("sut1 done");
             var sut2 = new OrderBoxPickingContainer(ob2, ob2.LineIsPickedStatuses.Keys.First());
             sut2.OnLinePickedForOrderBox += box => PrintStatus("sut2 done");
 */
            // 
            /* Runner runner = new Runner("/Users/Casper/Library/Projects/Uni/P5/wetransfer-f8286e/Picking 02-10-2017.csv",
                 1000, 0.2);*/
            //Runner runner = new Runner("C:/Picking 02-10-2017.csv", 5000, 0.2);
            Runner runner = new Runner("/Users/kasper/Downloads/wetransfer-f8286e/", 1000, 0.2,
                OrderGenerationConfiguration.FromFile);

            Console.WriteLine("Starting simulation!");

            Dictionary<AreaCode, int> FinishedBoxesInAreas = new Dictionary<AreaCode, int>()
            {
                {AreaCode.Area21, 0},
                {AreaCode.Area25, 0},
                {AreaCode.Area27, 0},
                {AreaCode.Area28, 0},
                {AreaCode.Area29, 0}
            };

            int totalFinishedOrders = 0;
            int finishedOrdersPerHour = 0;
            DateTime currentHour = runner.StartTime;
            List<Tuple<int, int>> ordersFinishedPerHour = new List<Tuple<int, int>>();

            //runner.Handler.OnOrderBoxFinished += o => PrintStatus($"Handler: Orderbox Finished {o} -- TimeSpend = {o.TimeInSystem}");

            runner.Handler.OnOrderBoxFinished += orderBox =>
            {
                totalFinishedOrders += orderBox.LineIsPickedStatuses.Keys.Count;
                finishedOrdersPerHour += orderBox.LineIsPickedStatuses.Keys.Count;
            };

            int index = 0;
            TimeKeeper.Tick += () =>
            {
                if (index++ > 60)
                {
                    if (TimeKeeper.CurrentDateTime.Hour == currentHour.Hour + 1)
                    {
                        ordersFinishedPerHour.Add(Tuple.Create(currentHour.Hour, finishedOrdersPerHour));

                        currentHour = TimeKeeper.CurrentDateTime;
                        finishedOrdersPerHour = 0;
                    }
                    
                   /* PrintBoxDict(FinishedBoxesInAreas);
                    PrintLinesFinishedPerHour(runner.StartTime, TimeKeeper.CurrentDateTime, totalFinishedOrders);
                    ordersFinishedPerHour.ForEach(x =>
                        Console.WriteLine(
                            "Lines between " + x.Item1 + " - " + (x.Item1 + 1) + ": " + x.Item2 + " lines"));
                    Console.WriteLine("Lines between " + TimeKeeper.CurrentDateTime.Hour + " - " +
                                      (TimeKeeper.CurrentDateTime.Hour + 1) + ": " + finishedOrdersPerHour + " lines");
                    index = 0;*/
                }
            };


            foreach (var area in runner.Areas)
            {
                //area.OnOrderBoxReceivedAtAreaEvent += (orderBox, areaCode) => PrintStatus($"{areaCode} << received orderBox {orderBox}");
                //area.OnOrderBoxInAreaFinished += (orderBox, areaCode) => PrintStatus($"{areaCode} >> finished orderBox {orderBox} - MainLoopCount: {runner.Handler.MainLoop.BoxesInMainLoop}");
                area.OnOrderBoxInAreaFinished += (orderBox, areaCode) =>
                {
                    Console.Clear();
                    IncrementBoxPerAreaCount(FinishedBoxesInAreas, areaCode);
                    PrintBoxDict(FinishedBoxesInAreas);
                    PrintLinesFinishedPerHour(runner.StartTime, TimeKeeper.CurrentDateTime, totalFinishedOrders);
                    ordersFinishedPerHour.ForEach(x => Console.WriteLine("Lines between " + x.Item1 + " - " + (x.Item1 + 1) + ": " + x.Item2 + " lines"));
                    Console.WriteLine("Lines between " + TimeKeeper.CurrentDateTime.Hour + " - " + (TimeKeeper.CurrentDateTime.Hour + 1) + ": " + finishedOrdersPerHour + " lines");
                };
            }
        }

        private static void IncrementBoxPerAreaCount(IDictionary<AreaCode, int> dict, AreaCode areaCode)
        {
            dict[areaCode]++;
        }

        private static void PrintBoxDict(Dictionary<AreaCode, int> dict)
        {
            
            string str = dict.Aggregate("", (current, kvp) => current + $"[{kvp.Key}: {kvp.Value}] ");
            PrintStatus(str);
        }

        private static void PrintLinesFinishedPerHour(DateTime startTime, DateTime currentTime, int totalFinishedLines)
        {
            TimeSpan timeSpent = currentTime - startTime;
            double linesPerHour = totalFinishedLines / timeSpent.TotalHours;
            Console.WriteLine($"Total lines: {totalFinishedLines}\nLines per hour: {linesPerHour}");
        }

        private static void PrintStatus(string status)
        {
            Console.WriteLine($"{TimeKeeper.CurrentDateTime} :: {status}");
        }
    }
}