using System;
using System.Collections.Immutable;
using SolarSystem.Classes;
using SolarSystem.Interfaces;

namespace SolarSystem.Classes
{
    public abstract class Area : IRecieveOrder
    {
        private Order _orderReceived = null;

        event EventHandler<OrderBoxEventArgs> OrderBoxCompleteEvent;
        private delegate OrderBoxProgress OrderCompletedHandler(OrderBoxProgress orderBoxProgress);
        event OrderCompletedHandler OrderBoxProgressDoneEvent;

        public int AreaNumber { get; set; }
        public ImmutableArray<ItemType> AvailableWares { get; set; }
        public Station[] Stations { get; }
        public ShelfSpace ShelfSpace { get; }

        public Area(int area, ImmutableArray<ItemType> availableWares, Station[] stations, ShelfSpace shelfSpace)
        {
            AreaNumber = area;
            AvailableWares = availableWares;
            Stations = stations ?? throw new ArgumentNullException(nameof(stations));
            ShelfSpace = shelfSpace ?? throw new ArgumentNullException(nameof(shelfSpace));

            // Subscribe to each Station order complete event
            foreach (Station x in Stations)
            {
                x.OrderBoxCompleteEvent += StationOrderCompleted;
            }
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
        public void ReceiveOrder(OrderBox OrderBox)
        {
            //call DistributeOrder with input as parameter
            DistributeOrder(OrderBox);

        }

        public void StationOrderCompleted(object sender, OrderBoxEventArgs e)
        {
            OrderBoxCompleteEvent(this, e);
        }

    }
}
