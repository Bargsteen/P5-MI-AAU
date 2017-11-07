﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
{
    public class Handler
    {
        private const int PoolTimerMinutes = 15;
        
        public event Action<OrderBox> OnOrderBoxFinished;
        public readonly Dictionary<AreaCode, Area> Areas;
        public readonly MainLoop MainLoop;
        
        public Handler()
        {
            
            Areas = new Dictionary<AreaCode, Area>
            {
                {AreaCode.Area21, new Area(AreaCode.Area21)},
                {AreaCode.Area25, new Area(AreaCode.Area25)},
                {AreaCode.Area27, new Area(AreaCode.Area27)},
                {AreaCode.Area28, new Area(AreaCode.Area28)},
                {AreaCode.Area29, new Area(AreaCode.Area29)}
            };
            
            MainLoop = new MainLoop();

            foreach (var area in Areas.Values)
            {
                area.OnOrderBoxInAreaFinished += ReceiverOrderBoxFromArea;
            }
            MainLoop.OnOrderBoxInMainLoopFinished += ReceiveOrderBoxFromMainLoop;
        }

     
        
        public void ReceiveOrder(Order order)
        {
            // Convert Order to OrderBox
            OrderBox orderBox = new OrderBox(order);
            // Choose area to send to
            var nextArea = ChooseNextArea(orderBox);
            // Send to Main Loop
            SendToMainLoop(orderBox, nextArea);

        }

        private void SendToMainLoop(OrderBox orderBox, AreaCode areaCode)
        {
            // Call MainLoops ReceiveOrderBox with this input
            MainLoop.ReceiveOrderBoxAndArea(orderBox, areaCode);
        }

        private void SendToArea(OrderBox orderBox, AreaCode areaCode)
        {
            // If orderBox has been in areaCode already - throw exception
            if (orderBox.AreasVisited[areaCode])
            {
                throw new ArgumentException("Area has already been visited.");
            }
            // Send orderBox to area
            
            Areas[areaCode].ReceiveOrderBox(orderBox);
            
        }

        private void ReceiveOrderBoxFromMainLoop(OrderBox orderBox, AreaCode areaTo)
        {
            SendToArea(orderBox, areaTo);
        }

        private void ReceiverOrderBoxFromArea(OrderBox orderBox, AreaCode areaFrom)
        {
            // Update AreaVisited in orderBox
            orderBox.AreasVisited[areaFrom] = true;
            // Check if all areas has been visited
            if (orderBox.AreasVisited.All(a => a.Value))
            {
                OnOrderBoxFinished?.Invoke(orderBox);
                return;
            }
            // Choose Next Area
            var nextArea = ChooseNextArea(orderBox);
            // Send to MainLoop with area
            SendToMainLoop(orderBox, nextArea);
        }

        private AreaCode ChooseNextArea(OrderBox orderBox)
        {
            // Decide the next area to be visited.
            return orderBox.AreasVisited.First(a => !a.Value).Key;
        }
    }
}