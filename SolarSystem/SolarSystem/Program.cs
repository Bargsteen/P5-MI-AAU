using System;
using System.Collections.Generic;
using SolarSystem.Backend;
using System.IO;
using SolarSystem.Backend.Classes.Data;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.PickingAndErp;

namespace SolarSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SimulationConfiguration.SeedType = RandomSeedType.Fixed;
            SimulationConfiguration.SimulationState = SimulationState.Experimental;
            
            var filePath =
                Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                    .ToString()) + "/SolarSystem.Backend/SolarData/";
            
            var pickNScrape = new PickingScrape(filePath + "Picking 02-10-2017.csv");
            pickNScrape.GetOrdersFromPicking();

            var orders = pickNScrape.OrderList;
            var orders2 = new List<PickingOrder>();
            orders2.AddRange(orders);
            
            const int simSpeed = 5000;
            const double randomNewOrderChance = 0.1;
            const OrderGenerationConfiguration orderGenerationConfiguration = OrderGenerationConfiguration.FromFile;
            
            const SchedulerType schedulerType = SchedulerType.Estimator;
            const int hoursToSimulate = 16;
            const int runsToDo = 1;
            DateTime simulationStartTime = new DateTime(2017, 10, 2, 6, 0, 0); //02/10/2017
            DateTime schedulerStartTime = simulationStartTime.AddHours(0);
            
            var runner = new Runner(filePath, simSpeed, randomNewOrderChance, orderGenerationConfiguration, 
                schedulerType, hoursToSimulate, simulationStartTime, schedulerStartTime, orders, runsToDo);
            
            
            //var outPutter = new Outputter(runner);
            //var dataSaver = new DataSaver(runner);
            //SaveData(pickNScrape.OrderList);
            
           
            Statistics stats = new Statistics(orders2, runner);
            
            runner.Start();
            
            var consoleStatusPrinter = new ConsoleStatusPrinter(runner, stats);
            consoleStatusPrinter.StartPrinting();
            
        }

        private static void SaveData(List<PickingOrder> orderList)
        {
            foreach (PickingOrder order in orderList)
            {
                DataSaver.Orders.Add(new DataSavingOrder(order.ToSimOrder()));
                Outputter.LinesFromScrape.Add(order.OrderNumber + ";" + order.OrderTime.Hour + ":" + order.OrderTime.Minute +
                                              ":" + order.OrderTime.Second);
            }
        }
        
        
    }
}