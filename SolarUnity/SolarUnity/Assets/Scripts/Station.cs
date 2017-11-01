using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Station : MonoBehaviour, IDrawable {

    private string _Name;
    public List<OrderBox> orderBoxes = new List<OrderBox>();
    public List<ShelfBox> shelfBoxes = new List<ShelfBox>();

    public int maxShelfBoxes;
    public int maxOrderBoxes;

    public string Name
    {
        get { return this._Name; }

        set { this._Name = value; }
    }

    public Station(string name, int maxShelfBoxes, int maxOrderBoxes, List<OrderBox> orderBoxes, List<ShelfBox> shelfBoxes)
    {
        this._Name = name;
        this.maxShelfBoxes = maxShelfBoxes;
        this.maxOrderBoxes = maxOrderBoxes;
        this.orderBoxes = orderBoxes;
        this.shelfBoxes = shelfBoxes;
    }



}
