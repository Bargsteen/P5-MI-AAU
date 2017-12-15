using System.Linq;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    internal class RealismScheduler : Scheduler
    {
        protected override Order ChooseNextOrder()
        {
            var ord = ActualOrderPool.OrderBy(o => o.OrderTime).First();

            if (ord.OrderId == 150350)
            {
                var b = "lol";
            }
            return ord;
        }

        public RealismScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(
            orderGenerator, handler, poolMoverTime)
        {
        }
    }
}
