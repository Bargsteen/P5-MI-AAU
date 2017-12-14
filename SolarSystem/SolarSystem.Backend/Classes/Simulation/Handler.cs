using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes.Simulation
{
    public class Handler
    {
        private const int PoolTimerMinutes = 15;
        
        public event Action<OrderBox> OnOrderBoxFinished;
        public readonly Dictionary<AreaCode, Area> Areas;
        public readonly MainLoop MainLoop;

        public bool HandlerIsFull => Areas.Values.All(a => a.AreaIsFull);
        
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
            if (HandlerIsFull)
            {
                throw new AccessViolationException("Handler received order when it is full!");
            }
            // Convert PickingOrder to OrderBox
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
            // If Area is full, then choose new area and give it another turn in MainLoop
           
            if (Areas[areaTo].AreaIsFull)
            {
                var newArea = ChooseNextArea(orderBox);
                SendToMainLoop(orderBox, newArea);
            }
            else
            {
                // Else send to the area
                SendToArea(orderBox, areaTo);
            }
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
            // Find areaCode of first needed area which is not full
            if (orderBox.AreasVisited.Any(a => !a.Value && !Areas[a.Key].AreaIsFull))
            {
                return orderBox.AreasVisited.First(a => !a.Value && !Areas[a.Key].AreaIsFull).Key;
            }
            // If all needed areas are full -> just choose a needed area and take a chill on the MainLoop
            return orderBox.AreasVisited.First(a => !a.Value).Key;
        }
    }
}