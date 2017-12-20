using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using UnityEngine;

namespace Assets.Scripts
{
    public class StationComponent : MonoBehaviour, IDrawable {

        public WarehouseManager WhManager;
        public AreaCode areaCode;
        public int stationNumber;
        private int _boxState = 0;
        public GameObject GO;
        public List<OrderBox> Orderboxes;
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

        
            WhManager.Runner.Handler.Areas.Values.ToList().Find(x => x.AreaCode == areaCode).Stations.ToList()[stationNumber].OnOrderBoxFinishedAtStation += delegate
            {
                _boxState = -1;
            };

            WhManager.Runner.Handler.Areas.Values.ToList().Find(x => x.AreaCode == areaCode).Stations.ToList()[stationNumber].OnOrderBoxReceivedAtStation += delegate
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

            if (IsActive) {
                foreach (GameObject GO in GameObject.FindGameObjectsWithTag("OrderBox"))
                {
                    Destroy(GO);
                }
                GameObject.Find("GameManager").GetComponent<WarehouseSetup>().DrawBoxes(
                    Orderboxes,
                    transform.position,
                    WarehouseSetup.BoxTypes.Orderbox,
                    GameObject.Find("GameManager").GetComponent<WarehouseSetup>().OrderBoxtemplate,
                    gameObject);
                GameObject.Find("GameManager").GetComponent<WarehouseSetup>().CurrentArea = areaCode;
            }



            GO.transform.GetChild(2).GetChild(0).GetComponent<TextMesh>().text = Orderboxes.Count.ToString();
        }


    }
}
