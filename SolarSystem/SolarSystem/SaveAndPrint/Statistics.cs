using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.Classes.Simulation.Boxes;
using SolarSystem.Backend.Classes.Simulation.Orders;
using SolarSystem.Backend.PickingAndErp;

namespace SolarSystem.SaveAndPrint
{
    public class Statistics
    {
        private List<PickingOrder> _orderList;
        private double AverageTimePerAreaOurs { get; set; }
        private int _finishedOrderCount;
        private long _totalLineCount;
        private long _totalLineQuantity;

        private Order _slowestOrder;

        public Statistics(List<PickingOrder> orderList, Runner runner)
        {
            _orderList = orderList ?? throw new ArgumentNullException(nameof(orderList));
            runner.Handler.OnOrderBoxFinished += OnEachOrderBoxFinished;
        }

        public void OnEachOrderBoxFinished(OrderBox orderBoxFinished)
        {
            var order = orderBoxFinished.Order;
            KeepTrackOfSlowestOrder(order);
            AddAverageTimePerAreaToSum(order);
            AddToTotalLineCount(order);
            AddToTotalLineQuantity(order);
        }

        private void AddToTotalLineCount(Order order)
        {
            _totalLineCount += order.Lines.Count;
        }

        private void AddToTotalLineQuantity(Order order)
        {
            _totalLineQuantity += order.Lines.Select(l => l.Quantity).Sum();
        }

        private void KeepTrackOfSlowestOrder(Order order)
        {
            var timeSpentForOrder = GetTotalTimeForOrderInMinutes(order);

            if (_slowestOrder == null || GetTotalTimeForOrderInMinutes(_slowestOrder) <= timeSpentForOrder)
            {
                _slowestOrder = order;
            }
        }

        private double GetTotalTimeForOrderInMinutes(Order order)
        {
            if (order == null) return 0;
            
            var sortedAreas = order.AreaTimeInOutLog.OrderBy(o => o.Value.OutTime);
            var firstArea = sortedAreas.First();
            var lastArea = sortedAreas.Last();

            var totalTimeSpent = lastArea.Value.OutTime - firstArea.Value.InTime;
            return totalTimeSpent.TotalMinutes;
        }
        
        public double CalcAverageTimePerAreaSolar()
        {
            var summedAverage = 0d;
            foreach (var order in _orderList)
            {
                var orderedLines = order.LineList.OrderBy(l => l.OutTimeStamp).ToList();
                var firstLineFinished = orderedLines.First();
                var lastLineFinished = orderedLines.Last();

                var timeSpent = lastLineFinished.OutTimeStamp - firstLineFinished.OutTimeStamp;
                
                // Find distinct areas, and minus by one, since we use the finishTime from the first area
                var totalAreasVisited = orderedLines.Select(l => l.Article.AreaCode).Distinct().Count() - 1;
                var average = totalAreasVisited >= 1 ? timeSpent.TotalMinutes / totalAreasVisited : 0;
                summedAverage += average;
            }

            return summedAverage / _orderList.Count;
        }

        private void AddAverageTimePerAreaToSum(Order finishedOrder)
        {
            _finishedOrderCount++;
            
            var orderedAreas = finishedOrder.AreaTimeInOutLog.Values.OrderBy(t => t.OutTime).ToList();
            var firstAreaFinished = orderedAreas.First();
            var lastAreaFinished = orderedAreas.Last();

            var timeSpent = lastAreaFinished.OutTime - firstAreaFinished.OutTime;

            var averageTimePerArea = orderedAreas.Count >= 2 ? timeSpent.TotalMinutes / (orderedAreas.Count() - 1) : 0;
            
            AverageTimePerAreaOurs += averageTimePerArea;
        }
        
        public double GetFinalAverageTimePerAreaSim()
        {
            return AverageTimePerAreaOurs / _finishedOrderCount;
        }

        public double GetSlowestOrderTime()
        {
            return GetTotalTimeForOrderInMinutes(_slowestOrder);
        }

        public double GetAverageLinesPerOrderSim()
        {
            return _totalLineCount / _finishedOrderCount;
        }

        public double GetAverageQuantityPerLineSim()
        {
            return _totalLineQuantity / _totalLineCount;
        }

        public double GetAverageQuantityPerLineSolar()
        {
            var totalQuantity = 0;
            var totalLines = 0;
            
            foreach (var pickingOrder in _orderList)
            {
                foreach (var line in pickingOrder.LineList)
                {
                    totalQuantity += line.Quantity;
                    totalLines++;
                }
            }

            return totalQuantity / totalLines;
        }

        public double GetAverageLinesPerOrderSolar()
        {
            var totalLines = 0;

            foreach (var pickingOrder in _orderList)
            {
                totalLines += pickingOrder.LineList.Count;
            }

            return totalLines / _orderList.Count;
        }
    }
}