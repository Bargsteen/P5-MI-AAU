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
            Runner runner = new Runner("/Users/kasper/Downloads/wetransfer-f8286e/Picking 02-10-2017.csv",
                1000, 0.2);

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
            
            //runner.Handler.OnOrderBoxFinished += o => PrintStatus($"Handler: Orderbox Finished {o} -- TimeSpend = {o.TimeInSystem}");

            runner.Handler.OnOrderBoxFinished += orderBox =>
            {
                totalFinishedOrders += orderBox.LineIsPickedStatuses.Keys.Count;
            };
            
            foreach (var area in runner.Areas)
            {
                //area.OnOrderBoxReceivedAtAreaEvent += (orderBox, areaCode) => PrintStatus($"{areaCode} << received orderBox {orderBox}");
                //area.OnOrderBoxInAreaFinished += (orderBox, areaCode) => PrintStatus($"{areaCode} >> finished orderBox {orderBox} - MainLoopCount: {runner.Handler.MainLoop.BoxesInMainLoop}");
                area.OnOrderBoxInAreaFinished += (orderBox, areaCode) =>
                {
                    IncrementBoxPerAreaCount(FinishedBoxesInAreas, areaCode);
                    PrintBoxDict(FinishedBoxesInAreas);
                    PrintLinesFinishedPerHour(runner.StartTime, TimeKeeper.CurrentDateTime, totalFinishedOrders);
                };
                   
                //area.Storage.OnSendShelfBoxToStation += () => PrintStatus($"{area} STORAGE >> send shelfBox");
                foreach (var station in area.Stations)
                {
                    //station.OnShelfBoxNeededRequest += (station1, list) =>
                      //  PrintStatus($"Station {station1} >?> requesting shelfbox");
                    //station.OnOrderBoxReceivedAtStation += orderBox => PrintStatus($"Station {station} << received orderBox {orderBox}");
                    //station.OnOrderBoxFinishedAtStation +=
                      //  orderBox => PrintStatus($"Station {station} >> finished orderBox {orderBox.Order}");
                }
            }
           /* var firstArea = runner.Areas.First();
            
            firstArea.OnOrderBoxReceivedAtAreaEvent += (orderBox, areaCode) => PrintStatus($"Received {orderBox}");
            firstArea.OnOrderBoxInAreaFinished += (orderBox, areaCode) => PrintStatus($"Finished {orderBox}");


            var firstStation = firstArea.Stations.First();
            firstStation.OnOrderBoxReceivedAtStation += orderBox => PrintStatus($"Station Received {orderBox}");
            firstStation.OnOrderBoxFinishedAtStation += orderBox => PrintStatus($"Station Finished {orderBox}");*/
            //runner.Handler.MainLoop.OnOrderBoxInMainLoopFinished += (o, v) => Console.WriteLine("MainLoop: OnOrderBoxInMainLoopFinished.");
            //runner.Handler.Areas[0].OnOrderBoxInAreaFinished += (box, code) => Console.WriteLine($"{code}: OnOrderBoxInAreaFinished");
            //runner.OrderGenerator.CostumerSendsOrderEvent +=
            //  order => PrintStatus($"OrderGenerator: {order.OrderId} Created.");

        }




        private static void IncrementBoxPerAreaCount(Dictionary<AreaCode, int> dict, AreaCode areaCode)
        {
            dict[areaCode]++;
        }

        private static void PrintBoxDict(Dictionary<AreaCode, int> dict)
        {
            
            string str = "";
            foreach (var kvp in dict)
            {
                str += $"[{kvp.Key}: {kvp.Value}] ";
            }
            Console.Clear();
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