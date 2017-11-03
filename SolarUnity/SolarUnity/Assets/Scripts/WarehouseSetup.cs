using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework.Interfaces;
using Ploeh.AutoFixture;
using SolarSystem;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes;
using Area = SolarSystem.Backend.Classes.Area;

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

    // Use this for initialization
    void Start () {
        
        Runner420 runner = new Runner420();

        DontDestroyOnLoad(this);
	    Camera cam = Camera.main;
	    height = 2f * cam.orthographicSize;
	    width = height * cam.aspect;


        Camera.main.pixelRect = new Rect(0, 0, 800, 800);
        Constants c = this.GetComponent<Constants>();
        // c.Setup();


        Debug.Log((Camera.main.transform.position));

        Areas = DrawBoxes (runner.Areas.Values.ToList(), Camera.main.transform.position, BoxTypes.Area, Areatemplate);





	}


    public List<GameObject> DrawBoxes<T>(List<T> boxes, Vector3 Origin, BoxTypes type, GameObject template)
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
                    box.transform.GetChild(0).GetComponent<TextMesh>().text = (boxes[i] as Area).AreaCode.ToString();
                    box.gameObject.GetComponent<AreaComponent>().Stations = (boxes[i] as Area).Stations.ToList();
                    box.tag = "Area";
                    break;

                case BoxTypes.Station:
                    box.transform.GetChild(0).GetComponent<TextMesh>().text = (boxes[i] as Station).Name;
                    box.gameObject.GetComponent<StationComponent>().orderBoxes = (boxes[i] as Station).OrderBoxes.ToList();
                    box.tag = "Station";
                    break;

                case BoxTypes.Shelfbox:
                    box.transform.GetChild(0).GetComponent<TextMesh>().text = (boxes[i] as ShelfBox).Id;
                    box.tag = "ShelfBox";
                    break;


                case BoxTypes.Orderbox:
                    box.transform.GetChild(0).GetComponent<TextMesh>().text = (boxes[i] as OrderBox).Order.OrderId;
                    box.tag = "OrderBox";
                    break;
            }


            Boxes.Add(box);
        }




        return Boxes;
    }



    // Update is called once per frame

}
