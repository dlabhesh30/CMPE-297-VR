using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObj : MonoBehaviour {

    public bool isColliding = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter()
    {
        isColliding = true;
    }

    void OnTriggerExit()
    {
        isColliding = false;
    }

    public bool colliding()
    {
        return isColliding;
    }
}
