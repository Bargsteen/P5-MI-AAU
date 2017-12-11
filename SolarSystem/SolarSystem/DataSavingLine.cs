using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem
{
    public class DataSavingLine
    {
        public DataSavingLine(Line line)
        {
            _line = line;
        }

        Line _line;
        DateTime plannedPickingTime;
        DateTime actualPickingTime;
    }
}
