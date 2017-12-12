using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public class MiScheduler
    { 
        private readonly double[] _weights;

        private readonly Dictionary<Order, double[]> _actions;

        private SimulationInformation SimulationInformation { get; }
        private readonly Handler _handler;
        
        
        private readonly Article[] _articles; 
 
        private double[] _simulationState; 
 
        private const int AvgLinesHour = 1400; 
        private int _avgLinesActual; 
        private int _avgLinesLeft; 
        private int _lastHourSinceSent; 
        
        public MiScheduler(int simFeatureCount, Article[] articles, SimulationInformation simulationInformation, 
            OrderGenerator orderGenerator, Handler handler)
        {
            _handler = handler;
            
            _simulationState = new double[simFeatureCount]; 
            _articles = articles; 
 
            if (SimulationInformation.AreaInformation.Count == simFeatureCount) 
            { 
                // All Good, all features match in size 
            } 
            else 
            { 
                throw new IndexOutOfRangeException("Features and simulationFeatures needs to be the same size."); 
            } 
             
            _weights = Enumerable.Repeat(1d, _articles.Length + simFeatureCount).ToArray(); 
            _actions = new Dictionary<Order, double[]>(); 
 
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
                double[] denseActionDoubles = action.Value;
                
                // Merge with simulation state vector
                var fullStateVector = new List<double>();
                fullStateVector.AddRange(denseActionDoubles);
                fullStateVector.AddRange(_simulationState);
                
                // Calculate single action value and add to dict
                actionAndValue.Add(action.Key, CalculateSingleActionValue(fullStateVector.ToArray(), _weights));
            }
            // return dict
            return actionAndValue;
        }

        private void SendActionToHandler(Order order)
        {
            // Send to handler
            _handler.ReceiveOrder(order);
        }
        
        private void TimeActionToHandler()
        {
            if (_actions.Any())
            {
                // Select an action
                var actionOrder = ChooseAndReturnAction();

                // If action orderID != 0; Send
                if (actionOrder.OrderId != 0)
                {
                    SendActionToHandler(actionOrder);
                }
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
            const double learningRate = 0.1;
            int weightsLength = _weights.Length;
            
            for(int i = 0; i < weightsLength; i++)
            {
                // LEARN SOMETHING
                _weights[i] += learningRate * reward;
            }
        }

        private double CalculateRewardValue(Order order) 
        { 
            // Reset the actual sent lines if an hour has passed. 
            if (_lastHourSinceSent != TimeKeeper.CurrentDateTime.Hour) 
            { 
                _avgLinesActual = 0; 
                _lastHourSinceSent = TimeKeeper.CurrentDateTime.Hour; 
            } 
             
            // Calculate reward. 
            _avgLinesLeft = AvgLinesHour - _avgLinesActual; 
            var timeLeft = 60 - TimeKeeper.CurrentDateTime.Minute; 
 
            var shouldSendLines = _avgLinesLeft / timeLeft; 
              
            var actualLinesSent = order.Lines.Count; 
 
            var reward = actualLinesSent / (double)shouldSendLines; 
 
            _avgLinesActual += actualLinesSent; 
             
            // Moving average 
            return reward; 
        } 
    }
}