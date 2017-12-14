using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    class EstimatorScheduler : Scheduler
    {
        private const int secondsLookAhead = 20000;
        private readonly Queue<Dictionary<AreaCode, double>> AreaFillInfo;
           
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
            AreaFillInfo = new Queue<Dictionary<AreaCode, double>>();
            var zeroDicts = Enumerable.Repeat(zeroAreaFillDict, secondsLookAhead);
            
            foreach (var zeroDict in zeroDicts)
            {
                AreaFillInfo.Enqueue(zeroDict);
            }

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

        private void UpdateAreaFillInfo(Order order)
        {
            var startTime = order.StartPackingTime;
            var estimatedEndTime = startTime.AddSeconds(order.EstimatedPackingTimeInSeconds);
            var timeSpent = TimeKeeper.CurrentDateTime - startTime;
            var stepsToUpdate = (estimatedEndTime - (startTime.AddSeconds(timeSpent.TotalSeconds))).TotalSeconds;

            var stepsUpdated = 0;
            
            foreach (var stepInfo in AreaFillInfo)
            {
                if (stepsUpdated >= stepsToUpdate) break;

                foreach (var kvp in stepInfo)
                {
                    if (order.EstimatedAreaFill.ContainsKey(kvp.Key))
                    {
                        stepInfo[kvp.Key] = stepInfo[kvp.Key] + order.EstimatedAreaFill[kvp.Key];
                    }
                }
                
            }
        }

        private int EstimateOrderPackingTime(Order order)
        {
            // MainLoop time
            var totalTimeOnMainLoop = order.Areas.Count * GlobalConstants.TimeInMainLoop;
            // Per area time
            var timePerArea = order.Areas.ToDictionary(k => k.Key, v => 0);
            order.Lines.ForEach(l => timePerArea[l.Article.AreaCode] += EstimateTimeBasedOnQuantity(l.Quantity));
            
            // total time
            return totalTimeOnMainLoop + timePerArea.Sum(kvp => kvp.Value);
        }

        private int EstimateTimeBasedOnQuantity(int quantity)
        {
            if (quantity < GlobalConstants.LineCountDifferentiation)
            {
                return Math.Max(1, (int) (quantity * GlobalConstants.TimePerArticlePick));
            }
            return (int) (quantity * GlobalConstants.TimePerArticlePick * GlobalConstants.LargeLineQuantityMultiplier);
        }

        private double AreaFill(int areaCount)
        {
            // Return normalized fill
            return 1d / areaCount;
        }
    }
}