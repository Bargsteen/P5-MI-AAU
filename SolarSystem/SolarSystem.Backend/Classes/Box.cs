﻿namespace SolarSystem.Backend.Classes
{
    public abstract class Box
    {
        public int Id;

        private static int NextId = 1;
        
        public Box()
        {
            Id = NextId++;
        }
    }
}
