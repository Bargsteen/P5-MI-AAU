﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace SolarSystem.Backend.Classes
{
    public class Scheduler
    {
        private double TimerStartMinutes { get; }
        private int PoolTimer { get; set; }
        private static readonly Random Rand = new Random();
        
        // Order pool from costumers
        private List<Order> InitialOrderPool { get; }
        
        // The pool that is actively being moved to Handler
        private List<Order> ActualOrderPool { get; }

        private OrderGenerator OrderGenerator { get; }
        public Handler Handler { get; set; }
        
        public Scheduler(OrderGenerator orderGenerator, Handler handler, double timerStartMinutes)
        {
            OrderGenerator = orderGenerator;
            Handler = handler;
            TimerStartMinutes = timerStartMinutes;

            InitialOrderPool = new List<Order>();
            ActualOrderPool = new List<Order>();
            
            StartTimer();

            OrderGenerator.CostumerSendsOrderEvent += AddOrderToPool;
            TimeKeeper.Tick += PoolTimeChecker;
        }

        private void AddOrderToPool(Order order)
        {
            InitialOrderPool.Add(order);
        }


        void StartTimer() => ResetTimer();
        
        void ResetTimer()
        {
            PoolTimer = (int) TimerStartMinutes * 60;
        }

        Order OrderSelectorFromActualPool()
        {
            // Select random order
            var returnOrder = ActualOrderPool[Rand.Next(0, ActualOrderPool.Count - 1)] ?? throw new IndexOutOfRangeException();
            
            // Remove it from transfer pool
            ActualOrderPool.Remove(returnOrder);

            // Return it
            return returnOrder;
        }

        void OrderToHandlerMover()
        {
            // If system/handler is full then return
            if (Handler.HandlerIsFull) return;
            
            // Select next order for pushing to handler
            var nextOrder = OrderSelectorFromActualPool();
            
            // Send to handler
            Handler.ReceiveOrder(nextOrder);
        }
        
        // Check every tick if the timer hits 0
        void PoolTimeChecker()
        {
            // Check if timer is <= 0
            if (PoolTimer <= 0)
            {
                // Move current pool to actualPool
                InitialOrderPool.ForEach(o => ActualOrderPool.Add(o));
                InitialOrderPool.Clear();
                
                ResetTimer();
            }
            else
            {
                // Decrement timer
                PoolTimer--;
            }
            
            // If there is something in the current pool
            if (ActualOrderPool.Any())
            {
                // Push it to the handler one element per tick
                OrderToHandlerMover();
            }
        }
        
    }
}