using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;

namespace SolarSystem.Backend.Classes
{
    public class MiScheduler
    {
        private double[] weights;

        private readonly Dictionary<Order, Sparse<int>> _actions;

        private Article[] _articles;

        private double[] _simulationState;
        
        public MiScheduler(int simFeatureCount, Article[] articles)
        {
            _simulationState = new double[simFeatureCount];
            _articles = articles;
            weights = Enumerable.Repeat(1d, articles.Length + simFeatureCount).ToArray();
            _actions = new Dictionary<Order, Sparse<int>>();

            // Sentinel waiting action. Must be checked for later
            var waitOrder = new Order(0, DateTime.Now, new List<Line>());
            _actions.Add(waitOrder, new Sparse<int>());

        }

        public void AddNewOrder(Order order)
        {
            _actions.Add(order, StateRepresenter.MakeOrderRepresentation(order, _articles.ToList()));
        }

        private static double CalculateSingleActionValue(double[] state, double[] weights)
        {
            if (state.Length != weights.Length)
            {
                throw new ArgumentException("The arrays state a weights should be of the same size!");
            }

            return state.Zip(weights, (a, b) => a * b).Sum();
        }

        private Dictionary<Order, double> CalculateAllActionValues()
        {
            // Create dict for saving the action and value-pairs
            var actionAndValue = new Dictionary<Order, double>();
            // For all actions (all orders + waitOrder)
            foreach (var action in _actions)
            {
                // Convert to dense vector representing the article counts for each order
                int[] denseActionVector = action.Value.ToDense();
                double[] denseActionDoubles = denseActionVector.ToDouble();
                
                // Merge with simulation state vector
                var fullStateVector = new List<double>();
                fullStateVector.AddRange(denseActionDoubles);
                fullStateVector.AddRange(_simulationState);
                
                // Calculate single action value and add to dict
                actionAndValue.Add(action.Key, CalculateSingleActionValue(fullStateVector.ToArray(), weights));
            }
            // return dict
            return actionAndValue;
        }


        private void ChooseAction()
        {
            var actionValues = CalculateAllActionValues();
            var bestAction = actionValues.OrderByDescending(kvp => kvp.Value).First().Key;

            // Check if this is the special wait action
            if (bestAction.OrderId != 0)
            {
                // Send Order to simulation
            }
            
            UpdateKnowledge();
        }

        private void UpdateKnowledge()
        {
            var reward = CalculateRewardValue();
            var learningRate = 0.1;
            int weightsLength = weights.Length;
            
            for(int i = 0; i < weightsLength; i++)
            {
                // LEARN SOMETHING - FAKED
                weights[i] += learningRate * reward;
            }
        }

        private double CalculateRewardValue()
        {
            // Moving average
            return 3;
        }
    }
}