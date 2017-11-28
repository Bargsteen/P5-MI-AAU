using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;

namespace SolarSystem.Backend.Classes
{
    public class MiScheduler
    { 
        private double[] weights; 

        public readonly Dictionary<Order, Sparse<int>> _actions; 
 
        public SimulationInformation SimulationInformation { get; set; }
        private readonly Handler Handler;
        
        
        private readonly Article[] _articles; 
 
        private double[] _simulationState; 
 
        private const int AvgLinesHour = 200; 
        private int avgLinesActual = 0; 
        private int avgLinesLeft; 
        private int LastHourSinceSent = 0; 
        
        public MiScheduler(int simFeatureCount, Article[] articles, SimulationInformation simulationInformation, OrderGenerator orderGenerator, Handler handler)
        {
            Handler = handler;
            
            _simulationState = new double[simFeatureCount]; 
            _articles = articles; 
            SimulationInformation = simulationInformation; 
 
            if (SimulationInformation.AreaInformation.Count == simFeatureCount) 
            { 
                // All Good, all features match in size 
            } 
            else 
            { 
                throw new IndexOutOfRangeException("Features and simulationFeatures needs to be the same size."); 
            } 
             
            weights = Enumerable.Repeat(1d, _articles.Length + simFeatureCount).ToArray(); 
            _actions = new Dictionary<Order, Sparse<int>>(); 
 
            // Sentinel waiting action. Must be checked for later 
            var waitOrder = new Order(0, DateTime.Now, new List<Line>()); 
            _actions.Add(waitOrder, StateRepresenter.MakeOrderRepresentation(waitOrder, _articles.ToList())); 
             
            UpdateAreaFeatures();

            orderGenerator.CostumerSendsOrderEvent += AddNewOrder;
            TimeKeeper.Tick += TimeActionToHandler;
        }

        private void UpdateAreaFeatures() 
        { 
            // Update the array to hold the updated with features 
            var areaFeatures = SimulationInformation.AreaInformation.Values.ToList(); 
 
            _simulationState = areaFeatures.ToArray(); 
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

        private void SendActionToHandler(Order order)
        {
            // Send to handler
            Handler.ReceiveOrder(order);
        }
        
        private void TimeActionToHandler()
        {
            // Select an action
            var actionOrder = ChooseAndReturnAction();

            // If action orderID != 0; Send
            if (actionOrder.OrderId != 0)
            {
                SendActionToHandler(actionOrder);
            }
            
            // Else return
        }

        public Order ChooseAndReturnAction()
        {
            var actionValues = CalculateAllActionValues(); 
            var bestAction = actionValues.OrderByDescending(kvp => kvp.Value).First().Key; 
            
            UpdateKnowledge(bestAction);

            if (bestAction.OrderId != 0)
            {
                _actions.Remove(bestAction);
            }

            return bestAction; 
        }

        private void UpdateKnowledge(Order order)
        {
            var reward = CalculateRewardValue(order); 
            var learningRate = 0.1;
            int weightsLength = weights.Length;
            
            for(int i = 0; i < weightsLength; i++)
            {
                // LEARN SOMETHING
                weights[i] += learningRate * reward;
            }
        }

        private double CalculateRewardValue(Order order) 
        { 
            // Reset the actual sent lines if an hour has passed. 
            if (LastHourSinceSent != TimeKeeper.CurrentDateTime.Hour) 
            { 
                avgLinesActual = 0; 
                LastHourSinceSent = TimeKeeper.CurrentDateTime.Hour; 
            } 
             
            // Calculate reward. 
            avgLinesLeft = AvgLinesHour - avgLinesActual; 
            var timeLeft = 60 - TimeKeeper.CurrentDateTime.Minute; 
 
            var shouldSendLines = avgLinesLeft / timeLeft; 
              
            var actualLinesSent = order.Lines.Count; 
 
            var reward = (double)actualLinesSent / (double)shouldSendLines; 
 
            avgLinesActual += actualLinesSent; 
             
            // Moving average 
            return reward; 
        } 
    }
}