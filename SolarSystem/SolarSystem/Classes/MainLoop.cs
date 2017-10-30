using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net.Mail;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using C5;
using Microsoft.Win32;

namespace SolarSystem.Classes
{

    public class MainLoop
    {
        public delegate Func<OrderBox> OrderBoxSendHandler(OrderBox box);

        public event OrderBoxSendHandler OrderBoxIsFinishedEvent;
        private Dictionary<int, OrderboxProgressContainer> areaQueues;

        public MainLoop()
        {
            areaQueues = new Dictionary<int, OrderboxProgressContainer>();
            areaQueues.Add(21, new OrderboxProgressContainer());
            areaQueues.Add(25, new OrderboxProgressContainer());
            areaQueues.Add(27, new OrderboxProgressContainer());
            areaQueues.Add(28, new OrderboxProgressContainer());
            areaQueues.Add(28, new OrderboxProgressContainer());
            areaQueues.Add(29, new OrderboxProgressContainer());

            TimeKeeper.Tick += CheckAndSend;
        }

        public void AddOrderBox(OrderBox orderBox, int area)
        {
            var orderBoxProgress = PackToOrderboxProgress(orderBox, area);
            areaQueues[area].AddOrderBoxProgress(orderBoxProgress);
        }

        private OrderBoxProgress PackToOrderboxProgress(OrderBox orderBox, int area)
        {
            // Estimate time based on Loop Flow and areas
            int timeToSpend = EstimateTime(area);

            // Create new OrderBoxProgress based on orderbox and time.
            var orderBoxProgress =
                new OrderBoxProgress(orderBox, Program.TimeKeeper.CurrentDateTime, EstimateTime(area));

            // Return the new OrderBoxProgress.
            return orderBoxProgress;
        }

        private int EstimateTime(int area)
        {
            return 10;
        }

        private void CheckAndSend()
        {
            foreach (var areaQueue in areaQueues)
            {
                if (areaQueue.Value.GetNext().SecondsToSpend <= 0)
                {
                    OrderBoxIsFinishedEvent?.Invoke(areaQueue.Value.GetNext().OrderBox);
                }
            }
        }
    }
}