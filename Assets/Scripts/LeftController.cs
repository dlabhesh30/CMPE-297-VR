using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftController : MonoBehaviour {


    float buildingRotation = 0;
    float padXPrev = 0, lastPx = 0;
    public Transform createObjModel;

    float scale = 0;
    bool padTouched = false;

    float selectionTimer = 0;

    // Use this for initialization
    void Start () {

        var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }
        trackedController.PadTouched += new ClickedEventHandler(DoPadTouched);
        trackedController.PadUntouched += new ClickedEventHandler(DoPadReleased);
    }

    // Update is called once per frame
    void Update() {

        var trackedController = GetComponent<SteamVR_TrackedController>();
        SteamVR_TrackedObject trackedObj = GetComponent<SteamVR_TrackedObject>();

        //buildingRotation += 5;
        //createObjModel.eulerAngles = new Vector3(0, 0, buildingRotation);


        var device = SteamVR_Controller.Input((int) trackedObj.index);

        float px = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x;
        float py = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y;

        if (padTouched)
        {
            selectionTimer = 500;
            //Debug.Log("GOT TO DoPadTouched");
            //if (padXPrev != 0)
            //{
            //Debug.Log("GOT TO INSIDE" + Time.realtimeSinceStartup);
            //when user swipes add to rotation of buildings around pad
            //buildingRotation += 10; // e.padX - padXPrev;
            Debug.Log("px = " + px + " lastPx = " + lastPx);
            if (px != 0 && lastPx != 0)
            {
                buildingRotation -= (px - lastPx) * 120; //* (padXPrev - e.padX);
               
            }

            //}

            //store previous position of pad
            scale = (scale + 1) / 2;
        }
        else
        {
            scale = (scale + 0) / 2;
            if (selectionTimer > 0)
                selectionTimer--;
            else
            {
            }
            buildingRotation = (Mathf.Round(buildingRotation / 90) * 90 + buildingRotation) / 2;
        }

        createObjModel.localScale = new Vector3(scale, scale, scale);
        createObjModel.eulerAngles = new Vector3(transform.eulerAngles.x , transform.eulerAngles.y, transform.eulerAngles.z + buildingRotation);
        lastPx = px;
        //buildingRotation += 1; // Input.GetAxis("Horizontal") * 10;
        /*
        var trackedController = GetComponent<SteamVR_TrackedController>();

        if (trackedController.padTouched)
        {
            Debug.Log("GOT TO DoPadTouched");
            buildingRotation += .5f;
        }
        */
    }
    
    void DoPadTouched(object sender, ClickedEventArgs e)
    {
        //Debug.Log("GOT TO DoPadTouched");
        padTouched = true;
    }

    void DoPadReleased(object sender, ClickedEventArgs e)
    {
        //Debug.Log("GOT TO DoPadReleased");
        padTouched = false;
    }

    }
