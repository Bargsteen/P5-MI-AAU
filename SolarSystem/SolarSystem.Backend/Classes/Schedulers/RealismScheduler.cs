using System.Linq;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    internal class RealismScheduler : Scheduler
    {
        protected override Order ChooseNextOrder()
        {
            return ActualOrderPool.OrderBy(o => o.OrderTime).First();
        }

        public RealismScheduler(OrderGenerator orderGenerator, Handler handler, double timerStartMinutes) : base(orderGenerator, handler, timerStartMinutes)
        {

        }
    }
}
