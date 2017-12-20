namespace SolarSystem.Backend.Classes.Simulation.ConstantsAndEnums
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

            return RealConstants.MinArticleQuantityOg;
        }
        
        public static int GetMaxArticleQuantityOG(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MaxArticleQuantityOG;
            }

            return RealConstants.MaxArticleQuantityOg;
        }
        
        public static int GetMinLineCountOG(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MinLineCountOG;
            }

            return RealConstants.MinLineCountOg;
        }
        
        public static int GetMaxLineCountOG(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MaxLineCountOG;
            }

            return RealConstants.MaxLineCountOg;
        }
        
        public static int GetRandomSeedValue(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.RandomSeedValue;
            }

            return RealConstants.RandomSeedValue;
        }
    }
}
