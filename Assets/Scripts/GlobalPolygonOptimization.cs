using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GlobalPolygonOptimization : MonoBehaviour {

    public int maxHighPolyDistance; //default 15

    List<DistancePolyOptimizeInit> objectsToOptimize;
    
    int objectToOptimize, timesUntilListUpdate;

	// Use this for initialization
	void Start () {
        //objectsToOptimize = new List<DistancePolyOptimizeInit>(); //DistancePolyOptimizeInit
        objectToOptimize = 0;

        timesUntilListUpdate = 100;

        DistancePolyOptimizeInit[] optimizedObjects;
        optimizedObjects = Object.FindObjectsOfType<DistancePolyOptimizeInit>();

        objectsToOptimize = new List<DistancePolyOptimizeInit>();

        for (int i = 0; i < optimizedObjects.Length; i++)
        {
            objectsToOptimize.Add(optimizedObjects[i]);
        }
    }

    //Adds a model to the list of models to optimize
    public void addModel(DistancePolyOptimizeInit obj) //DistancePolyOptimizeInit
    {
        objectsToOptimize.Add(obj);
    }
    //Adds a model to the list of models to optimize
    public void removeModel(DistancePolyOptimizeInit obj) //DistancePolyOptimizeInit
    {
        objectsToOptimize.Remove(obj);
    }

    // Update is called once per frame
    void Update () {

        //If object is far away from camera, set to low poly model, else if its close set to high poly model
        
        for (int i = 0; i < 2; i++)
        {
            if (objectToOptimize < objectsToOptimize.Count)
            {
                if (objectsToOptimize[objectToOptimize] != null)
                    objectsToOptimize[objectToOptimize].UpdateModel(maxHighPolyDistance);
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
