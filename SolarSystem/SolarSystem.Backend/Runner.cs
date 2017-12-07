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
     
        
        public Runner(string pickingPath, double simulationSpeed, double orderChance)
        {
            var pickNScrape = new PickingScrape(pickingPath);
            pickNScrape.GetOrdersFromPicking();

            var orders = pickNScrape.OrderList;
            var erpscrape = new ErpScrape();
            erpscrape.ScrapeErp("C:/ErpTask_trace.log");

            erpscrape.orders.Sort((x,y) => { return x.OrderTime.CompareTo(y.OrderTime); });

            //foreach (Order order in erpscrape.orders)
            //{
            //    Console.WriteLine(order.OrderNumber);
            //}

            for (int i = 0; i < orders.Count; i++)
            {
                Order order = orders[i];

                //Console.WriteLine(order.OrderNumber);
                //Console.WriteLine(erpscrape.orders[0].OrderNumber);
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
            OrderGenerator = new OrderGenerator(articleList, orderChance, orders, OrderGenerator.configuration.FromFile);
            
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