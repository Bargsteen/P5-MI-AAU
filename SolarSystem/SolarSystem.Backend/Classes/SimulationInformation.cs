using System.Collections.Generic; 
using System.Linq;
using Accord.Math;

namespace SolarSystem.Backend.Classes  
{  
    public class SimulationInformation  
    {  
        private Handler Handler { get; }  
        public readonly Dictionary<AreaCode, double> AreaInformation;  
  
        public SimulationInformation(Handler handler)  
        {  
            Handler = handler;  
  
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
            return AreaInformation.Values.ToArray();
        }
    }  
}  