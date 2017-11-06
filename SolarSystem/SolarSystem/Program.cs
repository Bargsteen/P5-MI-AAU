using System;
using System.Globalization;
using SolarSystem.Backend;

namespace SolarSystem
{
    public class Program
    {


        public static void Main(string[] args)
        {
            Runner runner = new Runner("/Users/Casper/Library/Projects/Uni/P5/wetransfer-f8286e/Picking 02-10-2017.csv",
                15, 0.2);

            Console.WriteLine("Starting simulation!");
            
            // Event Subscription Output
            //runner.Handler.OnOrderBoxFinished += o => Console.WriteLine($"Handler: Orderbox {o.Order}");
            //runner.Handler.MainLoop.OnOrderBoxInMainLoopFinished += (o, v) => Console.WriteLine("MainLoop: OnOrderBoxInMainLoopFinished.");
            //runner.Handler.Areas[0].OnOrderBoxInAreaFinished += (box, code) => Console.WriteLine($"{code}: OnOrderBoxInAreaFinished");
            runner.OrderGenerator.CostumerSendsOrderEvent +=
                Order => Console.WriteLine($"OrderGenerator: {Order.OrderId} Created.");
            
        }
    }
}