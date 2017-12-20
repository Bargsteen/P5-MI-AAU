using System.Linq;
using SolarSystem.Backend.Solution.Simulation.Orders;
using SolarSystem.Backend.Solution.Simulation.Warehouse;

namespace SolarSystem.Backend.Solution.Schedulers
{
    public class FifoScheduler : Scheduler
    {
        public FifoScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(orderGenerator, handler, poolMoverTime)
        {
            
        }

        protected override Order ChooseNextOrder()
        {
            return ActualOrderPool.OrderBy(o => o.OrderTime).First();
        }
    }
}