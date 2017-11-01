using System;
using System.Collections.Immutable;
using SolarSystem.Backend.Interfaces;

namespace SolarSystem.Backend.Classes
{
    public class Area
    {
        public event Action<OrderBox, AreaCode> OnOrderBoxInAreaFinished;
        
        public AreaCode AreaCode { get; set; }
        public ImmutableArray<ItemType> AvailableWares { get; set; }
        public Station[] Stations { get; }
        public ShelfSpace ShelfSpace { get; }
        private readonly ITimeKeeper _timeKeeper;

        public Area(AreaCode areaCode, ImmutableArray<ItemType> availableWares, Station[] stations, ShelfSpace shelfSpace, ITimeKeeper timeKeeper)
        {
            _timeKeeper = timeKeeper ?? throw new ArgumentNullException(nameof(timeKeeper));
            AreaCode = areaCode;
            AvailableWares = availableWares;
            Stations = stations ?? throw new ArgumentNullException(nameof(stations));
            ShelfSpace = shelfSpace ?? throw new ArgumentNullException(nameof(shelfSpace));

            // Subscribe to each Station order complete event
            foreach (Station station in Stations)
            {
                station.OnOrderBoxFinished += StationOrderCompleted;
            }
        }

        public Area(AreaCode areaCode, ITimeKeeper timeKeeper)
        {
            _timeKeeper = timeKeeper ?? throw new ArgumentNullException(nameof(timeKeeper));
            Stations = new[]
            {
                new Station("A", 1000, 1000, _timeKeeper),
                new Station("B", 1000, 1000, _timeKeeper),
                new Station("C", 1000, 1000, _timeKeeper)
            };
        }


        //method for distributing orders to stations
        private void DistributeOrder(OrderBox receivedOrderBox)
        {
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
            //call DistributeOrder with input as parameter
            DistributeOrder(OrderBox);

        }

        public void StationOrderCompleted(OrderBox orderBox)
        {
            OnOrderBoxInAreaFinished?.Invoke(orderBox, AreaCode);
        }

    }
}
