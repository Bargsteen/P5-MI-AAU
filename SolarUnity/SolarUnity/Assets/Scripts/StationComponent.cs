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
    public OrderboxProgressContainer OBPContainer;
    public bool IsActive;


    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(1);
            GO.GetComponent<Renderer>().material.color = Color.white;
    }


    IEnumerator SetToActiveColour()
    {
        yield return new WaitForSeconds(1);
        GO.GetComponent<Renderer>().material.color = Color.yellow;
    }



    void Start()
    {
        WhManager.runner.Handler.Areas.Values.ToList().Find(x => x.AreaCode == areaCode).Stations.ToList()[stationNumber].OnOrderBoxFinished += delegate
        {
            _boxState = -1;
        };

        WhManager.runner.Handler.Areas.Values.ToList().Find(x => x.AreaCode == areaCode).Stations.ToList()[stationNumber].OnOrderBoxReceivedAtStationEvent += delegate
        {
            _boxState = 1;
            Debug.Log("Area: " + areaCode + " Station: " + stationNumber);
            Debug.Log(OBPContainer.GetNext().OrderBox.Id);
            Debug.Log("Done with station : " + stationNumber + "\n\n\n\n");
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
                StartCoroutine(SetToActiveColour());
                _boxState = 0;
                break;
        }

        if (IsActive) {
            foreach (GameObject GO in GameObject.FindGameObjectsWithTag("OrderBox"))
            {
                Destroy(GO);
            }
            GameObject.Find("GameManager").GetComponent<WarehouseSetup>().DrawBoxes(
            OBPContainer.ToList(),
            transform.position,
            WarehouseSetup.BoxTypes.Orderbox,
            GameObject.Find("GameManager").GetComponent<WarehouseSetup>().OrderBoxtemplate,
            gameObject);
        }



        GO.transform.GetChild(2).GetChild(0).GetComponent<TextMesh>().text = OBPContainer.ToList().Count.ToString();
    }


}
