using System;
using System.Collections.Generic;
using System.IO;
using SolarSystem.Backend;
using SolarSystem.Backend.Extraction;
using SolarSystem.Backend.Solution.Simulation;
using UnityEngine;

namespace Assets.Scripts
{
    public class WarehouseManager : MonoBehaviour
    {

        public event Action ManagerReady;
        public Runner Runner;

        // Use this for initialization
        void Start () {
        }
	
        // Update is called once per frame
        void Update () {

            RandomSeedType chosenSeedType = RandomSeedType.Fixed;
            SimulationState chosenSimulationState = SimulationState.Real;

            SimulationConfiguration.SeedType = chosenSeedType;
            SimulationConfiguration.SimulationState = chosenSimulationState;

            var filePath = "Assets/SolarData/";

            var pickNScrape = new PickingExtraction(filePath + "Picking 02-10-2017.csv");
            pickNScrape.GetOrdersFromPicking();

            var orders = pickNScrape.OrderList;
            var ordersCopy = new List<PickingOrder>();
            ordersCopy.AddRange(orders);

            const int simSpeed = 10000;
            const double randomNewOrderChance = 0.1;
            const OrderGenerationConfiguration orderGenerationConfiguration = OrderGenerationConfiguration.FromFile;

            const SchedulerType schedulerType = SchedulerType.Real;
            const int hoursToSimulate = 22;
            const int runsToDo = 1;
            DateTime simulationStartTime = new DateTime(2017, 10, 2, 6, 0, 0); //02/10/2017
            DateTime schedulerStartTime = simulationStartTime.AddHours(4);


            Runner = new Runner(filePath, simSpeed, randomNewOrderChance, orderGenerationConfiguration,
                schedulerType, hoursToSimulate, simulationStartTime, schedulerStartTime, orders, runsToDo);

            // var outPutter = new Outputter(runner);
            // var dataSaver = new DataSaver(runner);
            // SaveData(pickNScrape.OrderList);

            Runner.Start();
            ManagerReady?.Invoke();
        }
    }
}
