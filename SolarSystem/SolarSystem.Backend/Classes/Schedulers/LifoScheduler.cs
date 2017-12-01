using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public class LifoScheduler : SchedulerModular
    {
        public LifoScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(orderGenerator, handler, poolMoverTime)
        {
        }

        public override Order ChooseNextOrder()
        {
            return ActualOrderPool.OrderBy(o => o.OrderTime).Last();
        }
    }
}
