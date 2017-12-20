using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleTables;
using SolarSystem.Backend.Solution.Simulation;
using SolarSystem.Backend.Solution.Simulation.Boxes;
using SolarSystem.Backend.Solution.Simulation.Orders;
using SolarSystem.Backend.Solution.Simulation.Warehouse;

namespace SolarSystem.Backend.Solution.Schedulers
{
    class EstimatorScheduler : Scheduler
    {
        private const int SecondsLookAhead = 20000;
        private const int MaxOrdersPerArea = 25;
        private readonly Queue<Dictionary<AreaCode, decimal>> _areaFillInfo;
        private SimulationInformation _simInfo;
        private readonly OrdertimeEstimator _orderTimeEstimator;

        public EstimatorScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime, SimulationInformation siminfo) : base(
            orderGenerator, handler, poolMoverTime)
        {
            OnOrderActuallySent += MakeOrderFill;
            TimeKeeper.Tick += EnAndDequeueOnTick;
            _simInfo = siminfo;
            _orderTimeEstimator = new OrdertimeEstimator(siminfo);

            //OnOrderActuallySent += OrderBox => PrintXTimeStepsOfMatrix(1);
            
            foreach (var areasKey in handler.Areas.Keys)
            {
                handler.Areas[areasKey].OnOrderBoxInAreaFinished += UpdateOnOrderFinishedAtArea;
            }
            
            var waitOrder = new Order(0, DateTime.MinValue, new List<Line>()); 
            SetOrderFill(waitOrder);
            ActualOrderPool.Add(waitOrder);
            
            _areaFillInfo = new Queue<Dictionary<AreaCode, decimal>>();
            for (int i = 0; i < SecondsLookAhead; i++)
            {
                var zeroDicts = new Dictionary<AreaCode, decimal>
                {
                    {AreaCode.Area21, 0},
                    {AreaCode.Area25, 0},
                    {AreaCode.Area27, 0},
                    {AreaCode.Area28, 0},
                    {AreaCode.Area29, 0}
                };

                _areaFillInfo.Enqueue(zeroDicts);
            }

        }

        protected override void DoWhenMoveInitialToActualPool()
        {
            InitialOrderPool.ForEach(SetOrderFill);
        }

        private void EnAndDequeueOnTick()
        {
            _areaFillInfo.Dequeue();
            
            _areaFillInfo.Enqueue(new Dictionary<AreaCode, decimal>
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
            var orderedActualPool = ActualOrderPool.OrderByDescending(o => CalcOrderFitness(o, 20));
            var ord = orderedActualPool.First();
            return ord;
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
            
            decimal areaFillValue = AreaFill(order.Areas.Count(a => !a.Value));
            
            
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
            
            UpdateAreaFillInfo(_areaFillInfo, applyOrder);
        }

        private Dictionary<AreaCode, decimal> CalcValuesForOrder(Order order)
        {
            var fillInfoDict = new Dictionary<AreaCode, decimal>();
            fillInfoDict.Add(AreaCode.Area21, 0);
            fillInfoDict.Add(AreaCode.Area25, 0);
            fillInfoDict.Add(AreaCode.Area27, 0);
            fillInfoDict.Add(AreaCode.Area28, 0);
            fillInfoDict.Add(AreaCode.Area29, 0);

            if (order.OrderId == 0) return fillInfoDict;

            decimal areaFillValue = AreaFill(order.Areas.Count(a => !a.Value));
            
            int count = 0;
            foreach (var area in order?.Areas)
            {
                fillInfoDict[area.Key] = areaFillValue;
            }

            return fillInfoDict;
        }

        private double CalcOrderFitness(Order order, int stepLookAhead)
        {
            var lookAheadCount = 0;
            var summedFitness = 0d;
            foreach (var areaFill in _areaFillInfo)
            {
                foreach (var key in areaFill.Keys)
                {
                    var val = order.EstimatedAreaFill[key] + areaFill[key];
                    var fitness = CalcErrorForOrderFitness((double)val);
                    summedFitness += fitness;
                }
                lookAheadCount++;
                if (lookAheadCount >= stepLookAhead)
                {
                    break;
                }
            }
            order.OrderFitness = summedFitness;
            
            return summedFitness;
        }

        private double CalcErrorForOrderFitness(double val)
        {
            if (val <= MaxOrdersPerArea)
            {
                return MapXFromTo(val, 0, MaxOrdersPerArea, 0, 1);
            }
            return MapXFromTo(val, MaxOrdersPerArea, 40, 0, -10);
        }

        private double MapXFromTo(double x, double fromLower, double fromUpper, double toLower, double toUpper)
        {
            // y=(x−a)*((d−c)/(b−a))+c
            return (x - fromLower) * ((toUpper - toLower) / (fromUpper - fromLower)) + toLower;
        }

        private void MakeOrderFill(Order order)
        {
            SetOrderFill(order);

            // Push to matrix
            UpdateAreaFillInfo(_areaFillInfo, order);
        }

        private void SetOrderFill(Order order)
        {
            // Set the startPackingTime for order
            order.StartPackingTime = TimeKeeper.CurrentDateTime;
            // Estimate the timeToFinish for order and set it on the order
            order.EstimatedPackingTimeInSeconds = EstimateOrderPackingTime(order);

            // Make matrix representation
            order.EstimatedAreaFill = CalcValuesForOrder(order);
        }

        private void UpdateAreaFillInfo(Queue<Dictionary<AreaCode, decimal>> areaInfo, Order order, int stepsToUpdate = 0)
        {
            var startTime = order.StartPackingTime;
            var estimatedEndTime = startTime.AddSeconds(order.EstimatedPackingTimeInSeconds);
            var timeSpent = TimeKeeper.CurrentDateTime - startTime;
            if (stepsToUpdate == 0)
            {
                stepsToUpdate = (int)(estimatedEndTime - startTime.AddSeconds(timeSpent.TotalSeconds)).TotalSeconds;
            }

            var stepsUpdated = 0;

            
            foreach (var stepInfo in areaInfo)
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
            // Guess the ordertime from the learned Machine Algorithm
            return (int)_orderTimeEstimator.GuessTimeForOrder(order);
        }

        private decimal AreaFill(int areaCount)
        {
            // Return normalized fill
            if (areaCount == 0) return 0;
            return 1m / areaCount;
        }

        public void PrintXTimeStepsOfMatrix(int timeStepsToRun)
        {
            Console.Clear();
            int timeStepsUsed = 0;

            ConsoleTable table = new ConsoleTable("Time", "Area 21", "Area 25", "Area 27", "Area 28", "Area 29");
            foreach (var kvp in _areaFillInfo)
            {
                var row = new List<decimal>{timeStepsUsed};
                row.AddRange(kvp.Values);

                if (row.Any(d => d < 0))
                {
                    var b = 2;
                }

                var fRow = row.Select(x => $"{x:N2}").ToArray();
                
                table.AddRow(fRow[0], fRow[1], fRow[2], fRow[3], fRow[4], fRow[5]);
                
                if (timeStepsUsed++ >= timeStepsToRun)
                {
                    break;
                }
            }
            
            table.Write(Format.Alternative);
        }
    }
}