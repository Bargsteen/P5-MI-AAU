using System;
using System.Globalization;
using System.Linq;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes;

namespace SolarSystem
{
    public class Program
    {
        
        
        public static void Main(string[] args)
        {
            // 
            Runner runner = new Runner("/Users/kasper/Downloads/wetransfer-f8286e/Picking 02-10-2017.csv",
                1000, 0.2);

            Console.WriteLine("Starting simulation!");
            
            // Event Subscription Output
            runner.Handler.OnOrderBoxFinished += o => PrintStatus($"Handler: Orderbox Finished {o} -- TimeSpend = {o.TimeInSystem}");
            /*foreach (var area in runner.Areas)
            {
                area.OnOrderBoxReceivedAtAreaEvent += areaCode => PrintStatus($"{areaCode} << received orderBox");
                area.OnOrderBoxInAreaFinished += (box, areaCode) => PrintStatus($"{areaCode} >> finished orderBox");
                area.Storage.OnSendShelfBoxToStation += () => PrintStatus($"{area} STORAGE >> send shelfBox");
                foreach (var station in area.Stations)
                {
                    station.OnShelfBoxNeededRequest += (station1, list) =>
                        PrintStatus($"Station {station1} >?> requesting shelfbox");
                    station.OnOrderBoxReceivedAtStationEvent +=
                        () => PrintStatus($"Station {station} << received orderBox");
                    station.OnOrderBoxFinishedAtStation +=
                        orderBox => PrintStatus($"Station {station} >> finished orderBox {orderBox.Order}");
                }
            }*/
            var firstArea = runner.Areas.First();
            
            firstArea.OnOrderBoxReceivedAtAreaEvent += (orderBox, areaCode) => PrintStatus($"Received {orderBox}");
            firstArea.OnOrderBoxInAreaFinished += (orderBox, areaCode) => PrintStatus($"Finished {orderBox}");


            var firstStation = firstArea.Stations.First();
            firstStation.OnOrderBoxReceivedAtStation += orderBox => PrintStatus($"Station Received {orderBox}");
            firstStation.OnOrderBoxFinishedAtStation += orderBox => PrintStatus($"Station Finished {orderBox}");
            //runner.Handler.MainLoop.OnOrderBoxInMainLoopFinished += (o, v) => Console.WriteLine("MainLoop: OnOrderBoxInMainLoopFinished.");
            //runner.Handler.Areas[0].OnOrderBoxInAreaFinished += (box, code) => Console.WriteLine($"{code}: OnOrderBoxInAreaFinished");
            //runner.OrderGenerator.CostumerSendsOrderEvent +=
            //  order => PrintStatus($"OrderGenerator: {order.OrderId} Created.");

        }


        private static void PrintStatus(string status)
        {
            Console.WriteLine($"{TimeKeeper.CurrentDateTime} :: {status}");
        }
    }
}