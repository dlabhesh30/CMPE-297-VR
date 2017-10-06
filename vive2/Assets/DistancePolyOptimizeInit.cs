using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistancePolyOptimizeInit : MonoBehaviour {

    public Transform highPolyModelVR, lowPolyModelVR, highPolyModelPC, lowPolyModelPC;
    public List<GameObject> Children;

    float maxHighPolyDistance = 10;

    // Use this for initialization
    void Start ()
    {
        highPolyModelVR = transform.GetChild(0);
        lowPolyModelVR = transform.GetChild(1);
        highPolyModelPC = transform.GetChild(2);
        lowPolyModelPC = transform.GetChild(3);

        highPolyModelVR.gameObject.SetActive(false);
        lowPolyModelVR.gameObject.SetActive(true);
        highPolyModelPC.gameObject.SetActive(false);
        lowPolyModelPC.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
        UpdateModel();

    }

    //Activates the high or low poly model depending on how far away it is from the camera
    void UpdateModel() {
        //Do Polygon Optimization for VR Headset
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        float dist = Vector3.Distance(cam.transform.position, transform.position);
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
        
        //Do Polygon Optimization for PC Screen
        cam = GameObject.FindGameObjectWithTag("MainCameraPC");
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
}
