using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Castle.Windsor;
using Ploeh.AutoFixture;
using SolarSystem.Classes;
using SolarSystem.Simulation;


namespace SolarSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hi!");
            var timeKeeper = new TimeKeeper(DateTime.Now);
            timeKeeper.Tick += () => Console.WriteLine($"Tick: {timeKeeper.CurrentDateTime}");
            var order = OrderHandler.ConstructOrder();
            var handler = new Handler(timeKeeper);
            handler.MainLoop.OnOrderBoxInMainLoopFinished += (orderBox, code) =>
                Console.WriteLine($"MainLoop: OrderBoxFinished {orderBox}");
            handler.OnOrderBoxFinished += box => Console.WriteLine($"BOX FINISHED: {box}");

            Console.WriteLine("Start ticking");
            timeKeeper.StartTicking(13);

            Console.WriteLine("Listen for completed orders");
            handler.ReceiveOrder(order);
            
        }
    }
}