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

        public static event Action AllRunsFinished;

        public static void StartTicking(double ticksPerSecond, DateTime startDateTime, int hoursToSimulate, int runsToDo)
        {
            CurrentDateTime = startDateTime;
            
            // Calculate time to wait between each tick
            int waitingTime = (int)(1000 / ticksPerSecond);

            var finishDateTime = startDateTime.AddHours(hoursToSimulate);

            var reRunCount = 0;
                       
            while (reRunCount < runsToDo)
            {
                if (CurrentDateTime >= finishDateTime)
                {
                    reRunCount++;
                    if (reRunCount == runsToDo)
                    {
                        break;
                    }
                    
                    SimulationFinished?.Invoke();
                    
                    CurrentDateTime = startDateTime;
                }
                
                // Wait and invoke
                Thread.Sleep(waitingTime);
                Tick?.Invoke();
                
                // Increment CurrentDateTime, so it always shows a current time of the day.
                CurrentDateTime = CurrentDateTime.AddSeconds(1);
            }

            SimulationFinished?.Invoke();
            AllRunsFinished?.Invoke();
            
            
        }
    }
}