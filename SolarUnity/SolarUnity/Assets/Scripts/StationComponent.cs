using System.Collections;
using System.Collections.Generic;
using SolarSystem.Backend.Classes;
using UnityEngine;
using System.Linq;

public class StationComponent : MonoBehaviour, IDrawable {

    public List<OrderBox> orderBoxes = new List<OrderBox>();
    public WarehouseManager WhManager;
    public AreaCode areaCode;
    public int stationNumber;
    private int _boxState = 0;
    public GameObject GO;



    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(0.3f);
        GO.GetComponent<Renderer>().material.color = Color.white;
    }




    void Start()
    {
        WhManager.runner.Areas.ToList().Find(x => x.AreaCode == areaCode).Stations.ToList()[stationNumber].OnOrderBoxFinished += delegate
        {
            _boxState = -1;
        };

        WhManager.runner.Areas.ToList().Find(x => x.AreaCode == areaCode).Stations.ToList()[stationNumber].OnOrderBoxReceivedAtStationEvent += delegate
        {
            _boxState = 1;
        };
    }




    void Update()
    {
        switch (_boxState)
        {
            case -1:
                GO.GetComponent<Renderer>().material.color = Color.red;
                StartCoroutine(ResetColour());
                _boxState = 0;
                break;
            case 1:
                GO.GetComponent<Renderer>().material.color = Color.green;
                StartCoroutine(ResetColour());
                _boxState = 0;
                break;
        }
    }


}
