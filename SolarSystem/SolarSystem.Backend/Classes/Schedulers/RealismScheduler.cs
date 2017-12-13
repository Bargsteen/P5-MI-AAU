using System;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    internal class RealismScheduler : Scheduler
    {
        protected override Order ChooseNextOrder()
        {
            var ord = ActualOrderPool.OrderBy(o => o.OrderTime).First();

            if (Math.Abs((TimeKeeper.CurrentDateTime - ord.OrderTime).TotalSeconds) > 3)
            {
                var b = 2;
            }
            if (ord.OrderId == 150402)
            {
                var slow = 3;
            }


            if (Math.Abs((TimeKeeper.CurrentDateTime - ord.OrderTime).TotalSeconds) > 3)
            {
                throw new Exception();
            }

            return ord;



            return ord;
        }

        public override void Start()
        {
            base.Start();
            UsePoolTime = true;
        }

        public RealismScheduler(OrderGenerator orderGenerator, Handler handler, double timerStartMinutes) : base(orderGenerator, handler, timerStartMinutes)
        {

        }
    }
}
