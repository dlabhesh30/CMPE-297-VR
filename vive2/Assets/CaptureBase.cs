using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureBase : MonoBehaviour {

    public int team;

	// Use this for initialization
	void Start () {
        //team = 1;
        if (tag == "VR Player's Unit")
        {
            team = 1;
        }
        if (tag == "PC Player's Unit")
        {
            team = 2;
        }
	}

    // Update is called once per frame
    void Update() {
        //GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject[] flags = GameObject.FindGameObjectsWithTag("Flag");

        GameObject nearestFlag = null;

        float smallestDist = float.MaxValue;

        for (int i = 0; i < flags.Length; i++)
        {
            GameObject currentFlag = flags[i];
            float dist;
            dist = Vector3.Distance(currentFlag.transform.position, transform.position);
            if (dist < smallestDist && dist < 2.5)
            {
                smallestDist = dist;
                nearestFlag = currentFlag;
            }
        }

        if (nearestFlag != null)
        {
            nearestFlag.BroadcastMessage("Capture", team);
        }
    }
}
