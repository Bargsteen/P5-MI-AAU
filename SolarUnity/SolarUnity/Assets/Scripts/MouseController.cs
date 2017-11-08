using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour
{
    private Stack _level = new Stack();


    void Start()
    {
        _level.Push(new Tuple<int, Vector3>(0, Camera.main.transform.position));
    }

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {
				Transform objectHit = hit.transform;

                switch (objectHit.gameObject.tag)
                {
                    case "Area":
                        _level.Push(new Tuple<int, Vector3>(1, Camera.main.transform.position));
                        Camera.main.transform.position = hit.transform.position;
                        transform.GetComponent<WarehouseSetup>().DrawBoxes(objectHit.gameObject.GetComponent<AreaComponent>().Stations,
                            objectHit.transform.position,
                            WarehouseSetup.BoxTypes.Station,
                            transform.GetComponent<WarehouseSetup>().Stationtemplate,
                            hit.transform.gameObject);
                        break;

                    case "Station":
                        _level.Push(new Tuple<int, Vector3>(2, Camera.main.transform.position));
                        Camera.main.transform.position = hit.transform.position;
                        //transform.GetComponent<WarehouseSetup>().DrawBoxes(objectHit.GetComponent<StationComponent>().shelfBoxes.Cast<IDrawable>().ToList(),
                        //    objectHit.transform.position,
                        //    WarehouseSetup.BoxTypes.Shelfbox,
                        //    transform.GetComponent<WarehouseSetup>().ShelfBoxtemplate);

                        transform.GetComponent<WarehouseSetup>().DrawBoxes(objectHit.GetComponent<StationComponent>().OBPContainer.ToList(),
                            objectHit.transform.position,
                            WarehouseSetup.BoxTypes.Orderbox,
                            transform.GetComponent<WarehouseSetup>().OrderBoxtemplate,
                            hit.transform.gameObject);

                        objectHit.GetComponent<StationComponent>().IsActive = true;
                        break;

                }


            }
		} 

		if (Input.GetMouseButtonDown (1))
		{
		    if (_level.Count == 0)
		        return;
		    Tuple<int, Vector3> _currentLevel = (Tuple<int, Vector3>) _level.Pop();
		    Camera.main.transform.position = _currentLevel.Item2;

		    switch (_currentLevel.Item1)
		    {
		        case 2: 
		            foreach (GameObject GO in GameObject.FindGameObjectsWithTag("OrderBox"))
		            {
		                Destroy(GO);
		            }

		            foreach (GameObject GO in GameObject.FindGameObjectsWithTag("ShelfBox"))
		            {
		                Destroy(GO);
		            }

		            foreach (GameObject GO in GameObject.FindGameObjectsWithTag("Station"))
		            {
		                GO.GetComponent<StationComponent>().IsActive = false;
		            }
		            break;

		        case 1:
		            foreach (GameObject GO in GameObject.FindGameObjectsWithTag("Station"))
		            {
		                Destroy(GO);
		            }
		            break;
		    }

		}

	}
}
