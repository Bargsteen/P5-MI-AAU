using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
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