﻿using System;
using System.Globalization;
using System.Threading;
using Ploeh.AutoFixture;
using SolarSystem.Backend.Classes;


namespace SolarSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var order = OrderHandler.ConstructOrder();
            var handler = new Handler();
            handler.OnOrderBoxFinished += box => Console.WriteLine($"BOX FINISHED: {box}");
            
            TimeKeeper.Tick += () => Console.WriteLine(TimeKeeper.CurrentDateTime);
            
            Console.WriteLine("Start ticking");
            var t = new Thread(() => TimeKeeper.StartTicking(10, DateTime.Now));
            t.Start();
            Console.WriteLine("Listen for completed orders");
            handler.ReceiveOrder(order);
            
        }

        public event TickHandler OnTick;
        
      
        /*static void Main(string[] args) {
            //var t = new Thread(() => CallToChildThread(1000));
            Console.WriteLine("In Main: Creating the Child thread");
            t.Start();
            Console.ReadKey();
            
        }*/
    }
}