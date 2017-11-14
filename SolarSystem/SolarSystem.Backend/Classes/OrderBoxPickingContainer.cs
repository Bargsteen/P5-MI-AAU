using System;

namespace SolarSystem.Backend.Classes
{
    public class OrderBoxPickingContainer
    {
        private int _timeLeftOfPicking;
        private readonly OrderBox _orderBox;
        private readonly Line _lineBeingPicked;

        public event Action<OrderBox> OnLinePickedForOrderBox;
        
        public OrderBoxPickingContainer(OrderBox orderBox, Line lineBeingPicked)
        {
            _orderBox = orderBox ?? throw new ArgumentNullException(nameof(orderBox));
            _lineBeingPicked = lineBeingPicked ?? throw new ArgumentNullException(nameof(lineBeingPicked));

            // Check if lineBeingPicked is in orderBox. Should ALWAYS be the case
            if (!orderBox.LineIsPickedStatuses.ContainsKey(lineBeingPicked))
            {
                throw new ArgumentException($"{nameof(lineBeingPicked)} is not in {nameof(orderBox)}");
            }
            // Calculate the time it needs to actually this line. At least one.
            _timeLeftOfPicking = Math.Max(1, (int) (lineBeingPicked.Quantity * GlobalConstants.TimePerArticlePick));

            TimeKeeper.Tick += DecrementAndMaybeInvoke;
        }

        private void DecrementAndMaybeInvoke()
        {
            // Decrement the time left
            _timeLeftOfPicking -= 1;
            if (_timeLeftOfPicking <= 0)
            {
                // When time has reached zero, we update the status of the line in orderbox
                _orderBox.LineIsPickedStatuses[_lineBeingPicked] = true;
                
                // Invoke event telling that this line has been picked
                OnLinePickedForOrderBox?.Invoke(_orderBox);

                if (OnLinePickedForOrderBox != null)
                {
                    foreach (var d in OnLinePickedForOrderBox?.GetInvocationList())
                    {
                        OnLinePickedForOrderBox -= (Action<OrderBox>) d;
                    }
                }
                    
            }
        }
    }
}