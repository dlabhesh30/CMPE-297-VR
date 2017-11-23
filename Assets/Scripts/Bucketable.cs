using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucketable : MonoBehaviour {

    public int bucketX = 0, bucketZ = 0;

    public bool positionSet = false;

    BucketGridController bucketGridController;
    // Use this for initialization
	void Start () {
        bucketGridController = GameObject.FindGameObjectWithTag("BucketGridController").transform.GetComponent<BucketGridController>();
        bucketGridController.addBucketable(transform);
    }
	
	// Update is called once per frame
	//void Update () {
		
	//}
    
    void OnDestroy()
    {
        if (transform != null)
        {
            GameObject bucketGridControllerGameObject = GameObject.FindGameObjectWithTag("BucketGridController");
            if (bucketGridControllerGameObject != null)
            {
                bucketGridController = bucketGridControllerGameObject.transform.GetComponent<BucketGridController>();
                bucketGridController.removeBucketable(transform);
            }
        }
    }

    public void setBucketPosition(int bx, int bz)
    {
        bucketX = bx;
        bucketZ = bz;
        positionSet = true;
    }
}
