using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using UnityEngine;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using SolarSystem.Backend;


public class AreaComponent : MonoBehaviour, IDrawable {
    
    public List<Station> Stations = new List<Station>();
    public GameObject GO;
    public WarehouseManager WhManager;
    public AreaCode areaCode;
    private int _boxState = 0;
    private Color _mappedColour;
    public UIManager UIM;
    private int _maxOrderBoxesForStation;
    private bool _runUpdate = false;

    float map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }


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
            _runUpdate = true;
            _maxOrderBoxesForStation = Stations.Capacity;
            UIM = WhManager.GetComponent<UIManager>();
            WhManager.runner.Handler.Areas.Values.ToList().Find(x => x.AreaCode == areaCode).OnOrderBoxInAreaFinished += delegate
            {
                _boxState = -1;
            };

            WhManager.runner.Handler.Areas.Values.ToList().Find(x => x.AreaCode == areaCode).OnOrderBoxReceivedAtAreaEvent += delegate
            {
                _boxState = 1;
            };
    }


    void Update()
    {
        if (_runUpdate == false)
            return;

        int activeOrders = 0;
        foreach (Station s in Stations)
        {
            activeOrders += s.OrderBoxes.ToList().Count;
        }
        GO.transform.GetChild(2).GetChild(0).GetComponent<TextMesh>().text = activeOrders.ToString();

        if(UIM == null)
        Debug.LogError("No UI Manager Available");

        switch (UIM.CurrentMapping)
        {
            case UIManager.MapColourTo.Orderboxes:
                _maxOrderBoxesForStation = 0;
                int currentAmountOfOrderBoxes = 0;
                foreach (Station s in Stations)
                {
                    currentAmountOfOrderBoxes += s.OrderBoxes.Count();
                    _maxOrderBoxesForStation += 5;
                }


                float red = map(_maxOrderBoxesForStation - currentAmountOfOrderBoxes, 0, _maxOrderBoxesForStation, 0, 1);
                float green = map(currentAmountOfOrderBoxes, 0, _maxOrderBoxesForStation, 0, 1);

                _mappedColour = new Color(red, green, 0);

                GetComponent<Renderer>().material.color = _mappedColour;

                break;

            case UIManager.MapColourTo.Actions:
                switch (_boxState)
                {
                    case -1:
                        GO.GetComponent<Renderer>().material.color = Color.red;
                        if (Stations.Exists(x => x.OrderBoxes.Count() > 0))
                            StartCoroutine(SetToActiveColour());
                        else
                            StartCoroutine(ResetColour());
                        _boxState = 0;
                        break;
                    case 1:
                        GO.GetComponent<Renderer>().material.color = Color.green;
                        StartCoroutine(SetToActiveColour());
                        _boxState = 0;
                        break;
                }
                break;

        }


    }

}
