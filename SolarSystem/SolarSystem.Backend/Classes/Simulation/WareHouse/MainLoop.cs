using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation.Boxes;
using SolarSystem.Backend.Classes.Simulation.ConstantsAndEnums;
using SolarSystem.Backend.Classes.Simulation.OrderBoxHandlers;

namespace SolarSystem.Backend.Classes.Simulation.WareHouse
{

    public class MainLoop
    {
        public event Action<OrderBox, AreaCode> OnOrderBoxInMainLoopFinished;
        private readonly Dictionary<AreaCode, OrderBoxProgressContainer> _areaQueues;

        public int BoxesInMainLoop => _areaQueues.Values.SelectMany(opc => opc.ToList()).Count();
        
        public MainLoop()
        {
            _areaQueues = new Dictionary<AreaCode, OrderBoxProgressContainer>();
            _areaQueues.Add(AreaCode.Area21, new OrderBoxProgressContainer());
            _areaQueues.Add(AreaCode.Area25, new OrderBoxProgressContainer());
            _areaQueues.Add(AreaCode.Area27, new OrderBoxProgressContainer());
            _areaQueues.Add(AreaCode.Area28, new OrderBoxProgressContainer());
            _areaQueues.Add(AreaCode.Area29, new OrderBoxProgressContainer());

            TimeKeeper.Tick += _CheckAndSend;
        }

        public void ReceiveOrderBoxAndArea(OrderBox orderBox, AreaCode areaCode)
        {
            var orderBoxProgress = PackToOrderboxProgress(orderBox, areaCode);
            _areaQueues[areaCode].AddOrderBoxProgress(orderBoxProgress);
        }

        private OrderBoxProgress PackToOrderboxProgress(OrderBox orderBox, AreaCode area)
        {
            // Estimate time based on Loop Flow and areas
            int timeToSpend = EstimateTimeInSeconds(area);

            // Create new OrderBoxProgress based on orderbox and time.
            var orderBoxProgress =
                new OrderBoxProgress(orderBox, EstimateTimeInSeconds(area));

            // Return the new OrderBoxProgress.
            return orderBoxProgress;
        }

        private int EstimateTimeInSeconds(AreaCode area)
        {
            return SimulationConfiguration.GetTimeInMainLoop();
        }

        public void _CheckAndSend()
        {
            foreach (var areaQueue in _areaQueues)
            {
                if (areaQueue.Value.GetNext()?.SecondsToSpend <= 0)
                {
                    OnOrderBoxInMainLoopFinished?.Invoke(areaQueue.Value.Pop()?.OrderBox, areaQueue.Key);
                }
            }
        }
    }
}