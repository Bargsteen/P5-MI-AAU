using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetMouseButtonDown(0)){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {
				Transform objectHit = hit.transform;

				switch (objectHit.gameObject.tag) {
                    case "Area":
					Camera.main.transform.position = hit.transform.position;
				    transform.GetComponent<WarehouseSetup>().DrawBoxes(objectHit.gameObject.GetComponent<Area>().stations.Cast<IDrawable>().ToList(),
                        objectHit.transform.position, 
                        WarehouseSetup.BoxTypes.Station,
                        transform.GetComponent<WarehouseSetup>().Stationtemplate);
					break;

                    case "Station":
                        Camera.main.transform.position = hit.transform.position;
                        transform.GetComponent<WarehouseSetup>().DrawBoxes(objectHit.GetComponent<Station>().shelfBoxes.Cast<IDrawable>().ToList(),
                            objectHit.transform.position,
                            WarehouseSetup.BoxTypes.Shelfbox,
                            transform.GetComponent<WarehouseSetup>().ShelfBoxtemplate);

                        transform.GetComponent<WarehouseSetup>().DrawBoxes(objectHit.GetComponent<Station>().orderBoxes.Cast<IDrawable>().ToList(),
                            objectHit.transform.position,
                            WarehouseSetup.BoxTypes.Orderbox,
                            transform.GetComponent<WarehouseSetup>().OrderBoxtemplate);
                        break;

				}


			}
		} 

		if (Input.GetMouseButtonDown (1)) {
			Camera.main.transform.position = new Vector3 (0, 10, 0);
		    foreach (GameObject GO in GameObject.FindGameObjectsWithTag("Station"))
		    {
		        Destroy(GO);
		    }

		    foreach (GameObject GO in GameObject.FindGameObjectsWithTag("OrderBox"))
		    {
		        Destroy(GO);
		    }

		    foreach (GameObject GO in GameObject.FindGameObjectsWithTag("ShelfBox"))
		    {
		        Destroy(GO);
		    }
        }

	}
}
