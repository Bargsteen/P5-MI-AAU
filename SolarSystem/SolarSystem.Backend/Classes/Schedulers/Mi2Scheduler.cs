using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Accord.Math;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public class Mi2Scheduler : Scheduler
    {
        private const int N = 100;
        private readonly int WeightCount;

        private readonly List<Article> _articles;
        
        private List<Memory> ReplayMemory { get; set; }
        private Sparse<double> Weights { get; set; }
        
        private List<Sparse<double>> Actions { get; set; }
        
        private static Random Random { get; } = new Random();
        
        public Mi2Scheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime, List<Article> articles) : base(orderGenerator, handler, poolMoverTime)
        {
            WeightCount = articles.Count + SimulationInformation.GetState().Length;
            // Initialize replay memory to capacity N
            ReplayMemory = new List<Memory>(N);
            // Initialize a set of weights, theta, to 0
            Weights = new Sparse<double>(Enumerable.Range(0, WeightCount).ToArray(), Enumerable.Repeat(Random.NextDouble(), WeightCount).ToArray());
            
            // Sentinel waiting action. Must be checked for later 
            var waitOrder = new Order(0, DateTime.MinValue, new List<Line>()); 
            ActualOrderPool.Add(waitOrder);

            _articles = articles;

            Actions = new List<Sparse<double>>
            {
                StateRepresenter.MakeOrderRepresentation(waitOrder, articles)
            };

        }

        protected override Order ChooseNextOrder()
        {
            int actionCount = Actions.Count;

            int bestActionIndex = int.MinValue;
            double bestActionValue = double.MinValue;

            var simState = SimulationInformation.GetState();
            
            for (int i = 0; i < actionCount; i++)
            {
                var action = Actions[i];
                var fullActionVector = new List<double>();
                fullActionVector.AddRange(action.ToDense());
                
                fullActionVector.AddRange(simState);
                
                var sparseFullActionVector = Sparse.FromDense(fullActionVector.ToArray());
                
                var valueForAction = sparseFullActionVector.Dot(Weights);
                if (valueForAction > bestActionValue)
                {
                    bestActionValue = valueForAction;
                    bestActionIndex = i;
                }
            }

            return ActualOrderPool[bestActionIndex];

            // Remove from action list if actually chosen. But does not always send. Needs to edit scheduler.
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

        protected override void MoveInitialToActualPool()
        {
            // If the time has passed, and there is something to move => move.
            if (TimerStartMinutes <= PoolTimer++)
            {
                AddOrdersToActionList(InitialOrderPool);
                ActualOrderPool.AddRange(InitialOrderPool);
                InitialOrderPool.Clear();
                
                // Reset timer
                PoolTimer = 0;
            }
            // Else return.
        }

        private void AddOrdersToActionList(List<Order> newOrders)
        {
            Actions.AddRange(newOrders
                .Select(x => StateRepresenter.MakeOrderRepresentation(x, _articles)));
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