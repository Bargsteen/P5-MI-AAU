using System;
using System.Collections.Generic;
using System.Threading;
using SolarSystem.Backend.Classes;

namespace SolarSystem.Backend
{
    public class Runner420
    {
        public readonly Handler Handler;
        
        public Runner420()
        {
            Handler = new Handler();
            
            
            
            //TimeKeeper.Tick += () => Console.WriteLine(TimeKeeper.CurrentDateTime);
            
            //Console.WriteLine("Start ticking");
            var t = new Thread(() => TimeKeeper.StartTicking(10, DateTime.Now));
            t.Start();
            //Console.WriteLine("Listen for completed orders");
           // Handler.ReceiveOrder(order);
            
        }

        public void StartSendingOrders()
        {
            var order = OrderHandler.ConstructOrder();
            Handler.ReceiveOrder(order);
        }

        public Dictionary<AreaCode, Area> Areas => Handler.Areas;


    }
}