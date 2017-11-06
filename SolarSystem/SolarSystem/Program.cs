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
                5, 0.2);
            
            // Event Subscription Output
            runner.Handler.OnOrderBoxFinished += o => Console.WriteLine("Handler: OnOrderBoxFinished.");
            //runner.Handler.MainLoop.OnOrderBoxInMainLoopFinished += (o, v) => Console.WriteLine("MainLoop: OnOrderBoxInMainLoopFinished.");
            //runner.Handler.Areas[0].OnOrderBoxInAreaFinished += (box, code) => Console.WriteLine($"{code}: OnOrderBoxInAreaFinished");
        }
    }
}