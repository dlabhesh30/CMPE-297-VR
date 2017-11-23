using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBuildingSelector : MonoBehaviour {

    // Use this for initialization

    List<Transform> selectObject;

	void Start ()
    {
        selectObject = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            selectObject.Add(transform.GetChild(i));
            float dir = -i * (360 / transform.childCount) + 90;
            float radDir = (dir) * Mathf.Deg2Rad;
            selectObject[i].localPosition = new Vector3(Mathf.Cos(radDir) * .1f, Mathf.Sin(radDir) * .1f, 0);
            selectObject[i].localRotation = Quaternion.Euler(0, 0, dir - 90);
            
        }
    }
	
	// Update is called once per frame
	void Update () {

    }
}
