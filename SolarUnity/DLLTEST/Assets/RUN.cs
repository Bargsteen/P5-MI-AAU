using UnityEngine;
using System.Collections;
using UnityLibraryTest;

public class RUN : MonoBehaviour {
    Class1 ollie = new Class1();
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        ollie.counter();
        Debug.Log(ollie.count);
    }
}
