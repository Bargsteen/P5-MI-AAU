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
        private double[] Weights { get; set; }

        private readonly Dictionary<int, ActionData> _actionDataDict;

        private readonly SimulationInformation _simInfo;

        private static Random Random { get; } = new Random();
        
        public Mi2Scheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime, List<Article> articles, SimulationInformation simInfo) : base(orderGenerator, handler, poolMoverTime)
        {
            _simInfo = simInfo;
            WeightCount = articles.Count + _simInfo.GetState().Length;
            // Initialize replay memory to capacity N
            ReplayMemory = new List<Memory>();
            // Initialize a set of weights, theta, to 0
            Weights = Enumerable.Repeat(Random.NextDouble(), WeightCount).ToArray();
            
            // Sentinel waiting action. Must be checked for later 
            var waitOrder = new Order(0, DateTime.MinValue, new List<Line>()); 
            ActualOrderPool.Add(waitOrder);
            
            _articles = articles;

            var actionVector = StateRepresenter.MakeOrderRepresentation(waitOrder, articles);
            
            _actionDataDict = new Dictionary<int, ActionData>
            {
                {waitOrder.OrderId, new ActionData(waitOrder, actionVector, CalculateActionValue(actionVector) ) }
            };
                
            OnOrderActuallySent += RemoveActionFromListIfActuallySent;

        }

        protected override Order ChooseNextOrder()
        {
            ActionData bestAction = _actionDataDict.OrderByDescending(kvp => kvp.Value.ActionValue).First().Value;

                  

            
            // Get Reward
            var reward = _simInfo.GetReward();
            // Store transition
            
            var newMemory = new Memory(_simInfo.GetState(), bestAction.ActionVector, reward);
            ReplayMemory.Add(newMemory);
            
            // Sample mini batch and learn with gradient descent
            return bestAction.Order;
        }


        protected override void MoveInitialToActualPool()
        {
            // If the time has passed, and there is something to move => move.
            if (TimerStartMinutes <= PoolTimer++)
            {
                CalculateAndAddActionData(InitialOrderPool);
                
                ActualOrderPool.AddRange(InitialOrderPool);
                InitialOrderPool.Clear();
                
                // Reset timer
                PoolTimer = 0;
            }
            // Else return.
        }

        private void CalculateAndAddActionData(List<Order> initialOrderPool)
        {
            foreach (var order in initialOrderPool)
            {
                var actionVector = StateRepresenter.MakeOrderRepresentation(order, _articles);
                _actionDataDict.Add(order.OrderId, new ActionData(order, actionVector, CalculateActionValue(actionVector)));
            }
        }

        private void RemoveActionFromListIfActuallySent(Order action)
        {
            _actionDataDict.Remove(action.OrderId);
        }

        private double CalculateActionValue(double[] actionVector)
        {
            var fullVector = actionVector.ToList();
            fullVector.AddRange(_simInfo.GetState());
            
            return fullVector.Zip(Weights, (a, w) => a * w).Sum();
        }
    }

    internal struct Memory
    {
        public Memory(double[] state, double[] actionVector, double reward) : this()
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            ActionVector = actionVector ?? throw new ArgumentNullException(nameof(actionVector));
            Reward = reward;
        }

        public double[] State { get; set; }
        public double[] ActionVector { get; set; }
        public double Reward { get; set; }
        public double[] NextState { get; set; }
    }

    internal struct ActionData
    {
        public Order Order;
        public double[] ActionVector;
        public double ActionValue;

        public ActionData(Order order, double[] actionVector, double actionValue)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            ActionVector = actionVector ?? throw new ArgumentNullException(nameof(actionVector));
            ActionValue = actionValue;
        }
    }
}


/*
On move to actual pool:
    get stateVector (simState)
    For each new order from initialPool:
        create the actionVector
        combine actionVector and simState to fullVector
        calculate actionValue by dotting fullVector with weights
        add (Order.Id : order, actionVector, actionValue) to orderDict

On Choose next Order:
    Choose best action from orderDict and try to send

On actually sent:
    Remove order from orderDict






*/