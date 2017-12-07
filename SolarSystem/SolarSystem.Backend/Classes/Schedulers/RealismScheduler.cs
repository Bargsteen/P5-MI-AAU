using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Backend.Classes.Schedulers
{
    class RealismScheduler : SchedulerModular
    { 
        public override Order ChooseNextOrder()
        {
            return ActualOrderPool.OrderBy(o => o.OrderTime).First();
        }

        public RealismScheduler(OrderGenerator orderGenerator, Handler handler, double timerStartMinutes) : base(orderGenerator, handler, timerStartMinutes)
        {

        }
    }
}
