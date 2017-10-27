using System;
using System.Threading;

namespace SolarSystem.Classes
{
    public delegate void TickHandler();
    public class TimeKeeper
    {
        public static event TickHandler Tick;
        private bool running = true;
        public DateTime CurrentDateTime { get; private set; }

        public TimeKeeper(DateTime startCurrentDateTime)
        {
            CurrentDateTime = startCurrentDateTime;
            
        }
        

        public void StartTicking(double ticksPerSecond)
        {
            // Calculate time to wait between each tick
            int waitingTime = (int)(1000 / ticksPerSecond);  
                       
            while (running)
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