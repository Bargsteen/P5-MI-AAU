using System;
using SolarSystem.Backend.Classes;

namespace SolarSystem.Backend.Interfaces
{
    public interface ITimeKeeper
    {        
        void StartTicking(double ticksPerSecond);
        DateTime CurrentDateTime { get; }
        event TickHandler Tick;
    }
}