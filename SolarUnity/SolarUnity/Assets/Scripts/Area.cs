using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Area : MonoBehaviour, IDrawable
{
    public List<Station> stations = new List<Station>();
    private string _Name;

    public Area(string name, List<Station> stations)
    {
        _Name = name;
        this.stations = stations;
    }

    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }
}
