using System;
using System.Collections.Generic;

namespace SolarSystem.Backend.Classes
{

    public class MainLoop
    {
        public event Action<OrderBox, AreaCode> OnOrderBoxInMainLoopFinished;
        private readonly Dictionary<AreaCode, OrderboxProgressContainer> areaQueues;

        public int BoxesInMainLoop => areaQueues.Count;
        
        public MainLoop()
        {
            areaQueues = new Dictionary<AreaCode, OrderboxProgressContainer>();
            areaQueues.Add(AreaCode.Area21, new OrderboxProgressContainer());
            areaQueues.Add(AreaCode.Area25, new OrderboxProgressContainer());
            areaQueues.Add(AreaCode.Area27, new OrderboxProgressContainer());
            areaQueues.Add(AreaCode.Area28, new OrderboxProgressContainer());
            areaQueues.Add(AreaCode.Area29, new OrderboxProgressContainer());

            TimeKeeper.Tick += _CheckAndSend;
        }

        public void ReceiveOrderBoxAndArea(OrderBox orderBox, AreaCode areaCode)
        {
            var orderBoxProgress = PackToOrderboxProgress(orderBox, areaCode);
            areaQueues[areaCode].AddOrderBoxProgress(orderBoxProgress);
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
            return 120;
        }

        public void _CheckAndSend()
        {
            foreach (var areaQueue in areaQueues)
            {
                if (areaQueue.Value.GetNext()?.SecondsToSpend <= 0)
                {
                    OnOrderBoxInMainLoopFinished?.Invoke(areaQueue.Value.Pop()?.OrderBox, areaQueue.Key);
                }
            }
        }
    }
}