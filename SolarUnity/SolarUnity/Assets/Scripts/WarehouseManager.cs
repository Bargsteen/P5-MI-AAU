using System.Collections;
using System.Collections.Generic;
using SolarSystem.Backend;
using UnityEngine;

public class WarehouseManager : MonoBehaviour {

    public Runner runner = new Runner();
    // Use this for initialization
    void Start () {
		runner.StartSendingOrders();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
