using UnityEngine;
using System.Collections;
using System;

public class ShelfBox : MonoBehaviour, IDrawable {


    private string _articleNumber;
    private string _name;
    public string articleNumber
    {
        get { return _articleNumber; }
        set { _articleNumber = value; }
    }

    public string Name
    {
        get { return this._name; }

        set { this._name = value; }
    }


    public ShelfBox(string name)
    {
        _name = name;
    }
}
