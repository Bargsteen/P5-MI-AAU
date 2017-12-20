using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;


public class UIManager : MonoBehaviour
{
    private bool _showMenu;
    private bool _breakingOnCongestion;
    public MapColourTo CurrentMapping = MapColourTo.Actions;
    private WarehouseManager _whManager;
    public enum MapColourTo
    {
        Throughput,
        Orderboxes,
        Actions
    }

	// Use this for initialization
	void Start ()
	{
	    _whManager = GetComponent<WarehouseManager>();
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 40, 40), "..."))
        {
            _showMenu = !_showMenu;
        }


        if (_showMenu)
        {
            string s = "break on congestion";
            if (_breakingOnCongestion)
                s = "!break on congestion";
            if (GUI.Button(new Rect(50, 0, 160, 40), s))
            {
                _breakingOnCongestion = !_breakingOnCongestion;
            }

            if (GUI.Button(new Rect(220, 0, 150, 40), "Map to " + CurrentMapping.ToString()))
            {

                int a = System.Array.IndexOf(System.Enum.GetValues(CurrentMapping.GetType()), CurrentMapping);
                CurrentMapping =
                    (MapColourTo) (Enum.GetValues(CurrentMapping.GetType())).GetValue(
                        a == Enum.GetNames(typeof(MapColourTo)).Length - 1 ? 0 : ++a);

            }
        }
        else
        {
            GUI.Label(new Rect(60, 10, 100, 125), "Mainloop: " + _whManager.runner.Handler.MainLoop.BoxesInMainLoop);
        }


    }

	// Update is called once per frame
	void Update () {
		
	}
}
