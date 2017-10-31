using System;
using System.Threading;
using SolarSystem.Interfaces;

namespace SolarSystem.Classes
{
    public delegate void TickHandler();
    
    public class TimeKeeper : ITimeKeeper
    {
        public event TickHandler Tick;
        private bool _running = true;
        public DateTime CurrentDateTime { get; private set; }

        public TimeKeeper(DateTime startCurrentDateTime)
        {
            CurrentDateTime = startCurrentDateTime;
            
        }
        

        public void StartTicking(double ticksPerSecond)
        {
            // Calculate time to wait between each tick
            int waitingTime = (int)(1000 / ticksPerSecond);  
                       
            while (_running)
            {
                // Wait and invoke
                Thread.Sleep(waitingTime);
                Tick?.Invoke();
                
                // Increment CurrentDateTime, so it always shows a current time of the day.
                CurrentDateTime = CurrentDateTime.AddSeconds(1);
            }
            
        }
        
        
    }
}