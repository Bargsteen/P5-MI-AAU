using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using SolarSystem.Backend.Classes;
using SolarSystem.Backend.Classes.Schedulers;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.PickingAndErp;

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
            
            const string s = "2017-10-02 08:00";
            StartTime = DateTime.ParseExact(s, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            
            var t = new Thread(() => TimeKeeper.StartTicking(simulationSpeed, StartTime));
            t.Start();
                       
            //Scheduler = new Scheduler(OrderGenerator, Handler, 0.0001);
            //MiScheduler = new MiScheduler(5, articleList.ToArray(), SimulationInformation, OrderGenerator, Handler);
            FifoScheduler fifoScheduler = new FifoScheduler(OrderGenerator, Handler, 4);
            fifoScheduler.Start();
        }


        public IEnumerable<Area> Areas => Handler.Areas.Values;


    }

}