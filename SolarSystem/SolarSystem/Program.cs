using System;
using System.Collections.Generic;
using SolarSystem.Simulation;

namespace SolarSystem
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ISim sim = new Sim(3);
            sim.Run(50);
            
            
        }

        public static int SolarAdd(int x, int y)
        {
            return x + y;
        }
    }
}