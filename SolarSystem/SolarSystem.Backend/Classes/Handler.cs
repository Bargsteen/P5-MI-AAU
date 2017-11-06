using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
{
    public class Handler
    {
        public event Action<OrderBox> OnOrderBoxFinished;
        public readonly Dictionary<AreaCode, Area> Areas;
        public MainLoop MainLoop;

        private List<Order> OrderPool { get; set; }

        private OrderGenerator OrderGenerator { get; }            
        
        public Handler(OrderGenerator orderGenerator)
        {
            OrderPool = new List<Order>();

            OrderGenerator = orderGenerator ?? throw new ArgumentNullException(nameof(orderGenerator));
            
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
            OrderGenerator.CostumerSendsOrderEvent += AddOrderToPool;

        }

        private void AddOrderToPool(Order order)
        {
            OrderPool.Add(order);
            // Added Order To Handler
        }
        
        public void ReceiveOrder(Order order)
        {
            Console.WriteLine("Handler: Getting new order");   
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
            Console.WriteLine("Handler: Sending to mainloop");
            MainLoop.ReceiveOrderBoxAndArea(orderBox, areaCode);
        }

        private void SendToArea(OrderBox orderBox, AreaCode areaCode)
        {
            // If orderBox has been in areaCode already - throw exception
            Console.WriteLine($"Handler: Sending to area: {areaCode}");
            if (orderBox.AreasVisited[areaCode])
            {
                throw new ArgumentException("Area has already been visited.");
            }
            // Send orderBox to area
            
            Areas[areaCode].ReceiveOrderBox(orderBox);
            
        }

        private void ReceiveOrderBoxFromMainLoop(OrderBox orderBox, AreaCode areaTo)
        {
            Console.WriteLine("Handler: Received new order from MainLoop");
            SendToArea(orderBox, areaTo);
        }

        private void ReceiverOrderBoxFromArea(OrderBox orderBox, AreaCode areaFrom)
        {
            Console.WriteLine($"Handler: Received orderbox from {areaFrom}");
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
            return orderBox.AreasVisited.First(a => !a.Value).Key;
        }
    }
}