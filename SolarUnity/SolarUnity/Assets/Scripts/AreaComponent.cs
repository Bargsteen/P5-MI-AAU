using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using UnityEngine;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes;

public class AreaComponent : MonoBehaviour, IDrawable {
    
    public List<Station> Stations = new List<Station>();
    public GameObject GO;
    public WarehouseManager WhManager;
    public AreaCode areaCode;
    private int _boxState = 0;


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
        WhManager.runner.Handler.Areas.Values.ToList().Find(x => x.AreaCode == areaCode).OnOrderBoxInAreaFinished += delegate
        {
            
            _boxState = -1;
            //GameObject.Find("GameManager").GetComponent<ConveyorBelt>().Transport(areaCode, );
        };

        WhManager.runner.Handler.Areas.Values.ToList().Find(x => x.AreaCode == areaCode).OnOrderBoxReceivedAtAreaEvent += delegate
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
                StartCoroutine(SetToActiveColour());
                _boxState = 0;
                break;
        }

        int activeOrders = 0;
        foreach (Station s in Stations)
        {
            activeOrders += s.OBPContainer.ToList().Count;
        }
        GO.transform.GetChild(2).GetChild(0).GetComponent<TextMesh>().text = activeOrders.ToString();

    }

}
