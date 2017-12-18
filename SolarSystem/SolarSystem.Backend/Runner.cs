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

        private readonly int _hoursToSimulate;

        private readonly int _runsToDo;
        
        public Runner(string filePath, double simulationSpeed, double orderChance, 
            OrderGenerationConfiguration orderGenerationConfiguration, SchedulerType schedulerType, 
            int hoursToSimulate, DateTime startTime, DateTime schedulerStartTime, List<PickingOrder> orderList, int runsToDo, bool useOrderTime)
        {
            
    
            _simulationSpeed = simulationSpeed;
            _hoursToSimulate = hoursToSimulate;
            StartTime = startTime;
            _schedulerStartTime = schedulerStartTime;
            var orders = orderList;
            var erpscrape = new ErpScrape();
            erpscrape.ScrapeErp(filePath + "ErpTask_trace.log");
            _runsToDo = runsToDo;

            erpscrape.Orders.Sort((x,y) => x.OrderTime.CompareTo(y.OrderTime));
            orders.Sort((x,y) => x.OrderTime.CompareTo(y.OrderTime));

            List<Article> articleList = orders
                .SelectMany(o => o.LineList)
                .Distinct()
                .Select(line => line.Article)
                .ToList();

            //var ordersIntersect = erpscrape.Orders.Where(o => orders.Any(e => e.OrderNumber == o.OrderNumber)).Distinct().OrderBy(o => o.OrderNumber).ToList();

            var ordersOrderedAndPicked =
                orders.Where(o => erpscrape.Orders.Any(e => e.OrderNumber == o.OrderNumber)).Distinct().ToList();

            if (useOrderTime)
            {
                // Update orders to use the time when they were ordered rather than picked.
                if (schedulerType == SchedulerType.Real)
                {
                    throw new ArgumentException("The real scheduler should only be used with userOrderTime = false. As it depends on the picking time.");
                }
                ordersOrderedAndPicked.ForEach(o => o.OrderTime = erpscrape.Orders.First(e => e.OrderNumber == o.OrderNumber).OrderTime);
            }
            
            Handler = new Handler(); 
            
            SimulationInformation simInfo = new SimulationInformation(Handler, schedulerStartTime);
            
            OrderGenerator = new OrderGenerator(articleList, orderChance, ordersOrderedAndPicked, orderGenerationConfiguration);
            
            switch (schedulerType)
            {
                case SchedulerType.Fifo:
                    _scheduler = new FifoScheduler(OrderGenerator, Handler, SimulationConfiguration.GetSchedulerPoolMoveTime());
                    break;
                case SchedulerType.Mi1:
                    throw new NotImplementedException("MI1 is not implemented yet..");
                    break;
                case SchedulerType.Mi6:
                    _scheduler = new Mi6Scheduler(OrderGenerator, Handler, SimulationConfiguration.GetSchedulerPoolMoveTime(), articleList, simInfo);
                    break;
                case SchedulerType.LST:
                    _scheduler = new LstScheduer(OrderGenerator, Handler, SimulationConfiguration.GetSchedulerPoolMoveTime());
                    break;
                case SchedulerType.Real:
                    _scheduler = new RealismScheduler(OrderGenerator, Handler, 0);
                    break;
                case SchedulerType.Regression:
                    _scheduler = new RegressionScheduler(OrderGenerator, Handler, 4);
                break;
                case SchedulerType.Estimator:
                    _scheduler = new EstimatorScheduler(OrderGenerator, Handler, SimulationConfiguration.GetSchedulerPoolMoveTime());
                break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(schedulerType), schedulerType, null);
            }
            
        }

        public void Start()
        {
            // Start Ticking
            var t = new Thread(() => TimeKeeper.StartTicking(_simulationSpeed, StartTime, _hoursToSimulate, _runsToDo));
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