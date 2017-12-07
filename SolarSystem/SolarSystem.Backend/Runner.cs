using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using SolarSystem.Backend.Classes;
using SolarSystem.Backend.PickingandERP;
using SolarSystem.PickingAndErp;
using Order = SolarSystem.PickingAndErp.Order;

namespace SolarSystem.Backend
{
    
    public class Runner
    {
        public readonly Handler Handler;
        public readonly OrderGenerator OrderGenerator;
        public readonly Scheduler Scheduler;
        
        public readonly MiScheduler MiScheduler;
        public readonly SimulationInformation SimulationInformation;

        public readonly DateTime StartTime;
     
        
        public Runner(string filePath, double simulationSpeed, double orderChance, OrderGenerationConfiguration orderGenerationConfiguration)
        {
            var pickNScrape = new PickingScrape(filePath + "Picking 02-10-2017.csv");
            pickNScrape.GetOrdersFromPicking();

            var orders = pickNScrape.OrderList;
            var erpscrape = new ErpScrape();
            erpscrape.ScrapeErp(filePath + "ErpTask_trace.log");

            erpscrape.orders.Sort((x,y) => x.OrderTime.CompareTo(y.OrderTime));


            for (int i = 0; i < orders.Count; i++)
            {
                Order order = orders[i];

                try
                {
                    order.OrderTime = erpscrape.orders.Find(x => x.OrderNumber == order.OrderNumber).OrderTime;
                }
                catch (NullReferenceException)
                {
                    orders.Remove(order);
                    i--;
                }
            }
           

            List<Article> articleList = orders
                .SelectMany(o => o.LineList)
                .Distinct()
                .Select(line => line.Article)
                .ToList();

            Handler = new Handler(); 
            OrderGenerator = new OrderGenerator(articleList, orderChance, orders, orderGenerationConfiguration);
            
            StartTime = new DateTime(2017, 10, 2, 8, 0, 0); //02/10/2017
            
            var t = new Thread(() => TimeKeeper.StartTicking(simulationSpeed, StartTime));
            t.Start();
                       
            //Scheduler = new Scheduler(OrderGenerator, Handler, 0.0001);
            //MiScheduler = new MiScheduler(5, articleList.ToArray(), SimulationInformation, OrderGenerator, Handler);
            FifoScheduler fifoScheduler = new FifoScheduler(OrderGenerator, Handler, 4);
        }


        public IEnumerable<Area> Areas => Handler.Areas.Values;


    }

}