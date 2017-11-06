﻿using System;
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
            
            //TimeKeeper.Tick += () => Console.WriteLine(TimeKeeper.CurrentDateTime);
            
            //Console.WriteLine("Start ticking");
            var t = new Thread(() => TimeKeeper.StartTicking(simulationSpeed, DateTime.Now));
            t.Start();
            //Console.WriteLine("Listen for completed orders");
           // Handler.ReceiveOrder(order);
            
            Handler = new Handler(OrderGenerator);
        }
        
        public IEnumerable<Area> Areas => Handler.Areas.Values;


    }

}