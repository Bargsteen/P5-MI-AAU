using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation.ConstantsAndEnums;
using SolarSystem.Backend.Classes.Simulation.Orders;

namespace SolarSystem.Backend.Classes.Simulation.Boxes
{
    public class OrderBox : Box
    {
        public Order Order { get; }
        
        public readonly Dictionary<Line, bool> LineIsPickedStatuses; // True == isPicked
        public Dictionary<AreaCode, bool> AreasVisited { get; }

        private readonly DateTime _timeCreated;

        public TimeSpan TimeInSystem => TimeKeeper.CurrentDateTime - _timeCreated;
        
        public OrderBox(Order order)
        {
            _timeCreated = TimeKeeper.CurrentDateTime;
            
            Order = order ?? throw new ArgumentNullException(nameof(order));
            LineIsPickedStatuses = Order.Lines.ToDictionary(key => key, value => false);
            AreasVisited = order.Areas;

        }
        
        public BoxResult PutLineIntoBox(Line line)
        {

            if (!Order.Lines.Contains(line)) return BoxResult.NotInOrder;
            if (LineIsPickedStatuses[line]) return BoxResult.AlreadyPicked;

            LineIsPickedStatuses[line] = true;
            
            return BoxResult.SuccesfullyAdded;
        }

        public List<Line> LinesNotPicked()
        {
            return LineIsPickedStatuses
                .Where(l => l.Value == false)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public List<Line> LinesNotPickedIn(AreaCode inArea)
        {
            return LineIsPickedStatuses
                .Where(l => l.Value == false)
                .Select(kvp => kvp.Key)
                .Where(l => l.Article.AreaCode == inArea)
                .ToList();
        }

        public override string ToString()
        {
            return $"(OrderBox: {Id})";
        }
    }
}
