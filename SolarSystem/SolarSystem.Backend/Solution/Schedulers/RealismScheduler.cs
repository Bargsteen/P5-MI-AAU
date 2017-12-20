using System.Linq;
using SolarSystem.Backend.Solution.Simulation.Orders;
using SolarSystem.Backend.Solution.Simulation.Warehouse;

namespace SolarSystem.Backend.Solution.Schedulers
{
    internal class RealismScheduler : Scheduler
    {
        protected override Order ChooseNextOrder()
        {
            var ord = ActualOrderPool.OrderBy(o => o.OrderTime).First();
            return ord;
        }

        public RealismScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(
            orderGenerator, handler, poolMoverTime)
        {
        }
    }
}
