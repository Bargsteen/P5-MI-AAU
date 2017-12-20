﻿using System;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation.Boxes;
using SolarSystem.Backend.Classes.Simulation.ConstantsAndEnums;
using SolarSystem.Backend.Classes.Simulation.Orders;

namespace SolarSystem.Backend.Classes.Simulation.WareHouse
{
    public class Area
    {
        public event Action<OrderBox, AreaCode> OnOrderBoxInAreaFinished;
        public event Action<OrderBox, AreaCode> OnOrderBoxReceivedAtAreaEvent;

        private AreaCode AreaCode { get; }
        public Station[] Stations { get; }
        private Storage Storage { get; }
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
                new Station(areaCode + "+S:D", 7, 5, AreaCode),
                new Station(areaCode + "+S:E", 7, 5, AreaCode),

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
            // Set time in the orders log
            orderBox.Order.AreaTimeInOutLog[AreaCode] = new TimeInOut(TimeKeeper.CurrentDateTime);
            
            OnOrderBoxReceivedAtAreaEvent?.Invoke(orderBox, AreaCode);
            
            //call DistributeOrder with input as parameter
            DistributeOrder(orderBox);

        }

        private void StationOrderCompleted(OrderBox orderBox)
        {
            // log time out
            orderBox.Order.AreaTimeInOutLog[AreaCode].OutTime = TimeKeeper.CurrentDateTime;
            
            OnOrderBoxInAreaFinished?.Invoke(orderBox, AreaCode);
        }

        public override string ToString()
        {
            return AreaCode.ToString();
        }
    }
}