using System;
using System.Threading;

namespace SolarSystem.Classes
{
    public delegate void TickHandler();
    public class TimeKeeper
    {
        public static event TickHandler Tick;
        private bool running = true;
        

        public void Run(double simulationSpeed)
        {
            int i = 0;
            
            int 
            
            Tick += () => Console.WriteLine(i);
            
            while (running)
            {
                Thread.Sleep(1000);
                i++;
                
                Tick?.Invoke();
            }
            
        }
        
        
    }
}