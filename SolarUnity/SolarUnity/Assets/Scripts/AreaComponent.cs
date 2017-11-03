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


    IEnumerator resetColour()
    {
        yield return new WaitForSeconds(0.3f);
        GO.GetComponent<Renderer>().material.color = Color.white;
    }


    void Start()
    {
        WhManager.runner.Areas.ToList().Find(x => x.AreaCode == areaCode).OnOrderBoxInAreaFinished += delegate
        {
            _boxState = -1;
        };

        WhManager.runner.Areas.ToList().Find(x => x.AreaCode == areaCode).OnOrderBoxReceivedAtAreaEvent += delegate
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
                StartCoroutine(resetColour());
                _boxState = 0;
                break;
            case 1:
                GO.GetComponent<Renderer>().material.color = Color.green;
                StartCoroutine(resetColour());
                _boxState = 0;
                break;
        }
    }

}
