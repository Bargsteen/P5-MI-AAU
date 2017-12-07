using System;
using System.Collections.Generic;
using Accord.Math;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public class Mi2Scheduler : SchedulerModular
    {
        private const int N = 100;
        private const int WeightCount = 19000;
        
        private List<Memory> ReplayMemory { get; set; }
        private Sparse<double> Weights { get; set; }
        
        private List<Sparse<double>> Actions { get; set; }
        
        private static Random Random { get; }
        
        public Mi2Scheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime, List<Article> articles) : base(orderGenerator, handler, poolMoverTime)
        {
            // Initialize replay memory to capacity N
            ReplayMemory = new List<Memory>(N);
            // Initialize a set of weights, theta, to 0
            Weights = new Sparse<double>(WeightCount);
            
            // Sentinel waiting action. Must be checked for later 
            var waitOrder = new Order(0, DateTime.MinValue, new List<Line>()); 
            ActualOrderPool.Add(waitOrder);
            Actions.Add(StateRepresenter.MakeFullRepresentation(waitOrder, articles,
                Sparse.FromDense(SimulationInformation.GetState())));
        }        

        public override Order ChooseNextOrder()
        {
            // HMM
           /* Matrix.Dot(Actions.ToArray(), Weights.ToDense());
            
                
               
            
            
            int actionCount = Actions.Count;
            
            double currentlyBestQ = Double.MinValue;
            int currentlyBestIndex = 0;
            for(int i = 0; i < Actions.)
            
            var indexOfBestOrder = Actions.IndexOf(bestOrder);*/
            return new Order(0, DateTime.MinValue, new List<Line>()); 
        }

        private double CalculateQ(Sparse<double> stateAndAction, Sparse<double> weights)
        {
            // TODO: Proper forward activation should be implemented
            // Should look for further actions as well, not just this single one
            return stateAndAction.Dot(weights);
        }
    }

    internal struct Memory
    {
        public Sparse<double> s_t { get; set; }
        public Sparse<double> a_t { get; set; }
        public double r_t { get; set; }
        public Sparse<double> s_t_1 { get; set; }
        
    }
}