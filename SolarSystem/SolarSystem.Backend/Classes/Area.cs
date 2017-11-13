using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Interfaces;

namespace SolarSystem.Backend.Classes
{
    public class Area
    {
        public event Action<OrderBox, AreaCode> OnOrderBoxInAreaFinished;
        public event Action<AreaCode> OnOrderBoxReceivedAtAreaEvent;
        
        public AreaCode AreaCode { get; set; }
        public List<Article> AvailableWares { get; set; }
        public Station[] Stations { get; }
        public ShelfSpace ShelfSpace { get; }
        public Storage Storage { get; }
        private static Random _rand = new Random();

        public Area(AreaCode areaCode, List<Article> availableWares, Station[] stations, ShelfSpace shelfSpace, ITimeKeeper timeKeeper)
        {
            AreaCode = areaCode;
            AvailableWares = availableWares ?? throw new ArgumentNullException(nameof(availableWares));
            Stations = stations ?? throw new ArgumentNullException(nameof(stations));
            ShelfSpace = shelfSpace ?? throw new ArgumentNullException(nameof(shelfSpace));

            // Subscribe to each Station order complete event
            foreach (Station station in Stations)
            {
                station.OnOrderBoxFinished += StationOrderCompleted;
            }
        }

        public Area(AreaCode areaCode)
        {
            AreaCode = areaCode;
            
            Storage = new Storage();
            
            Stations = new[]
            {
                new Station("A", 7, 5),
                new Station("B", 7, 5),
                new Station("C", 7, 5)
            };
            
            // Subscribe to each Station order complete event
            foreach (Station station in Stations)
            {
                station.OnOrderBoxFinished += StationOrderCompleted;
                station.OnShelfBoxNeededRequest += Storage.ReceiveRequestForShelfBoxes;
            }
        }


        //method for distributing orders to stations
        private void DistributeOrder(OrderBox receivedOrderBox)
        {
            // Variable for checking succes or failure 
            StationResult result = StationResult.FullError;

            // Randomize order of stations to try
            var stationsInRandomOrder = Stations.OrderBy(a => _rand.Next());
            
            // Foreach station in stations:
            foreach (Station station in stationsInRandomOrder)
            {
                // Call Station.RecieveBox()
                result = station.ReceiveBox(receivedOrderBox);

                // Check return of Station.RecieveBox()
                if (result == StationResult.Success)
                {
                    break;
                }

                // Keep looping Stations
            }

            // If all FullError - Alert Handler
            if (result == StationResult.FullError)
            {
                // Alert Handler, Orderbox needs to go to main loop
            }
        }

        //Listening on stations for orders that are done
        public void ReceiveOrderBox(OrderBox OrderBox)
        {
            OnOrderBoxReceivedAtAreaEvent?.Invoke(AreaCode);
            //call DistributeOrder with input as parameter
            DistributeOrder(OrderBox);

        }

        public void StationOrderCompleted(OrderBox orderBox)
        {
            OnOrderBoxInAreaFinished?.Invoke(orderBox, AreaCode);
        }

    }
}
