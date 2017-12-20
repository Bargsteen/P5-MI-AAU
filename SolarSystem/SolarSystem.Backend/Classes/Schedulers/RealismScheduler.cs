using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.Classes.Simulation.Orders;
using SolarSystem.Backend.Classes.Simulation.WareHouse;

namespace SolarSystem.Backend.Classes.Schedulers
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
