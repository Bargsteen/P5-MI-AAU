using System;
using System.Collections.Generic;
using System.Threading;
using SolarSystem.Backend.Classes;

namespace SolarSystem.Backend
{
    public class Runner
    {
        public readonly Handler Handler;
        
        public Runner(double simulationSpeed)
        {
            Handler = new Handler();
            
            
            
            //TimeKeeper.Tick += () => Console.WriteLine(TimeKeeper.CurrentDateTime);
            
            //Console.WriteLine("Start ticking");
            var t = new Thread(() => TimeKeeper.StartTicking(simulationSpeed, DateTime.Now));
            t.Start();
            //Console.WriteLine("Listen for completed orders");
           // Handler.ReceiveOrder(order);
            
        }
        
        public void StartSendingOrders()
        {
            var order = OrderHandler.ConstructOrder();
            Handler.ReceiveOrder(order);
        }

        public IEnumerable<Area> Areas => Handler.Areas.Values;


    }
}