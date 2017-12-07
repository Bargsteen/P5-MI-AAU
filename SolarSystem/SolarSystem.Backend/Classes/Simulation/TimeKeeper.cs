using System;
using System.Threading;

namespace SolarSystem.Backend.Classes
{
    public delegate void TickHandler();
    
    public static class TimeKeeper
    {
        public static event Action Tick;
        
        public static DateTime CurrentDateTime { get; private set; }

        public static void StartTicking(double ticksPerSecond, DateTime startDateTime)
        {
            CurrentDateTime = startDateTime;
            
            // Calculate time to wait between each tick
            int waitingTime = (int)(1000 / ticksPerSecond);  
                       
            while (true)
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