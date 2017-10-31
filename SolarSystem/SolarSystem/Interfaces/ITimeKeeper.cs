using System;
using Castle.Core.Resource;
using SolarSystem.Classes;

namespace SolarSystem.Interfaces
{
    public interface ITimeKeeper
    {        
        void StartTicking(double ticksPerSecond);
        DateTime CurrentDateTime { get; }
        event TickHandler Tick;
    }
}