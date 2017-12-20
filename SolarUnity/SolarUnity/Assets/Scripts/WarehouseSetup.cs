using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend;
using UnityEngine;
using SolarSystem.Backend.Classes.Simulation;

namespace Assets.Scripts
{
    public class WarehouseSetup : MonoBehaviour
    {
        //Input
        //Boxes & Position
        //Flow
        //Listen to events


        // Instantiate Areas

        // Area.cs
        //Liste af stationer

        // Instantiate station.cs
        //Liste af orderboxes
        //Liste af shelfboxes	

        // Instantiate orderboxes og Shelfboxes
        //Shelfboxes skal have line
    

        public enum BoxTypes
        {
            Area,
            Station,
            Orderbox,
            Shelfbox
        }
    
        private float height;
        private float width;
        public List<GameObject> Areas = new List<GameObject>();


        public GameObject Areatemplate;
        public GameObject Stationtemplate;
        public GameObject ShelfBoxtemplate;
        public GameObject OrderBoxtemplate;
        private Runner runner;

        public AreaCode CurrentArea;

        void Start ()
        {
            GetComponent<WarehouseManager>().ManagerReady += delegate { 
                runner = GetComponent<WarehouseManager>().Runner;

                DontDestroyOnLoad(this);
                Camera cam = Camera.main;
                height = 2f * cam.orthographicSize;
                width = height * cam.aspect;


                Camera.main.pixelRect = new Rect(0, 0, 800, 800);
                Constants c = this.GetComponent<Constants>();
                // c.Setup();


                Debug.Log((Camera.main.transform.position));
                Areas = DrawBoxes (runner.Handler.Areas.Values.ToList(), Camera.main.transform.position, BoxTypes.Area, Areatemplate);
            };


        }



        void Update()
        {
            Debug.Log(TimeKeeper.CurrentDateTime);
        }


        public List<GameObject> DrawBoxes<T>(List<T> boxes, Vector3 Origin, BoxTypes type, GameObject template, GameObject parent = null)
        {
            List<GameObject> Boxes = new List<GameObject>();

            float boxWidth = template.GetComponent<BoxCollider>().size.x * template.transform.localScale.x;
            float boxHeight = template.GetComponent<BoxCollider>().size.z * template.transform.localScale.z;


            int amountOfBoxesOnX = (int)(width / boxWidth);
            float AreaBoxMarginX = (width - amountOfBoxesOnX * boxWidth) / (amountOfBoxesOnX + 1);
            int linecount = Mathf.CeilToInt((float)boxes.Count / amountOfBoxesOnX);
            float AreaBoxMarginZ = (height - linecount * boxHeight) / (linecount + 1);



            float x = ((Origin.x - width / 2) + boxWidth / 2 + AreaBoxMarginX);
            float z = ((Origin.z + height / 2) - boxHeight / 2) - AreaBoxMarginZ;


            if (type == BoxTypes.Shelfbox)
            {
                z = ((Origin.z + height / 2) - boxHeight / 2) - 0.5f;
            }

            if (type == BoxTypes.Orderbox)
            {
                AreaBoxMarginZ = ((height - ShelfBoxtemplate.transform.localScale.z) - (linecount + 1) * boxHeight) / (linecount + 2);
                x = ((Origin.x - width / 2) + boxWidth / 2 + AreaBoxMarginX);
                z = (((Origin.z + height / 2) - boxHeight) - AreaBoxMarginZ * 2) - ShelfBoxtemplate.GetComponent<BoxCollider>().size.z * ShelfBoxtemplate.transform.localScale.z;
            }




            int count = 0;

            for (int i = 0; i < boxes.Count; i++)
            {
                count++;
                if (count > amountOfBoxesOnX)
                {
                    count = 0;
                    x = ((Origin.x - width / 2) + boxWidth / 2 + AreaBoxMarginX);
                    z -= boxHeight + AreaBoxMarginZ;
                }

                GameObject box = (GameObject)Instantiate(template, new Vector3(
                        x,
                        Origin.y - 1,
                        z),
                    template.transform.rotation);
                box.transform.position = new Vector3(x, Origin.y - 1, z);
                x += boxWidth + AreaBoxMarginX;


                switch (type)
                {
                    case BoxTypes.Area:
                        AreaComponent areaComponent = box.GetComponent<AreaComponent>();
                        box.transform.GetChild(0).GetComponent<TextMesh>().text = (boxes[i] as Area).AreaCode.ToString();
                        areaComponent.Stations = (boxes[i] as Area).Stations.ToList();
                        areaComponent.WhManager = GetComponent<WarehouseManager>();
                        areaComponent.GO = box;
                        areaComponent.areaCode = (boxes[i] as Area).AreaCode;
                        box.tag = "Area";
                        break;

                    case BoxTypes.Station:
                        StationComponent stationComponent = box.GetComponent<StationComponent>();
                        box.transform.GetChild(0).GetComponent<TextMesh>().text = (boxes[i] as Station)?.Name.Split('+')[1];
                        stationComponent.Orderboxes = (boxes[i] as Station)?.OrderBoxes.ToList();
                        stationComponent.GO = box;
                        stationComponent.areaCode = parent.GetComponent<AreaComponent>().areaCode;
                        stationComponent.stationNumber = i;
                        stationComponent.WhManager = GetComponent<WarehouseManager>();
                        stationComponent.Orderboxes = (boxes[i] as Station)?.OrderBoxes.ToList();
                        box.tag = "Station";
                        break;

                    case BoxTypes.Shelfbox:
                        box.transform.GetChild(0).GetComponent<TextMesh>().text = (boxes[i] as ShelfBox)?.Id.ToString();
                        box.tag = "ShelfBox";
                        break;


                    case BoxTypes.Orderbox:
                        box.transform.GetChild(0).GetComponent<TextMesh>().text = (boxes[i] as OrderBox)?.Order.OrderId.ToString();
                        

                        var linesNeededInArea = 0;
                        var lineIsPickedStatuses = (boxes[i] as OrderBox)?.LineIsPickedStatuses;
                        if (lineIsPickedStatuses != null)
                            for (var lIndex = 0; lIndex < lineIsPickedStatuses.Count; lIndex++)
                            {
                                var line = lineIsPickedStatuses.Keys.ToList()[lIndex];
                                if (line.Article.AreaCode == CurrentArea)
                                {
                                    linesNeededInArea++;
                                }
                            }
                        box.transform.GetChild(2).GetComponent<TextMesh>().text = linesNeededInArea.ToString();

                        box.transform.GetChild(4).GetComponent<TextMesh>().text =
                            (boxes[i] as OrderBox)?.LinesNotPickedIn(CurrentArea).Count.ToString();
                        box.tag = "OrderBox";
                        break;
                }


                Boxes.Add(box);
            }




            return Boxes;
        }



        // Update is called once per frame

    }
}
