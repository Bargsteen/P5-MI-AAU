using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            
            Stations = new[]
            {
                new Station("A", 1000, 1000),
                new Station("B", 1000, 1000),
                new Station("C", 1000, 1000)
            };
            
            // Subscribe to each Station order complete event
            foreach (Station station in Stations)
            {
                station.OnOrderBoxFinished += StationOrderCompleted;
            }
        }


        //method for distributing orders to stations
        private void DistributeOrder(OrderBox receivedOrderBox)
        {
            Console.WriteLine($"Area: {AreaCode}: Sending to station");
            // Variable for checking succes or failure 
            StationResult result = StationResult.FullError;

            // Foreach station in stations:
            foreach (Station station in Stations)
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
            OnOrderBoxReceivedAtAreaEvent?.Invoke(this.AreaCode);
            Console.WriteLine($"Area: {this.AreaCode} received order");
            //call DistributeOrder with input as parameter
            DistributeOrder(OrderBox);

        }

        public void StationOrderCompleted(OrderBox orderBox)
        {
            Console.WriteLine("Area: Sending back to Handler");
            OnOrderBoxInAreaFinished?.Invoke(orderBox, AreaCode);
        }

    }
}
