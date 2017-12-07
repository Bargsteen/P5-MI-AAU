using System.Linq;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public class FifoScheduler : SchedulerModular
    {
        public FifoScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(orderGenerator, handler, poolMoverTime)
        {
            
        }

        public override Order ChooseNextOrder()
        {
            return ActualOrderPool.OrderBy(o => o.OrderTime).First();
        }
    }
}