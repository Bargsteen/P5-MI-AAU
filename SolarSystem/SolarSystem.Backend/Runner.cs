using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SolarSystem.Backend.Classes;
using SolarSystem.Backend.Classes.Schedulers;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.PickingAndErp;
using Article = SolarSystem.Backend.Classes.Simulation.Article;

namespace SolarSystem.Backend
{

    public class Runner
    {
        public readonly Handler Handler;
        public readonly OrderGenerator OrderGenerator;
        
        public readonly DateTime StartTime;
        private readonly DateTime _schedulerStartTime;

        private readonly double _simulationSpeed;
        private readonly Scheduler _scheduler;

        private readonly int _daysToSimulate;
     
        
        public Runner(string filePath, double simulationSpeed, double orderChance, 
            OrderGenerationConfiguration orderGenerationConfiguration, SchedulerType schedulerType, 
            int daysToSimulate, DateTime startTime, DateTime schedulerStartTime)
        {
            var pickNScrape = new PickingScrape(filePath + "Picking 02-10-2017.csv");
            pickNScrape.GetOrdersFromPicking();

            _simulationSpeed = simulationSpeed;
            _daysToSimulate = daysToSimulate;
            StartTime = startTime;
            _schedulerStartTime = schedulerStartTime;
            var orders = pickNScrape.OrderList;
            var erpscrape = new ErpScrape();
            erpscrape.ScrapeErp(filePath + "ErpTask_trace.log");

            erpscrape.Orders.Sort((x,y) => x.OrderTime.CompareTo(y.OrderTime));


            /*for (int i = 0; i < orders.Count; i++)
            {
                Order order = orders[i];

                try
                {
                    order.OrderTime = erpscrape.Orders.Find(x => x.OrderNumber == order.OrderNumber).OrderTime;
                }
                catch (NullReferenceException)
                {
                    orders.Remove(order);
                    i--;
                }
            }*/
            
            
           

            List<Article> articleList = orders
                .SelectMany(o => o.LineList)
                .Distinct()
                .Select(line => line.Article)
                .ToList();

            Handler = new Handler(); 
            
            SimulationInformation simInfo = new SimulationInformation(Handler, schedulerStartTime);
            
            OrderGenerator = new OrderGenerator(articleList, orderChance, orders, orderGenerationConfiguration);
            
            switch (schedulerType)
            {
                case SchedulerType.Fifo:
                    _scheduler = new FifoScheduler(OrderGenerator, Handler, 0);
                    break;
                case SchedulerType.Mi1:
                    throw new NotImplementedException("MI1 is not implemented yet..");
                    break;
                case SchedulerType.Mi6:
                    _scheduler = new Mi6Scheduler(OrderGenerator, Handler, 4, articleList, simInfo);
                    break;
                case SchedulerType.LST:
                    _scheduler = new LSTScheduer(OrderGenerator, Handler, 4);
                    break;
                case SchedulerType.Real:
                    _scheduler = new RealismScheduler(OrderGenerator, Handler, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(schedulerType), schedulerType, null);
            }
            
        }

        public void Start()
        {
            // Start Ticking
            var t = new Thread(() => TimeKeeper.StartTicking(_simulationSpeed, StartTime, _daysToSimulate));
            t.Start();

            TimeKeeper.Tick += MaybeStartScheduler;
            
            // Start OrderGeneration
            OrderGenerator.Start();
        }

        private void MaybeStartScheduler()
        {
            if (TimeKeeper.CurrentDateTime == _schedulerStartTime)
            {
                // Start Schedular
                _scheduler.Start();
            }
        }


        public IEnumerable<Area> Areas => Handler.Areas.Values;


    }

}