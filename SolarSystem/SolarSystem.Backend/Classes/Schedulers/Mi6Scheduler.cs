using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.Classes.Schedulers
{
    public class Mi6Scheduler : Scheduler
    {
        private const int ReplayMemorySize = 100;
        private const int BatchSize = 1;
        private const double DiscountFactor = 0.3;
        private const double LearningRate = 0.01;
        private readonly int _weightCount;

        private readonly List<Article> _articles;

        private Queue<Memory> ReplayMemory { get; set; }
        private double[] Weights { get; set; }

        private readonly Dictionary<int, ActionData> _actionDataDict;

        private readonly SimulationInformation _simInfo;

        private static Random Random { get; } = new Random();
        
        public Mi6Scheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime, List<Article> articles, SimulationInformation simInfo) : base(orderGenerator, handler, poolMoverTime)
        {
            _simInfo = simInfo;
            _weightCount = articles.Count + _simInfo.GetState().Length;
            // Initialize replay memory to capacity ReplayMemorySize
            ReplayMemory = new Queue<Memory>();
            // Initialize a set of weights, theta, to 0
            Weights = Enumerable.Repeat(Random.NextDouble(), _weightCount).ToArray();
            
            // Sentinel waiting action. Must be checked for later 
            var waitOrder = new Order(0, DateTime.MinValue, new List<Line>()); 
            ActualOrderPool.Add(waitOrder);
            
            _articles = articles;

            var actionVector = StateRepresenter.MakeOrderRepresentation(waitOrder, articles);
            
            _actionDataDict = new Dictionary<int, ActionData>
            {
                {waitOrder.OrderId, new ActionData(waitOrder, actionVector, CalculateActionValue(actionVector, _simInfo.GetState()) ) }
            };
                
            OnOrderActuallySent += RemoveActionFromListIfActuallySent;

        }

        protected override Order ChooseNextOrder()
        {
            ActionData bestAction = _actionDataDict.OrderByDescending(kvp => kvp.Value.ActionValue).First().Value;

            // Get Reward
            var reward = _simInfo.GetReward();
            
            
            // Keep size ReplayMemorySize in ReplayMemory
            if (ReplayMemory.Count == ReplayMemorySize)
            {
                ReplayMemory.Dequeue();
            }
            
            // Store transition
            var newMemory = new Memory(_simInfo.GetState(), bestAction.ActionVector, reward);
            ReplayMemory.Enqueue(newMemory);
            
            
            Learn();
            
            return bestAction.Order;
        }

        protected override void RunFirstInTickLoop()
        {
            UpdateStateForLastAction();
        }

        private void Learn()
        {
            C5.HashSet<int> randomBatchIndexes = new C5.HashSet<int>();
            var replayMemorySize = ReplayMemory.Count;
            
            while (randomBatchIndexes.Count <= BatchSize && randomBatchIndexes.Count<= replayMemorySize)
            {
                randomBatchIndexes.Add(Random.Next(0, replayMemorySize));
            }
            
            // Sample mini-batch
            var randomBatch = randomBatchIndexes.Select(i => ReplayMemory.ElementAt(i));

            foreach (var memory in randomBatch)
            {
                
                var bestQ = double.MinValue;
                foreach (var action in _actionDataDict.Values)
                {
                    var actionValue = CalculateActionValue(action.ActionVector, memory.NextState);
                    if (actionValue > bestQ)
                    {
                        bestQ = actionValue;
                    }
                }
                
                var yk = memory.Reward + DiscountFactor * bestQ;
                var error = Math.Pow(yk - memory.Reward, 2);

                for (int i = 0; i < _weightCount; i++)
                {
                    Weights[i] = Weights[i] + LearningRate * error;
                }
            }
        }

        private void UpdateStateForLastAction()
        {
            if (!ReplayMemory.Any()) return;
            ReplayMemory.Last().NextState = _simInfo.GetState();
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
            // Else return
        }

        private void CalculateAndAddActionData(List<Order> initialOrderPool)
        {
            var stateVector = _simInfo.GetState();
            foreach (var order in initialOrderPool)
            {
                var actionVector = StateRepresenter.MakeOrderRepresentation(order, _articles);
                _actionDataDict.Add(order.OrderId, new ActionData(order, actionVector, CalculateActionValue(actionVector, stateVector)));
            }
        }

        private void RemoveActionFromListIfActuallySent(Order action)
        {
            _actionDataDict.Remove(action.OrderId);
        }

        private double CalculateActionValue(double[] actionVector, double[] stateVector)
        {
            var fullVector = actionVector.ToList();
            fullVector.AddRange(stateVector);
            
            return fullVector.Zip(Weights, (a, w) => a * w).Sum();
        }
    }

    internal class Memory
    {
        public Memory(double[] state, double[] actionVector, double reward)
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
        add (PickingOrder.Id : order, actionVector, actionValue) to orderDict

On Choose next PickingOrder:
    Choose best action from orderDict and try to send

On actually sent:
    Remove order from orderDict






*/