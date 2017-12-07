using System;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //const string filePath = "/Users/Casper/Library/Projects/Uni/P5/wetransfer-f8286e/";
            //const string filePath = "C:/";
            const string filePath = "/Users/kasper/Downloads/wetransfer-f8286e/";

            const int simSpeed = 200000;
            const double randomNewOrderChance = 0.1;
            const OrderGenerationConfiguration orderGenerationConfiguration = OrderGenerationConfiguration.FromFile;
            const SchedulerType schedulerType = SchedulerType.Fifo;
            const int daysToSimulate = 1;
            DateTime simulationStartTime = new DateTime(2017, 10, 2, 8, 0, 0); //02/10/2017
            DateTime schedulerStartTime = simulationStartTime.AddHours(4);

            var runner = new Runner(filePath, simSpeed, randomNewOrderChance, orderGenerationConfiguration, 
                schedulerType, daysToSimulate, simulationStartTime, schedulerStartTime);
            runner.Start();
            
            var consoleStatusPrinter = new ConsoleStatusPrinter(runner);
            consoleStatusPrinter.StartPrinting();
        }
    }
}