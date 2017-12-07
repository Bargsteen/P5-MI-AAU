using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Interfaces;

namespace SolarSystem.Backend.Classes
{
    public class Area
    {
        public event Action<OrderBox, AreaCode> OnOrderBoxInAreaFinished;
        public event Action<OrderBox, AreaCode> OnOrderBoxReceivedAtAreaEvent;
        
        public AreaCode AreaCode { get; set; }
        public List<Article> AvailableWares { get; set; }
        public Station[] Stations { get; }
        public ShelfSpace ShelfSpace { get; }
        public Storage Storage { get; }
        private static readonly Random Rand = new Random();
        
        public bool AreaIsFull => Stations.All(s => s.StationIsFull);

        public Area(AreaCode areaCode)
        {
            AreaCode = areaCode;
            
            Storage = new Storage();
            
            Stations = new[]
            {
                new Station(areaCode + "+S:A", 7, 5, AreaCode),
                new Station(areaCode + "+S:B", 7, 5, AreaCode),
                new Station(areaCode + "+S:C", 7, 5, AreaCode),
                new Station(areaCode + "+S:C", 7, 5, AreaCode),
                new Station(areaCode + "+S:C", 7, 5, AreaCode)
            };
            
            // Subscribe to each Station order complete event
            foreach (Station station in Stations)
            {
                station.OnOrderBoxFinishedAtStation += StationOrderCompleted;
                station.OnShelfBoxNeededRequest += Storage.ReceiveRequestForShelfBoxes;
            }
        }


        //method for distributing orders to stations
        private void DistributeOrder(OrderBox receivedOrderBox)
        {
            // Make a list of all non-full stations in a random order
            var nonFullStationsInRandomOrder = Stations
                .Where(s => !s.StationIsFull)
                .OrderBy(a => Rand.Next()).ToList();
            // Choose one station and send the receivedOrderBox to it
            if (!nonFullStationsInRandomOrder.Any())
            {
                throw new AccessViolationException($"{this} should not have received an orderBox because area is full.");
            }

            nonFullStationsInRandomOrder.First().ReceiveOrderBox(receivedOrderBox);
        }

        //Listening on stations for orders that are done
        public void ReceiveOrderBox(OrderBox orderBox)
        {   
            OnOrderBoxReceivedAtAreaEvent?.Invoke(orderBox, AreaCode);
            //call DistributeOrder with input as parameter
            DistributeOrder(orderBox);

        }

        public void StationOrderCompleted(OrderBox orderBox)
        {
            OnOrderBoxInAreaFinished?.Invoke(orderBox, AreaCode);
        }

        public override string ToString()
        {
            return AreaCode.ToString();
        }
    }
}
