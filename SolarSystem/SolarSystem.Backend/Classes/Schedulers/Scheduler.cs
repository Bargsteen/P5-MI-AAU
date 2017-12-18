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

        protected event Action<Order> OnOrderActuallySent;
        
        // PickingOrder pool from costumers
        protected List<Order> InitialOrderPool { get; }
        
        // The pool that is actively being moved to Handler
        protected List<Order> ActualOrderPool { get; }

        private OrderGenerator OrderGenerator { get; }
        private Handler Handler { get; }

        public bool UsePoolTime = true;

        protected Scheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime)
        {
            OrderGenerator = orderGenerator;
            Handler = handler;
            InitialOrderPool = new List<Order>();
            ActualOrderPool = new List<Order>();;

            PoolTimer = 0;
            TimerStartMinutes = poolMoverTime * 60;

            OrderGenerator.CostumerSendsOrderEvent += order => InitialOrderPool.Add(order);
            
        }

        public virtual void Start()
        {
            TimeKeeper.Tick += TickLoop;
        }

        private void TickLoop()
        {
            RunFirstInTickLoop();
            
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
                    int a;
                    if (action.OrderId == 150350)
                        a = 2;
                }
            }
            
            // Else => Return
        }

        protected virtual void RunFirstInTickLoop()
        {
            // Do nothing
        }

        protected virtual void MoveInitialToActualPool()
        {
            // If the time has passed, and there is something to move => move.
            if (UsePoolTime == false || TimerStartMinutes <= PoolTimer++)
            {
                
                DoWhenMoveInitialToActualPool();
                
                ActualOrderPool.AddRange(InitialOrderPool);
                InitialOrderPool.Clear();
                

                // Reset timer
                PoolTimer = 0;
            }
            
            // Else return.
        }

        protected virtual void DoWhenMoveInitialToActualPool()
        {
            //
        }

        protected abstract Order ChooseNextOrder();

        private bool SendActionToHandler(Order action)
        {
            // Check if the action is the wait action  && Handler isn't full
            if (action.OrderId != 0 && !Handler.HandlerIsFull)
            {
                // True => Send
                Handler.ReceiveOrder(action);
                
                // Used for letting deriving schedulers know whether it was actually sent
                OnOrderActuallySent?.Invoke(action);
                
                // Return action is valid => True
                return true;
            }
            
            // Else action is invalid => False
            return false;

        }
            
    }
}