namespace SolarSystem.Backend.Solution.Simulation
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
        
        public static int GetMinArticleQuanitityOg(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MinArticleQuantityOg;
            }

            return RealConstants.MinArticleQuantityOg;
        }
        
        public static int GetMaxArticleQuantityOg(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MaxArticleQuantityOg;
            }

            return RealConstants.MaxArticleQuantityOg;
        }
        
        public static int GetMinLineCountOg(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MinLineCountOg;
            }

            return RealConstants.MinLineCountOg;
        }
        
        public static int GetMaxLineCountOg(){
            if (SimulationState == SimulationState.Experimental)
            {
                return ExperimentalConstants.MaxLineCountOg;
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
