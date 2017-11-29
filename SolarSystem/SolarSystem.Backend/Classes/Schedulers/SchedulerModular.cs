using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
{
    public abstract class SchedulerModular
    {
        private double TimerStartMinutes { get; }
        private int PoolTimer { get; set; }
        
        // Order pool from costumers
        private List<Order> InitialOrderPool { get; }
        
        // The pool that is actively being moved to Handler
        public List<Order> ActualOrderPool { get; }

        private OrderGenerator OrderGenerator { get; }
        private Handler Handler { get; set; }
        
        public SchedulerModular(OrderGenerator orderGenerator, Handler handler, double poolMoverTime)
        {
            OrderGenerator = orderGenerator;
            Handler = handler;
            InitialOrderPool = new List<Order>();
            ActualOrderPool = new List<Order>();;

            PoolTimer = 0;
            TimerStartMinutes = poolMoverTime * 60;

            OrderGenerator.CostumerSendsOrderEvent += order => InitialOrderPool.Add(order);
            TimeKeeper.Tick += TickLoop;
        }

        public void TickLoop()
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
        
        private void MoveInitialToActualPool()
        {
            // If the time has passed, and there is something to move => move.
            if (TimerStartMinutes <= PoolTimer++)
            {
                InitialOrderPool.ForEach(o => ActualOrderPool.Add(o));
                InitialOrderPool.Clear();
                
                // Reset timer
                PoolTimer = 0;
            }
            
            // Else return.
        }

        public abstract Order ChooseNextOrder();

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