using System;
using System.Threading;

namespace SolarSystem.Backend.Classes.Simulation
{
    public delegate void TickHandler();
    
    public static class TimeKeeper
    {
        public static event Action Tick;
        
        public static DateTime CurrentDateTime { get; private set; }

        public static event Action SimulationFinished;

        public static void StartTicking(double ticksPerSecond, DateTime startDateTime, int hoursToSimulate)
        {
            CurrentDateTime = startDateTime;
            
            // Calculate time to wait between each tick
            int waitingTime = (int)(1000 / ticksPerSecond);

            var finishDateTime = startDateTime.AddHours(hoursToSimulate);
                       
            while (CurrentDateTime <= finishDateTime )
            {
                // Wait and invoke
                Thread.Sleep(waitingTime);
                Tick?.Invoke();
                
                // Increment CurrentDateTime, so it always shows a current time of the day.
                CurrentDateTime = CurrentDateTime.AddSeconds(1);
            }
            
            SimulationFinished?.Invoke();
            
        }
    }
}