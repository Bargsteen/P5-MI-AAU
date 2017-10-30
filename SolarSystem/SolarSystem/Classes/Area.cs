using System;
using System.Collections.Immutable;
using SolarSystem.Classes;
using SolarSystem.Interfaces;

namespace SolarSystem.Classes
{
    public abstract class Area : IRecieveOrder
    {
        private Order _orderReceived = null;
        private delegate OrderBoxProgress OrderCompletedHandler(OrderBoxProgress OrderBoxProgress);
        event OrderCompletedHandler OrderBoxProgressDoneEvent;

        public string AreaName { get; set; }
        public ImmutableArray<ItemType> AvailableWares { get; set; }
        public Station[] Stations { get; }
        public ShelfSpace ShelfSpace { get; }

        public Area(string areaName, ImmutableArray<ItemType> availableWares, Station[] stations, ShelfSpace shelfSpace)
        {
            AreaName = areaName ?? throw new ArgumentNullException(nameof(areaName));
            AvailableWares = availableWares;
            Stations = stations ?? throw new ArgumentNullException(nameof(stations));
            ShelfSpace = shelfSpace ?? throw new ArgumentNullException(nameof(shelfSpace));
        }

        public delegate OrderBoxProgress OrderOutputHandler(OrderBoxProgress order);

        event OrderOutputHandler OrderOutputEvent;

        //method for distributing orders to stations
        private void DistributeOrder(OrderBoxProgress receivedOrderBoxProgress)
        {
            // Foreach station in stations:
            foreach (Station station in Stations)
            {
                // Call Station.RecieveBox()
                station.ReceiveBox(receivedOrderBoxProgress
                    .);
                // Check return of Station.RecieveBox()
                // If Success - then we good
                // If FullError - Check next in stations
            }

            // If all FullError - Alert Handler
        }

        //Listening on stations for orders that are done
        public void RecieveOrder(OrderBoxProgress OrderBoxProgress)
        {
            //call DistributeOrder with input as parameter
            DistributeOrder(OrderBoxProgress);

        }


    }
}
