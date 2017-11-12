using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistancePolyOptimizeInit : MonoBehaviour {

    public Transform highPolyModelVR, lowPolyModelVR, highPolyModelPC, lowPolyModelPC;
    //public List<GameObject> Children;

    //public float maxHighPolyDistance = 15;

    //bool added = false;

    // Use this for initialization
    void Start ()
    {
        highPolyModelVR = transform.GetChild(0);
        lowPolyModelVR = transform.GetChild(1);
        highPolyModelPC = transform.GetChild(2);
        lowPolyModelPC = transform.GetChild(3);

        highPolyModelVR.gameObject.SetActive(true);
        lowPolyModelVR.gameObject.SetActive(false);
        highPolyModelPC.gameObject.SetActive(true);
        lowPolyModelPC.gameObject.SetActive(false);

        //Add this model to the global list of models to cycle through and optimize
        //if (transform.gameObject != null)

        //added = true;

        //GlobalPolygonOptimization polygonOptimizer = GameObject.FindGameObjectWithTag("PolygonOptimizer").GetComponent<GlobalPolygonOptimization>();
        //polygonOptimizer.addModel(transform.gameObject);

        //GlobalPolygonOptimization polygonOptimizer = GameObject.FindGameObjectWithTag("PolygonOptimizer").GetComponent<GlobalPolygonOptimization>();
        //Add this model to the global list of models to cycle through and optimize
        //polygonOptimizer.addModel(transform); //GetComponent<DistancePolyOptimizeInit>()


    }


    //Activates the high or low poly model depending on how far away it is from the camera
    //update model is called from the "PolygonOptimizer" object, which should have the global polygon optimization script attached to it
    public void UpdateModel(int maxHighPolyDistance) {
        
        //Do Polygon Optimization for VR Headset
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        float dist;
        if (cam != null)
        {
            dist = Vector3.Distance(cam.transform.position, transform.position);
            //If far enough away activate low poly model and deactivate high poly model
            if (dist > maxHighPolyDistance)
            {
                lowPolyModelVR.gameObject.SetActive(true);
                highPolyModelVR.gameObject.SetActive(false);
            }
            else
            //If close enough activate high poly model and deactivate low poly model
            {
                lowPolyModelVR.gameObject.SetActive(false);
                highPolyModelVR.gameObject.SetActive(true);
            }
        }
        
        //Do Polygon Optimization for PC Screen
        cam = GameObject.FindGameObjectWithTag("MainCameraPC");
        if (cam != null)
        {
            Debug.Log("Found PC camera");
            dist = Vector3.Distance(cam.transform.position, transform.position);
            //If far enough away activate low poly model and deactivate high poly model
            if (dist > maxHighPolyDistance)
            {
                lowPolyModelPC.gameObject.SetActive(true);
                highPolyModelPC.gameObject.SetActive(false);
            }
            else
            //If close enough activate high poly model and deactivate low poly model
            {
                lowPolyModelPC.gameObject.SetActive(false);
                highPolyModelPC.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Couldn't find PC camera");
        }
    }
}
