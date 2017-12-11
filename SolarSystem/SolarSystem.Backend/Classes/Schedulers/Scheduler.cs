using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public abstract class Scheduler
    {
        protected double TimerStartMinutes { get; }
        protected int PoolTimer { get; set; }

        protected SimulationInformation SimulationInformation { get; }
        
        // Order pool from costumers
        protected List<Order> InitialOrderPool { get; }
        
        // The pool that is actively being moved to Handler
        protected List<Order> ActualOrderPool { get; }

        private OrderGenerator OrderGenerator { get; }
        private Handler Handler { get; set; }

        protected Scheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime)
        {
            OrderGenerator = orderGenerator;
            Handler = handler;
            SimulationInformation = new SimulationInformation(Handler);
            InitialOrderPool = new List<Order>();
            ActualOrderPool = new List<Order>();;

            PoolTimer = 0;
            TimerStartMinutes = poolMoverTime * 60;

            OrderGenerator.CostumerSendsOrderEvent += order => InitialOrderPool.Add(order);
            
        }

        public void Start()
        {
            TimeKeeper.Tick += TickLoop;
        }

        private void TickLoop()
        {
            // Move OrderPool
            MoveInitialToActualPool();
            
            // Choose the next action if ActualPool != Empty
            if (ActualOrderPool.Any())
            {
                var action = ChooseNextOrder();

                // Send action to handler
                var actionSuccess = SendActionToHandler(action);
                
                // If success => Remove
                if (actionSuccess)
                {
                    ActualOrderPool.Remove(action);
                }
            }
            
            // Else => Return
        } 
        
        protected virtual void MoveInitialToActualPool()
        {
            // If the time has passed, and there is something to move => move.
            if (TimerStartMinutes <= PoolTimer++)
            {
                ActualOrderPool.AddRange(InitialOrderPool);
                InitialOrderPool.Clear();
                
                // Reset timer
                PoolTimer = 0;
            }
            
            // Else return.
        }

        protected abstract Order ChooseNextOrder();

        private bool SendActionToHandler(Order action)
        {
            // Check if the action is the wait action  && Handler isn't full
            if (action.OrderId != 0 && !Handler.HandlerIsFull)
            {
                // True => Send
                Handler.ReceiveOrder(action);  
                
                // Return action is valid => True
                return true;
            }
            
            // Else action is invalid => False
            return false;

        }
            
    }
}