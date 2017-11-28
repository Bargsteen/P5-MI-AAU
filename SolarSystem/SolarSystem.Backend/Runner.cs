using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SolarSystem.Backend.Classes;
using SolarSystem.PickingAndErp;

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

            List<Article> articleList = orders
                .SelectMany(o => o.LineList)
                .Distinct()
                .Select(line => line.Article)
                .ToList();
            
            Handler = new Handler(); 
            
            OrderGenerator = new OrderGenerator(articleList, orderChance);
            
            StartTime = DateTime.Now;
            
            var t = new Thread(() => TimeKeeper.StartTicking(simulationSpeed, StartTime));
            t.Start();
            
            
            SimulationInformation = new SimulationInformation(Handler);
            
            //Scheduler = new Scheduler(OrderGenerator, Handler, 0.0001);
            MiScheduler = new MiScheduler(5, articleList.ToArray(), SimulationInformation, OrderGenerator, Handler);
        }


        public IEnumerable<Area> Areas => Handler.Areas.Values;


    }

}