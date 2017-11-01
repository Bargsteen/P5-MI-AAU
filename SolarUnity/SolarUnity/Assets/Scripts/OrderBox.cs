using UnityEngine;
using System.Collections;
using System;

public class OrderBox : MonoBehaviour, IDrawable {


	private DateTime _creationTime;
    private string _name;

	public DateTime creationTime {
		get { return _creationTime; }
		set{ _creationTime = creationTime; }
	}

    public string Name
    {
        get { return this._name; }

        set { this._name = value; }
    }

    public OrderBox(string name)
    {
        _name = name;
    }

}
