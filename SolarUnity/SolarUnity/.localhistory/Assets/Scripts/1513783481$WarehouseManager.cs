﻿using System.Collections;
using System.Collections.Generic;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes;
using UnityEditor;
using UnityEngine;

public class WarehouseManager : MonoBehaviour
{

    public Runner runner;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        RandomSeedType chosenSeedType = RandomSeedType.Fixed;
        SimulationState chosenSimulationState = SimulationState.Real;

        SimulationConfiguration.SeedType = chosenSeedType;
        SimulationConfiguration.SimulationState = chosenSimulationState;

        var filePath =
            Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                .ToString()) + "/SolarSystem.Backend/SolarData/";

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


        var runner = new Runner(filePath, simSpeed, randomNewOrderChance, orderGenerationConfiguration,
            schedulerType, hoursToSimulate, simulationStartTime, schedulerStartTime, orders, runsToDo);

        // var outPutter = new Outputter(runner);
        // var dataSaver = new DataSaver(runner);
        // SaveData(pickNScrape.OrderList);

        SaveOrderData sod = new SaveOrderData(runner.Handler);

        Statistics stats = new Statistics(ordersCopy, runner);

        runner.Start();

        var consoleStatusPrinter = new ConsoleStatusPrinter(runner, stats, schedulerStartTime, schedulerType, orderGenerationConfiguration);
        consoleStatusPrinter.StartPrinting();
    }
}