using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
{
    public class OrderBox : Box
    {
        public Order Order { get; }
        
        public readonly Dictionary<Line, bool> LineIsPickedStatuses; // True == isPicked
        public int TimeRemaining { get; set; }
        public Dictionary<AreaCode, bool> AreasVisited => Order.Areas;
        public AreaCode StartAreaCode => Order.StartAreaCode;

        public OrderBox(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            LineIsPickedStatuses = Order.Lines.ToDictionary(key => key, value => false);
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

        public override string ToString()
        {
            return $"({Order}, {TimeRemaining})";
        }
    }
}
