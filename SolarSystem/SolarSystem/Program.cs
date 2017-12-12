using System;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes;
using System.IO;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var filePath =
                Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                    .ToString()) + "/SolarSystem.Backend/SolarData/";
            
            const int simSpeed = 1000;
            const double randomNewOrderChance = 0.1;
            const OrderGenerationConfiguration orderGenerationConfiguration = OrderGenerationConfiguration.FromFile;
            const SchedulerType schedulerType = SchedulerType.Fifo;
            const int daysToSimulate = 1;
            DateTime simulationStartTime = new DateTime(2017, 10, 2, 6, 0, 0); //02/10/2017
            DateTime schedulerStartTime = simulationStartTime.AddHours(0);
            
            var runner = new Runner(filePath, simSpeed, randomNewOrderChance, orderGenerationConfiguration, 
                schedulerType, daysToSimulate, simulationStartTime, schedulerStartTime);
           
            runner.Start();
            
            var consoleStatusPrinter = new ConsoleStatusPrinter(runner);
            consoleStatusPrinter.StartPrinting();
        }
    }
}