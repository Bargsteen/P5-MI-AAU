﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
            
            
            OrderGenerator = new OrderGenerator(articleList, orderChance);
            
            const string s = "2017-10-02 08:00";
            StartTime = DateTime.ParseExact(s, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            
            var t = new Thread(() => TimeKeeper.StartTicking(simulationSpeed, StartTime));
            t.Start();
            
            Handler = new Handler();
            
            Scheduler = new Scheduler(OrderGenerator, Handler, 0.0001);
        }


        public IEnumerable<Area> Areas => Handler.Areas.Values;


    }

}