using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.PickingAndErp;

namespace SolarSystem
{
    public class Statistics
    {
        private List<PickingOrder> orderList;
        private double averageTimePerAreaOurs { get; set; }
        private int finishedOrderCount;
        private long totalLineCount = 0;
        private long totalLineQuantity = 0;
        
        public double AverageQuantityPerLine;

        private Order SlowestOrder;

        public Statistics(List<PickingOrder> orderList, Runner runner)
        {
            this.orderList = orderList ?? throw new ArgumentNullException(nameof(orderList));
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
            totalLineCount += order.Lines.Count;
        }

        private void AddToTotalLineQuantity(Order order)
        {
            totalLineQuantity += order.Lines.Select(l => l.Quantity).Sum();
        }

        private void KeepTrackOfSlowestOrder(Order order)
        {
            var timeSpentForOrder = GetTotalTimeForOrderInMinutes(order);

            if (SlowestOrder == null || GetTotalTimeForOrderInMinutes(SlowestOrder) <= timeSpentForOrder)
            {
                SlowestOrder = order;
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
            foreach (var order in orderList)
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

            return summedAverage / orderList.Count;
        }

        private void AddAverageTimePerAreaToSum(Order finishedOrder)
        {
            finishedOrderCount++;
            
            var orderedAreas = finishedOrder.AreaTimeInOutLog.Values.OrderBy(t => t.OutTime).ToList();
            var firstAreaFinished = orderedAreas.First();
            var lastAreaFinished = orderedAreas.Last();

            var timeSpent = lastAreaFinished.OutTime - firstAreaFinished.OutTime;

            var averageTimePerArea = orderedAreas.Count >= 2 ? timeSpent.TotalMinutes / (orderedAreas.Count() - 1) : 0;
            
            averageTimePerAreaOurs += averageTimePerArea;
        }
        
        public double GetFinalAverageTimePerAreaSim()
        {
            return averageTimePerAreaOurs / finishedOrderCount;
        }

        public double GetSlowestOrderTime()
        {
            return GetTotalTimeForOrderInMinutes(SlowestOrder);
        }

        public double GetAverageLinesPerOrderSim()
        {
            return totalLineCount / finishedOrderCount;
        }

        public double GetAverageQuantityPerLineSim()
        {
            return totalLineQuantity / totalLineCount;
        }

        public double GetAverageQuantityPerLineSolar()
        {
            var totalQuantity = 0;
            var totalLines = 0;
            
            foreach (var pickingOrder in orderList)
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

            foreach (var pickingOrder in orderList)
            {
                totalLines += pickingOrder.LineList.Count;
            }

            return totalLines / orderList.Count;
        }
    }
}