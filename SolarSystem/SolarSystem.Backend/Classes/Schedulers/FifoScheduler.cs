using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.Classes.Simulation.Orders;
using SolarSystem.Backend.Classes.Simulation.WareHouse;

namespace SolarSystem.Backend.Classes.Schedulers
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