using System.Runtime.Remoting.Messaging;

namespace SolarSystem.Backend.Classes.Simulation
{
    

    
    public static class SimulationConfiguration
    {
        public static RandomSeedType SeedType;
        public static SimulationState SimulationState;
        
        public static double GetTimePerArticlePick(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.TimePerArticlePick;
            }

            return RealConstants.TimePerArticlePick;
        }
        
        public static int GetSchedulerPoolMoveTime(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.SchedulerPoolMoveTime;
            }

            return RealConstants.SchedulerPoolMoveTime;
        }
        
        public static int GetLineCountDifferentiation(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.LineCountDifferentiation;
            }

            return RealConstants.LineCountDifferentiation;
        }
        
        public static double GetLargeLineQuantityMultiplier(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.LargeLineQuantityMultiplier;
            }

            return RealConstants.LargeLineQuantityMultiplier;
        }
        
        public static int GetTimeInMainLoop(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.TimeInMainLoop;
            }

            return RealConstants.TimeInMainLoop;
        }
        
        public static int GetMinArticleQuanitityOG(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MinArticleQuantityOG;
            }

            return RealConstants.MinArticleQuantityOG;
        }
        
        public static int GetMaxArticleQuantityOG(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MaxArticleQuantityOG;
            }

            return RealConstants.MaxArticleQuantityOG;
        }
        
        public static int GetMinLineCountOG(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MinLineCountOG;
            }

            return RealConstants.MinLineCountOG;
        }
        
        public static int GetMaxLineCountOG(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MaxLineCountOG;
            }

            return RealConstants.MaxLineCountOG;
        }
        
        public static int GetRandomSeedValue(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.RandomSeedValue;
            }

            return RealConstants.RandomSeedValue;
        }
        
        
        
        
        
        
        
            /*
        public static readonly double TimePerArticlePick = 3;
        public static readonly int SchedulerPoolMoveTime = 15;
        public static readonly int LineCountDifferentiation = 100;
        public static readonly double LargeLineQuantityMultiplier = 0.0001;
        public static readonly int TimeInMainLoop = 14;
        public static readonly int MinArticleQuantityOG = 5;
        public static readonly int MaxArticleQuantityOG = 28;
        public static readonly int MinLineCountOG = 1;
        public static readonly int MaxLineCountOG = 6;
        public static readonly int RandomSeedValue = 10;
     */
    }
    
    
}

/*
    public static readonly double TimePerArticlePick = 3;
    public static readonly int SchedulerPoolMoveTime = 15;
    public static readonly int LineCountDifferentiation = 100;
    public static readonly double LargeLineQuantityMultiplier = 0.0001;
    public static readonly int TimeInMainLoop = 14;
    public static readonly int MinArticleQuantityOG = 5;
    public static readonly int MaxArticleQuantityOG = 28;
    public static readonly int MinLineCountOG = 1;
    public static readonly int MaxLineCountOG = 6;
    public static readonly int RandomSeedValue = 10;
 */