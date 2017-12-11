using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Data
{
    public class DataSavingLine
    {
        public DataSavingLine(Line line)
        {
            Line = line;
        }

        public Line Line;
        DateTime plannedPickingTime;
        DateTime actualPickingTime;
    }
}
