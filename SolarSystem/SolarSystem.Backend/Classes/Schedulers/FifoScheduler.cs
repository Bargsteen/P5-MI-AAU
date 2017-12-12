using System.Linq;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public class FifoScheduler : Scheduler
    {
        public FifoScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(orderGenerator, handler, poolMoverTime)
        {
            
        }

        protected override Order ChooseNextOrder()
        {
            var ord = ActualOrderPool.OrderBy(o => o.OrderTime).First();
            if (ord.OrderId == 150350)
            {
                var b = 2;
            }
            if (ord.OrderId == 150402)
            {
                var slow = 3;
            }
            return ord;
        }
    }
}