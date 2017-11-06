using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
{
    public class OrderBox : Box
    {
        public Order Order { get; }
        private readonly List<Line> _pickedLines;
        public IEnumerable<Line> PickedLines => _pickedLines.AsReadOnly();
        public int TimeRemaining { get; set; }
        public Dictionary<AreaCode, bool> AreasVisited { get; }
        
        
        public OrderBox(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            _pickedLines = new List<Line>();
            AreasVisited = order.Areas;
        }
        
        public BoxResult PutLineIntoBox(Line line)
        {

            if (!Order.Lines.Contains(line)) return BoxResult.NotInOrder;
            if (PickedLines.Contains(line)) return BoxResult.AlreadyPicked;
            
            _pickedLines.Add(line);
            return BoxResult.SuccesfullyAdded;
        }

        public List<Line> LinesNotPicked()
        {
            return Order.Lines.Except(PickedLines).ToList();
        }

        public override string ToString()
        {
            return $"({Order}, {TimeRemaining})";
        }
    }
}
