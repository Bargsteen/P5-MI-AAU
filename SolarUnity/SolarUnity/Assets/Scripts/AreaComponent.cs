using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SolarSystem.Backend;
using SolarSystem.Backend.Classes;

public class AreaComponent : MonoBehaviour, IDrawable {
    
    public List<Station> Stations = new List<Station>();
    public List<GameObject> StationGOs = new List<GameObject>();

}
