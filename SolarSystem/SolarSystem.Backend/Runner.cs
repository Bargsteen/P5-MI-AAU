using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using SolarSystem.Backend.Classes;
using SolarSystem.Backend.Classes.Schedulers;
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

        private readonly double _simulationSpeed;
        private SchedulerModular _scheduler;
     
        
        public Runner(string filePath, double simulationSpeed, double orderChance, OrderGenerationConfiguration orderGenerationConfiguration, SchedulerType schedulerType)
        {
            

            var pickNScrape = new PickingScrape(filePath + "Picking 02-10-2017.csv");
            pickNScrape.GetOrdersFromPicking();

            _simulationSpeed = simulationSpeed;
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
            

            switch (schedulerType)
            {
                case SchedulerType.FIFO:
                    _scheduler = new FifoScheduler(OrderGenerator, Handler, 4);
                    break;
                case SchedulerType.MI1:
                    throw new NotImplementedException("MI1 is not implemented yet..");
                    break;
                case SchedulerType.MI2:
                    throw new NotImplementedException("MI2 is not implemented yet..");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(schedulerType), schedulerType, null);
            }
            
            
            
        }

        public void Start()
        {
            // Start Ticking
            var t = new Thread(() => TimeKeeper.StartTicking(_simulationSpeed, StartTime));
            t.Start();
            
            // Start Schedular
            _scheduler.Start();
            
            // Start OrderGeneration
            OrderGenerator.Start();
        }


        public IEnumerable<Area> Areas => Handler.Areas.Values;


    }

}