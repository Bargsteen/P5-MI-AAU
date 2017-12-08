using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public class Mi2Scheduler : Scheduler
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
            Weights = new Sparse<double>(Enumerable.Range(0, WeightCount).ToArray(), Enumerable.Repeat(Random.NextDouble(), WeightCount).ToArray());
            
            // Sentinel waiting action. Must be checked for later 
            var waitOrder = new Order(0, DateTime.MinValue, new List<Line>()); 
            ActualOrderPool.Add(waitOrder);
            Actions.Add(StateRepresenter.MakeFullRepresentation(waitOrder, articles,
                Sparse.FromDense(SimulationInformation.GetState())));
        }

        protected override Order ChooseNextOrder()
        {
            int actionCount = Actions.Count;


            int bestActionIndex = int.MinValue;
            double bestActionValue = double.MinValue;
            
            for (int i = 0; i < actionCount; i++)
            {
                var action = Actions[i];
                var valueForAction = action.Dot(Weights);
                if (valueForAction > bestActionValue)
                {
                    bestActionValue = valueForAction;
                    bestActionIndex = i;
                }
            }

            return ActualOrderPool[bestActionIndex];

            // Get Reward
            // Store transition
            // Sample mini batch and learn with gradient descent
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
        public Sparse<double> ST { get; set; }
        public Sparse<double> AT { get; set; }
        public double RT { get; set; }
        public Sparse<double> ST1 { get; set; }
        
    }
}