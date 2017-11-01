using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Interfaces;

namespace SolarSystem.Backend.Classes
{
    public class Handler
    {
        public event Action<OrderBox> OnOrderBoxFinished;
        private ITimeKeeper _timeKeeper;
        private Dictionary<AreaCode, Area> _areas;
        public MainLoop MainLoop;

        public Handler(ITimeKeeper timeKeeper)
        {
            _timeKeeper = timeKeeper ?? throw new ArgumentNullException(nameof(timeKeeper));
            _areas = new Dictionary<AreaCode, Area>
            {
                {AreaCode.Area21, new Area(AreaCode.Area21, timeKeeper)},
                {AreaCode.Area25, new Area(AreaCode.Area25, timeKeeper)},
                {AreaCode.Area27, new Area(AreaCode.Area27, timeKeeper)},
                {AreaCode.Area28, new Area(AreaCode.Area28, timeKeeper)},
                {AreaCode.Area29, new Area(AreaCode.Area29, timeKeeper)}
            };
            
            MainLoop = new MainLoop(_timeKeeper);

            foreach (var area in _areas.Values)
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
            var nextArea = ChooseNextArea(orderBox, orderBox.StartAreaCode, true);
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
            _areas[areaCode].ReceiveOrderBox(orderBox);
            
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
            var nextArea = ChooseNextArea(orderBox, areaFrom);
            // Send to MainLoop with area
            SendToMainLoop(orderBox, nextArea);
        }

        private AreaCode ChooseNextArea(OrderBox orderBox, AreaCode areaFrom, bool initial = false)
        {
            // If the box is from outside decide next area
            if (initial)
            {
                return orderBox.StartAreaCode;
            }
            // If the box comes from an area, decide next area
            return orderBox.AreasVisited.First(a => a.Value).Key;
        }
    }
}