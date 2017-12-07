using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes;

namespace SolarSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var filePath = "/Users/Casper/Library/Projects/Uni/P5/wetransfer-f8286e/";
            //var filePath = "C:/";
            
            var filePath = "/Users/kasper/Downloads/wetransfer-f8286e/";
            
            var runner = new Runner(filePath, 86400, 0.2, OrderGenerationConfiguration.FromFile, SchedulerType.FIFO);
            runner.Start();
            
            var consoleStatusPrinter = new ConsoleStatusPrinter(runner);
            consoleStatusPrinter.StartPrinting();
        }
    }
}