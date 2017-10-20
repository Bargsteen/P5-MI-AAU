using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolarSystem.Simulation
{
    public class Sim : ISim
    {
        private uint timeSpentRunning;
        
        public void Run(uint timeUnits)
        {
            for (int i = 0; i < timeUnits; i++)
            {
                Update();
            }
        }

        private void Update()
        {
            Thread.Sleep(1);
            timeSpentRunning += 1;
            
        }
    }
}