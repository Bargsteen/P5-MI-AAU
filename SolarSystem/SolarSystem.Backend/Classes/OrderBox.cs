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
        public Dictionary<AreaCode, bool> AreasVisited { get; }
        
        
        public OrderBox(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
<<<<<<< HEAD
            LineIsPickedStatuses = Order.Lines.ToDictionary(key => key, value => false);
=======
            _pickedLines = new List<Line>();
            AreasVisited = order.Areas;
>>>>>>> master
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
