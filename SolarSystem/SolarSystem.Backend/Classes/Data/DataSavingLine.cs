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

<<<<<<< HEAD
        public Line _line;
        public DateTime plannedPickingTime;
        //public DateTime actualPickingTime;
=======
        public Line Line;
        DateTime plannedPickingTime;
        DateTime actualPickingTime;
>>>>>>> 76de74826069b4282715aa4af2757804dd98c720
    }
}
