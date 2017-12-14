using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    class EstimatorScheduler : Scheduler
    {
        private const int secondsLookAhead = 20000;
        private Dictionary<AreaCode, double>[] AreaFillInfo;
           
        public EstimatorScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(orderGenerator, handler, poolMoverTime)
        {
            OnOrderActuallySent += UpdateFillEstimateAndSetOrderTime;
            var zeroAreaFillDict = new Dictionary<AreaCode, double>
            {
                {AreaCode.Area21, 0},
                {AreaCode.Area25, 0},
                {AreaCode.Area27, 0},
                {AreaCode.Area28, 0},
                {AreaCode.Area29, 0}
            };
            AreaFillInfo = Enumerable.Repeat(zeroAreaFillDict, secondsLookAhead).ToArray();

        }

        protected override Order ChooseNextOrder()
        {
            throw new NotImplementedException();
        }

        private void UpdateFillEstimateAndSetOrderTime(Order order)
        {
            // Set the startPackingTime for order
            order.StartPackingTime = TimeKeeper.CurrentDateTime;
            // Estimate the timeToFinish for order and set it on the order
            order.EstimatedPackingTimeInSeconds = EstimateOrderPackingTime(order);
            // Estimate fill in each area for the order
            Dictionary<AreaCode, double> fillPerArea = order.Areas.ToDictionary(k => k.Key, v => AreaFill(order.Areas.Count));
            // Update order fill vector
            order.EstimatedAreaFill = fillPerArea;
            // Update global areafillmatrix for the next est. time steps
            
        }

        private int EstimateOrderPackingTime(Order order)
        {
            // MainLoop time
            var totalTimeOnMainLoop = order.Areas.Count * SimulationConfiguration.GetTimeInMainLoop();
            // Per area time
            var timePerArea = order.Areas.ToDictionary(k => k.Key, v => 0);
            order.Lines.ForEach(l => timePerArea[l.Article.AreaCode] += EstimateTimeBasedOnQuantity(l.Quantity));
            
            // total time
            return totalTimeOnMainLoop + timePerArea.Sum(kvp => kvp.Value);
        }

        private int EstimateTimeBasedOnQuantity(int quantity)
        {
            if (quantity < SimulationConfiguration.GetLineCountDifferentiation())
            {
                return Math.Max(1, (int) (quantity * SimulationConfiguration.GetTimePerArticlePick()));
            }
            return (int) (quantity * SimulationConfiguration.GetTimePerArticlePick() * SimulationConfiguration.GetLargeLineQuantityMultiplier());
        }

        private double AreaFill(int areaCount)
        {
            // Return normalized fill
            return 1d / areaCount;
        }
    }
}