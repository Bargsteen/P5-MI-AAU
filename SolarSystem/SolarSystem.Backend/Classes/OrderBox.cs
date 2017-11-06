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
            AreasVisited = ConstructAreasVisited(order);
        }

        private Dictionary<AreaCode, bool> ConstructAreasVisited(Order order)
        {
            // Dictionary to be returned.
            var returnAreas = new Dictionary<AreaCode, bool>();

            // Iterate through all lines and add to dictionary
            foreach (var line in order.Lines)
            {
                returnAreas.Add(line.Article.AreaCode, false);
            }
            
            // Sort according to the real flow
            // TODO: Sorting of dictionary needs do!!!
            
            // Return Dictionary
            return returnAreas;

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
