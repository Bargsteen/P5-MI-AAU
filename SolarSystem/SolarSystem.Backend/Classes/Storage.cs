﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
{
    public class Storage
    {
        public event Action OnSendShelfBoxToStation;
        
        private readonly Queue<(Station, Article)> _requestQueue;
    
        private const int MaxLineQuantity = 10000;

        public Storage()
        {
            _requestQueue = new Queue<(Station, Article)>();

            TimeKeeper.Tick += DistributeShelfBoxToStation;
        }
        
        
        public void ReceiveRequestForShelfBoxes(Station station, List<Article> articlesNeeded)
        {
            articlesNeeded.ForEach(a => _requestQueue.Enqueue((station, a)));
        }

        private void DistributeShelfBoxToStation()
        {
            if (_requestQueue.Any())
            {
                var stationRequest = _requestQueue.Dequeue();
            
                var shelfBoxLine = new Line(stationRequest.Item2, MaxLineQuantity);
            
                stationRequest.Item1.ReceiveShelfBoxFromStorage(new ShelfBox(shelfBoxLine));
                
                OnSendShelfBoxToStation?.Invoke();
            }
        }
    }
}