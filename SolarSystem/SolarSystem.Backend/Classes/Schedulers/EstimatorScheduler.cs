using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using Accord;
using ConsoleTables;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    class EstimatorScheduler : Scheduler
    {
        private const int secondsLookAhead = 20000;
        private Queue<Dictionary<AreaCode, double>> AreaFillInfo;

        public EstimatorScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(
            orderGenerator, handler, poolMoverTime)
        {
            OnOrderActuallySent += MakeOrderFill;
            TimeKeeper.Tick += EnAndDequeueOnTick;

            OnOrderActuallySent += OrderBox => PrintXTimeStepsOfMatrix(5);
            
            foreach (var areasKey in handler.Areas.Keys)
            {
                handler.Areas[areasKey].OnOrderBoxInAreaFinished += UpdateOnOrderFinishedAtArea;
            }
            
            AreaFillInfo = new Queue<Dictionary<AreaCode, double>>();
            for (int i = 0; i < secondsLookAhead; i++)
            {
                var zeroDicts = new Dictionary<AreaCode, double>
                {
                    {AreaCode.Area21, 0},
                    {AreaCode.Area25, 0},
                    {AreaCode.Area27, 0},
                    {AreaCode.Area28, 0},
                    {AreaCode.Area29, 0}
                };

                AreaFillInfo.Enqueue(zeroDicts);
            }

        }

        private void EnAndDequeueOnTick()
        {
            AreaFillInfo.Dequeue();
            
            AreaFillInfo.Enqueue(new Dictionary<AreaCode, double>
            {
                {AreaCode.Area21, 0},
                {AreaCode.Area25, 0},
                {AreaCode.Area27, 0},
                {AreaCode.Area28, 0},
                {AreaCode.Area29, 0}
            });
        }

        protected override Order ChooseNextOrder()
        {
            return ActualOrderPool.First();
        }

        /* Fix reference problems! ICloneable */

        private void UpdateOnOrderFinishedAtArea(OrderBox orderBox, AreaCode areaCode)
        {
            var order = orderBox.Order;
            
            // Get the old fill

            var oldFill = order.EstimatedAreaFill.ToDictionary(k => k.Key, v => v.Value);             
            
            // Update the order
            order.EstimatedAreaFill[areaCode] = 0;
            
            // Get the new fill
            var newFill = order.EstimatedAreaFill.ToDictionary(k => k.Key, v => v.Value);
            
            double areaFillValue = AreaFill(order.Areas.Count(a => !a.Value));
            
            
            
            
            int count = 0;
            foreach (var area in order.Areas.Where(a => !a.Value))
            {
                newFill[area.Key] = areaFillValue;
            }
            
            // Get the fill that is applied to matrix
            var applyFill = newFill.ToDictionary(k => k.Key, v => v.Value);
            var applyFillList = applyFill.Keys.ToList();
            int index = 0;

            foreach (var key in applyFillList)
            {
                applyFill[key] = newFill.Values.ToArray()[index] - oldFill.Values.ToArray()[index];
                index++;
            }
            
            
            // Make apply Order 
            Order applyOrder = order;
            applyOrder.EstimatedAreaFill = applyFill;
            
            UpdateAreaFillInfo(applyOrder);
        }

        private Dictionary<AreaCode, double> CalcValuesForOrder(Order order)
        {
            var fillInfoDict = new Dictionary<AreaCode, double>();
            fillInfoDict.Add(AreaCode.Area21, 0);
            fillInfoDict.Add(AreaCode.Area25, 0);
            fillInfoDict.Add(AreaCode.Area27, 0);
            fillInfoDict.Add(AreaCode.Area28, 0);
            fillInfoDict.Add(AreaCode.Area29, 0);

            double areaFillValue = AreaFill(order.Areas.Count(a => !a.Value));
            
            int count = 0;
            foreach (var area in order.Areas)
            {
                fillInfoDict[area.Key] = areaFillValue;
            }

            return fillInfoDict;
        }

        private void MakeOrderFill(Order order)
        {
            // Set the startPackingTime for order
            order.StartPackingTime = TimeKeeper.CurrentDateTime;
            // Estimate the timeToFinish for order and set it on the order
            order.EstimatedPackingTimeInSeconds = EstimateOrderPackingTime(order);
                        
            // Make matrix representation
            order.EstimatedAreaFill = CalcValuesForOrder(order);
            
            // Push to matrix
            UpdateAreaFillInfo(order);
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
                if (stepsUpdated >= stepsToUpdate)
                {
                    return;
                }
                
                stepsUpdated++;
                
                for (int i = 0; i < stepInfo.Keys.Count; i++)
                {
                    if (order.EstimatedAreaFill.ContainsKey(stepInfo.Keys.ToArray()[i]))
                    {
                        var stepKey = stepInfo.Keys.ToArray()[i];
                        stepInfo[stepKey] = stepInfo[stepKey] + order.EstimatedAreaFill[stepKey];
                    }
                }
            }
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
            if (areaCount == 0) return 0;
            return 1d / areaCount;
        }

        public void PrintXTimeStepsOfMatrix(int timeStepsToRun)
        {
            Console.Clear();
            int timeStepsUsed = 0;

            ConsoleTable table = new ConsoleTable("Time", "Area 21", "Area 25", "Area 27", "Area 28", "Area 29");
            foreach (var kvp in AreaFillInfo)
            {
                var row = new List<double>{timeStepsUsed};
                row.AddRange(kvp.Values);

                table.AddRow(row[0], row[1], row[2], row[3], row[4], row[5]);
                
                if (timeStepsUsed++ >= timeStepsToRun)
                {
                    break;
                }
            }
            
            table.Write(Format.Alternative);
        }
    }
}