using System;
using System.Globalization;
using SolarSystem.Backend;

namespace SolarSystem
{
    public class Program
    {
        
        
        public static void Main(string[] args)
        {
            // 
            Runner runner = new Runner("/Users/kasper/Downloads/wetransfer-f8286e/Picking 02-10-2017.csv",
                300, 0.2);

            Console.WriteLine("Starting simulation!");
            
            // Event Subscription Output
            //runner.Handler.OnOrderBoxFinished += o => Console.WriteLine($"Handler: Orderbox {o.Order}");
            //runner.Handler.MainLoop.OnOrderBoxInMainLoopFinished += (o, v) => Console.WriteLine("MainLoop: OnOrderBoxInMainLoopFinished.");
            //runner.Handler.Areas[0].OnOrderBoxInAreaFinished += (box, code) => Console.WriteLine($"{code}: OnOrderBoxInAreaFinished");
            runner.OrderGenerator.CostumerSendsOrderEvent +=
                order => Console.WriteLine($"OrderGenerator: {order.OrderId} Created.");
            
        }
    }
}