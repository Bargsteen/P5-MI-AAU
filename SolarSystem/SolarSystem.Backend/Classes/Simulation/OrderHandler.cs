using System.Collections.Generic;
using Ploeh.AutoFixture;

namespace SolarSystem.Backend.Classes.Simulation
{
    public static class OrderHandler
    {
        public static Order ConstructOrder()
        {
            var fixture = new Fixture();
            const AreaCode startArea = AreaCode.Area21;
            var areas = new Dictionary<AreaCode, bool> {{startArea, false}, {AreaCode.Area25, false}, {AreaCode.Area27, false}, {AreaCode.Area28, false}};

            var order = fixture.Build<Order>().With(o => o.Areas, areas).Create();

            return order;
        }
            
    }
}