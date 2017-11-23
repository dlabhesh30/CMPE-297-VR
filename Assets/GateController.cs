using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour {

    public BucketGridController bucketGrid;

	// Use this for initialization
	void Start () {
        bucketGrid = GameObject.FindGameObjectWithTag("BucketGridController").GetComponent<BucketGridController>();
        //bucketGrid.get
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
