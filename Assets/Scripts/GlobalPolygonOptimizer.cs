using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GlobalPolygonOptimizer : MonoBehaviour {

    public int maxHighPolyDistance = 15; //default 15

    List<Transform> objectsToOptimize;
    
    int objectToOptimize, timesUntilListUpdate;
    int pcBucketableToOptimize, vrBucketableToOptimize;

    public Transform bucketGridController;

    public Transform pcCamera, vrCamera;

    BucketGridController bucketGrid;

    float bucketWidth, bucketHeight;

    //GameObject[][] 

    void Awake()
    {
        bucketGridController = GameObject.FindGameObjectWithTag("BucketGridController").transform;
        bucketGrid = bucketGridController.GetComponent<BucketGridController>();
        objectsToOptimize = new List<Transform>(); //DistancePolyOptimizeInit
        objectToOptimize = 0;
        pcBucketableToOptimize = 0;
        vrBucketableToOptimize = 0;
    }

    // Use this for initialization
    void Start () {

        bucketWidth = bucketGridController.GetComponent<BucketGridController>().bucketWidth;
        bucketHeight = bucketGridController.GetComponent<BucketGridController>().bucketHeight;

        /*
        ArrayList firstDimension = new ArrayList();
        ArrayList secondDimensionChild = new ArrayList();
        ArrayList thirdDimensionChild = new ArrayList();
        firstDimension.Add(secondDimensionChild);
        secondDimensionChild.Add(thirdDimensionChild);
        thirdDimensionChild.Add(3);
        */



        timesUntilListUpdate = 100;
        /*
        DistancePolyOptimizeInit[] optimizedObjects;
        optimizedObjects = Object.FindObjectsOfType<DistancePolyOptimizeInit>();

        objectsToOptimize = new List<Transform>(); //DistancePolyOptimizeInit

        for (int i = 0; i < optimizedObjects.Length; i++)
        {
            objectsToOptimize.Add(optimizedObjects[i]);
        }
        */
    }

    

    //Adds a model to the bucketGrid of models to optimize
    public void addModel(Transform obj) //DistancePolyOptimizeInit    DistancePolyOptimizeInit
    {
        /*
        Transform curTransform = obj.transform;
        */

        

        //bucketGrid

        objectsToOptimize.Add(obj);
    }

    //Adds a model to the list of models to optimize
    public void removeModel(Transform obj) //DistancePolyOptimizeInit
    {
        objectsToOptimize.Remove(obj);
    }



    // Update is called once per frame
    void Update () {

        int pcBucketX, pcBucketZ, vrBucketX, vrBucketZ;

        pcBucketX = (int) (pcCamera.transform.position.x / bucketWidth);
        pcBucketZ = (int) (pcCamera.transform.position.z / bucketHeight);
        vrBucketX = (int) (pcCamera.transform.position.x / bucketWidth);
        vrBucketZ = (int) (pcCamera.transform.position.z / bucketHeight);

        //BucketGridController bgc = bucketGridController.GetComponent<BucketGridController>();
        int pcMaxBucketSize = 0;

        for (int ix = -1; ix <= 1; ix++)
        {
            for (int iy = -1; iy <= 1; iy++)
            {
                int testSize = bucketGrid.getBucket(pcBucketX + ix, pcBucketZ + iy).Count;
                if (testSize > pcMaxBucketSize)
                    pcMaxBucketSize = testSize;
            }
        }

        int vrMaxBucketSize = 0;

        for (int ix = -1; ix <= 1; ix++)
        {
            for (int iy = -1; iy <= 1; iy++)
            {
                int testSize = bucketGrid.getBucket(vrBucketX + ix, vrBucketZ + iy).Count;
                if (testSize > vrMaxBucketSize)
                    vrMaxBucketSize = testSize;
            }
        }

        for (int r = 0; r < 3; r++)
        {
            for (int ix = -1; ix <= 1; ix++)
            {
                for (int iy = -1; iy <= 1; iy++)
                {
                    //Update models near PC camera
                    List<Transform> bucketList = bucketGrid.getBucket(pcBucketX + ix, pcBucketZ + iy);

                    if (pcBucketableToOptimize < bucketList.Count)
                    //for (int i = 0; i < bucketList.Count; i++)
                    {
                        if (bucketList[pcBucketableToOptimize] != null)
                        {
                            //Debug.Log("object: " + bucketList[bucketableToOptimize]);
                            bucketList[pcBucketableToOptimize].GetComponent<PolygonOptimizer>().UpdateModel(maxHighPolyDistance);
                        }
                        else
                        {
                            bucketList.RemoveAt(pcBucketableToOptimize);
                        }
                    }

                    //Update models near VR camera
                    bucketList = bucketGrid.getBucket(vrBucketX + ix, vrBucketZ + iy); //.GetComponent<BucketGridController>()

                    //for (int i = 0; i < bucketList.Count; i++)
                    if (vrBucketableToOptimize < bucketList.Count)
                    {
                        if (bucketList[vrBucketableToOptimize] != null)
                        {
                            bucketList[vrBucketableToOptimize].GetComponent<PolygonOptimizer>().UpdateModel(maxHighPolyDistance);
                        }
                        else
                        {
                            bucketList.RemoveAt(vrBucketableToOptimize);
                        }
                    }
                }
            }

            pcBucketableToOptimize += 1;
            if (pcBucketableToOptimize >= pcMaxBucketSize)
            {
                pcBucketableToOptimize = 0;
            }
            vrBucketableToOptimize += 1;
            if (vrBucketableToOptimize >= vrMaxBucketSize)
            {
                vrBucketableToOptimize = 0;
            }
        }


        //If object is far away from camera, set to low poly model, else if its close set to high poly model
        for (int i = 0; i < 2; i++)
        {
            if (objectToOptimize < objectsToOptimize.Count)
            {
                if (objectsToOptimize[objectToOptimize] != null) 
                    objectsToOptimize[objectToOptimize].GetComponent< PolygonOptimizer >().UpdateModel(maxHighPolyDistance);
                //.GetComponent<DistancePolyOptimizeInit>()
            }
            //Debug.Log(objectToOptimize + " / " + objectsToOptimize.Count);
            //Cycle through one model per frame, looping through all the models
            if (objectToOptimize < objectsToOptimize.Count - 1)
            {
                objectToOptimize++;
            }
            else
            {
                objectToOptimize = 0;

                timesUntilListUpdate--;

                if (timesUntilListUpdate <= 0)
                {
                    timesUntilListUpdate = 100;
                    //optimizedObjects = Object.FindObjectsOfType<DistancePolyOptimizeInit>();
                }
            }
        }
	}
}
