using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation.ConstantsAndEnums;
using SolarSystem.Backend.Classes.Simulation.WareHouse;

namespace SolarSystem.Backend.Classes.Simulation  
{  
    public class SimulationInformation  
    {  
        private Handler Handler { get; }  
        public static Dictionary<AreaCode, double> AreaInformation;

        private int _totalLinesFinished;
        private readonly DateTime _schedulerStartTime;
  
        public SimulationInformation(Handler handler, DateTime schedulerStartTime)  
        {  
            Handler = handler;
            
            _schedulerStartTime = schedulerStartTime;

            handler.OnOrderBoxFinished += box => _totalLinesFinished += box.LineIsPickedStatuses.Keys.Count;
  
            AreaInformation = Handler.Areas.ToDictionary(a => a.Key, x => 0.0);  
  
            // Init all EventListeners for areas  
            foreach (var area in Handler.Areas)  
            {  
                area.Value.OnOrderBoxReceivedAtAreaEvent += (box, code) => AreaInformation[code] += 1;  
                area.Value.OnOrderBoxInAreaFinished += (box, code) => AreaInformation[code] -= 1;  
            }  
        }

        public double[] GetState()
        {
            var values = AreaInformation.Values.Concat(new[] {(double)Handler.MainLoop.BoxesInMainLoop}).ToArray();
            return values;
        }
        
        public double GetReward()
        {          
            // Get running time since started packing
            var timeSinceStartedPacking = TimeKeeper.CurrentDateTime - _schedulerStartTime;
            // Calculate average lines per hour
            var avgLinesPerHour = (_totalLinesFinished / timeSinceStartedPacking.TotalSeconds) * 3600;
            // Calc reward based on squared errors above or below 2600 or 2300 lines/houe
            var error = CalcSquaredError(2300, 2600, avgLinesPerHour);
            // Pass to activation function
            return Activate(error);
        }

        private static double CalcSquaredError(double min, double max, double actual)
        {
            if (actual >= max)
            {
                return Math.Pow(actual - max, 2);
            }
            if (actual <= min)
            {
                return Math.Pow(actual - min, 2);
            }
            return 0;
        }

        private static double Activate(double value)
        {
            return value / (1 + Math.Abs(value));
        }
    }  
}  