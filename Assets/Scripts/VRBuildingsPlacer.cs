using UnityEngine;
using System.Collections;
using System;

public class VRBuildingsPlacer : MonoBehaviour
{
    //List of objects player can create
    public Transform createObj;

    public GameObject groundPlane;

    //place_obj_turret and place_obj_wall are the objects floating around your hand, placeObj is the building object you have currently selected to place
    public Transform placeObj, place_obj_turret, place_obj_wall, place_obj_hut;

    //Building cost indicator is the number that floats over the buildings when placing them in VR
    public Transform buildingCostIndicator, floatingNumberPrefab;

    //buildingSelector is the parent object of all the objects floating around your hand, so if you rotate it then those objects will rotate too
    public Transform buildingSelector;

    Vector3 startBuildPoint;
    public Transform wallPrefab, turretPrefab, hutPrefab;

    public int createObjSelected = 0;

    bool triggerHeld = false, triggerReleased = false;
    
    public Vector3 slpoint1;
    public Vector3 slpoint2;
    public Vector3 targetPoint;

    bool clicked = false;

    bool swipingHorizontally = false, swipingVertically = false;

    float swipeTimer = 0;

    float selectionTimer = 0;

    float lastPx = 0, lastPy = 0;

    float scale = 0;
    bool padTouched = false;

    VRController vrcontroller;

    float buildingRotation = 90;
    /*
    private enum controlModeState
    {
        selecting,
        building_hut,
        building_turret,
        building_wall
    }

    private controlModeState controlMode = controlModeState.selecting;
    */
    public enum TeleportType
    {
        TeleportTypeUseTerrain,
        TeleportTypeUseCollider,
        TeleportTypeUseZeroY
    }
    
    //public bool teleportOnClick = false;

    public TeleportType teleportType = TeleportType.TeleportTypeUseZeroY;
    
    Transform reference
    {
        get
        {
            var top = SteamVR_Render.Top();
            return (top != null) ? top.origin : null;
        }
    }

    GameObject resourceControllerGameObject;
    ResourceController resourceController;

    // Use this for initialization
    void Start()
    {
        vrcontroller = GetComponent<VRController>();
        resourceControllerGameObject = GameObject.FindGameObjectWithTag("Resource Controller");

        resourceController = resourceControllerGameObject.GetComponent<ResourceController>();

        placeObj = null;
        groundPlane = GameObject.FindGameObjectWithTag("GroundPlane");
        var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }
        
        trackedController.PadTouched += new ClickedEventHandler(DoPadTouched);
        trackedController.PadUntouched += new ClickedEventHandler(DoPadReleased);

        if (teleportType == TeleportType.TeleportTypeUseTerrain)
        {
            // Start the player at the level of the terrain
            var t = reference;
            if (t != null)
                t.position = new Vector3(t.position.x, Terrain.activeTerrain.SampleHeight(t.position), t.position.z);
        }
    }
    
    void DoPadTouched(object sender, ClickedEventArgs e)
    {
        padTouched = true;
    }

    void DoPadReleased(object sender, ClickedEventArgs e)
    {
        padTouched = false;
    }

    private void Update()
    {
        var trackedController = GetComponent<SteamVR_TrackedController>();

        //////////////PAD SWIPE / SWIPING FOR TOOL SELECTION AND ZOOMING IN / OUT//////////////////
        SteamVR_TrackedObject trackedObj = GetComponent<SteamVR_TrackedObject>();

        var device = SteamVR_Controller.Input((int)trackedObj.index);

        float px = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x;
        float py = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y;

        if (padTouched && Mathf.Abs(py) < .3)
        {
            selectionTimer = 30;

            //if (!swipingHorizontally && !swipingVertically)
            //{
            swipingHorizontally = false;
            swipingVertically = false;

            if (lastPx != 0 && Math.Abs(px - lastPx) > 2 * Time.deltaTime && Math.Abs(px - lastPx) > Math.Abs(py - lastPy))
                swipingHorizontally = true;
            else
            if (lastPy != 0 && Math.Abs(py - lastPy) > 2 * Time.deltaTime)
                swipingVertically = true;
            //}

            if (swipingHorizontally && px != 0 && lastPx != 0)
            {
                buildingRotation -= (px - lastPx) * 120; //* (padXPrev - e.padX);
            }

            if (swipingVertically && py != 0 && lastPy != 0)
            {
                float magnitude = transform.parent.gameObject.transform.localScale.magnitude;
                transform.parent.gameObject.transform.localScale += new Vector3(1, 1, 1) * (py - lastPy) * magnitude;

                magnitude = transform.parent.gameObject.transform.localScale.magnitude;

                if (magnitude < .25)
                    transform.parent.gameObject.transform.localScale = new Vector3(1, 1, 1) * .25f;

                if (magnitude > 500)
                    transform.parent.gameObject.transform.localScale = new Vector3(1, 1, 1) * 500;
            }

            if (swipingHorizontally)
                scale = (scale + 1) / 2;
        }
        else
        {
            if (selectionTimer > 0)
                selectionTimer -= 20 * Time.deltaTime;
            else
                selectionTimer = 0;

            if (selectionTimer < 18)
            {
                swipingHorizontally = false;
                swipingVertically = false;
            }

            if (selectionTimer < 10)
            {
                scale = Mathf.Lerp(0, 1, selectionTimer / 10); //(scale + 0) / 2;

            }
            buildingRotation = (Mathf.Round(buildingRotation / 90) * 90 + buildingRotation) / 2;
        }

        buildingSelector.localScale = new Vector3(scale, scale, scale);
        buildingSelector.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + buildingRotation);

        //store previous position of pad
        lastPx = px;
        lastPy = py;
        ////////////////////////////////////

        // First get the current Transform of the the reference space (i.e. the Play Area, e.g. CameraRig prefab)
        var t = reference;
        if (t == null)
            return;

        // Get the current Y position of the reference space
        float refY = t.position.y;
        Plane plane = new Plane(Vector3.up, -refY);
        Ray ray = new Ray(this.transform.position, transform.forward);

        bool hasGroundTarget = false;
        float dist = 0f;
        RaycastHit hitInfo;
        TerrainCollider tc; // = Terrain.activeTerrain.GetComponent<TerrainCollider>();
        //dist = hitInfo.distance;

        if (teleportType == TeleportType.TeleportTypeUseTerrain) // If we picked to use the terrain
        {
            //RaycastHit hitInfo;
            tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
            hasGroundTarget = tc.Raycast(ray, out hitInfo, 1000f);
            dist = hitInfo.distance;
        }
        else if (teleportType == TeleportType.TeleportTypeUseCollider) // If we picked to use the collider
        {
            //RaycastHit hitInfo;
            hasGroundTarget = Physics.Raycast(ray, out hitInfo);
            dist = hitInfo.distance;
        }
        else // If we're just staying flat on the current Y axis
        {
            // Intersect a ray with the plane that was created earlier
            // and output the distance along the ray that it intersects
            hasGroundTarget = plane.Raycast(ray, out dist);
        }
        
        Transform placePrefab = turretPrefab;

        bool placing = false;

        if (buildingRotation > 360)
            buildingRotation -= 360;
        if (buildingRotation < 0)
            buildingRotation += 360;

        //GameObject[] models;
        //models = createObj.GetComponentsInChildren<GameObject>();

        Transform wallTransform, turretTransform, tepeeTransform;
        wallTransform = createObj.GetChild(0);
        turretTransform = createObj.GetChild(1);
        tepeeTransform = createObj.GetChild(2);

        if (Mathf.Round(buildingRotation / 90) * 90 == 0 || Mathf.Round(buildingRotation / 90) * 90 == 360)
        {
            vrcontroller.controlMode = VRController.controlModeState.building_turret;
            placeObj = place_obj_turret; // turretTransform;
            placePrefab = turretPrefab;
            wallTransform.gameObject.SetActive(false);
            turretTransform.gameObject.SetActive(true);
            tepeeTransform.gameObject.SetActive(false);
            placing = true;
        }
        else
        if (Mathf.Round(buildingRotation / 90) * 90 == 270)
        {
            vrcontroller.controlMode = VRController.controlModeState.building_wall;
            placeObj = place_obj_wall; // wallTransform;
            placePrefab = wallPrefab;
            wallTransform.gameObject.SetActive(true);
            turretTransform.gameObject.SetActive(false);
            tepeeTransform.gameObject.SetActive(false);
            placing = true;
        }
        else
        if (Mathf.Round(buildingRotation / 90) * 90 == 180)
        {
            vrcontroller.controlMode = VRController.controlModeState.building_hut;
            placeObj = place_obj_hut; // tepeeTransform;
            placePrefab = hutPrefab;
            wallTransform.gameObject.SetActive(false);
            turretTransform.gameObject.SetActive(false);
            tepeeTransform.gameObject.SetActive(true);
            placing = true;
        }
        else
        {
            vrcontroller.controlMode = VRController.controlModeState.selecting;
            wallTransform.gameObject.SetActive(false);
            turretTransform.gameObject.SetActive(false);
            tepeeTransform.gameObject.SetActive(false);
        }

        GameObject[] cam = GameObject.FindGameObjectsWithTag("MainCamera");

        //IF THE TRIGGER IS HELD DOWN
        if (triggerHeld && !trackedController.triggerPressed)
            triggerReleased = true;
        else
            triggerReleased = false;
        triggerHeld = trackedController.triggerPressed;

        //Create Buildings, Place Buildings
        /*
        Transform newobj = Instantiate(placePrefab, placeObj.position, new Quaternion(0, 0, 0, 0));
        newobj.eulerAngles = new Vector3(0, -transform.eulerAngles.z, 0);
        */
        if (vrcontroller.controlMode != VRController.controlModeState.selecting)
        {

            buildingCostIndicator.gameObject.SetActive(true);
            
            if (placeObj != null)
            {

                //if (!placeObj.gameObject.GetComponent<PlaceObj>().isColliding)
                //{
                placeObj.position = (ray.origin + (ray.direction * dist));
                buildingCostIndicator.position = placeObj.position + new Vector3(0, 1, 0);
                buildingCostIndicator.eulerAngles = new Vector3(0, 90 - Mathf.Rad2Deg * (Mathf.Atan2(buildingCostIndicator.position.z - transform.position.z, buildingCostIndicator.position.x - transform.position.x)), 0);
                createObj.position = placeObj.position;
                //}
                //if (placeObj.gameObject.colliding() == false)
                placeObj.eulerAngles = new Vector3(0, -transform.eulerAngles.z, 0); // - cam[0].transform.eulerAngles.y
                createObj.eulerAngles = placeObj.eulerAngles;

                int increments = 1;

                if (triggerHeld || triggerReleased)
                {
                    if (!clicked && placing)
                    {
                        startBuildPoint = placeObj.position;
                        clicked = true;
                    }

                    if (placeObj.position.magnitude == 0)
                    {
                        wallTransform.gameObject.SetActive(false);
                        turretTransform.gameObject.SetActive(false);
                        tepeeTransform.gameObject.SetActive(false);
                    }

                    float dis = Vector3.Distance(startBuildPoint, placeObj.position);

                    float xpos = startBuildPoint.x;
                    float zpos = startBuildPoint.z;

                    if (dis > .5f && placeObj == place_obj_wall)
                    {
                        dis = Math.Min(dis, 4);
                        increments = Math.Min((int)(dis / .4f), 10);

                        //Debug.Log("INCREMENTS: " + increments);

                        float dir = -Mathf.Atan2(placeObj.position.z - startBuildPoint.z, placeObj.position.x - startBuildPoint.x) * 180 / Mathf.PI;
                        //Angle(new Vector2(startBuildPoint.x, startBuildPoint.z) , new Vector2(placeObj.position.x, placeObj.position.z));

                        for (int i = 0; i <= increments; i++)
                        {
                            Transform newobj = Instantiate(placePrefab, new Vector3(xpos, 0, zpos), new Quaternion(0, 0, 0, 0)); // startBuildPoint.y
                            newobj.eulerAngles = new Vector3(0, dir, 0);
                            newobj.tag = "VR Player's Building";
                            //Component hb = newobj.gameObject.GetComponent<HealthBar>();

                            if (!triggerReleased)
                            {
                                GameObject.Destroy(newobj.gameObject, .02f);
                            }
                            xpos += Mathf.Cos(-Mathf.Deg2Rad * dir) * (dis / increments);
                            zpos += Mathf.Sin(-Mathf.Deg2Rad * dir) * (dis / increments);
                        }
                    }
                    else
                    {
                        //Debug.Log("HERE2");
                        Transform newobj = Instantiate(placePrefab, new Vector3(startBuildPoint.x, 0, startBuildPoint.z), new Quaternion(0, 0, 0, 0));
                        newobj.eulerAngles = new Vector3(0, -transform.eulerAngles.z, 0);
                        newobj.tag = "VR Player's Building";
                        if (trackedController.triggerPressed)
                        {
                            GameObject.Destroy(newobj.gameObject, .02f);
                        }
                    }

                    if (triggerReleased)
                    {
                        //Create floating number


                        Vector3 floatingNumberPosition;
                        //startBuildPoint, placeObj.position
                        if (increments > 1)
                            floatingNumberPosition = new Vector3((startBuildPoint.x + placeObj.position.x) / 2, 1, (startBuildPoint.z + placeObj.position.z) / 2);
                        else
                            floatingNumberPosition = new Vector3(startBuildPoint.x, 1, startBuildPoint.z);

                        Transform floatingNumber = Instantiate(floatingNumberPrefab, floatingNumberPosition,
                             buildingCostIndicator.rotation);

                        

                        int costCoins = 0;

                        if (placeObj == place_obj_hut)
                            costCoins = resourceController.costTepee;
                        if (placeObj == place_obj_turret)
                            costCoins = resourceController.costTurret;
                        if (placeObj == place_obj_wall)
                            costCoins = resourceController.costWall * increments;

                        floatingNumber.GetComponent<TextMesh>().text = "-" + costCoins+" Coins";
                        floatingNumber.GetComponent<TextMesh>().color = Color.red;
                        resourceController.addVRCoins(-costCoins);
                    }

                }
                else
                {
                    clicked = false;
                }


                if (placeObj == place_obj_hut)
                    buildingCostIndicator.GetComponent<TextMesh>().text = "-" + resourceController.costTepee + " Coins";
                if (placeObj == place_obj_wall)
                    buildingCostIndicator.GetComponent<TextMesh>().text = "-" + (resourceController.costWall * increments) + " Coins";
                if (placeObj == place_obj_turret)
                    buildingCostIndicator.GetComponent<TextMesh>().text = "-" + resourceController.costTurret + " Coins";

            }
        }
        else
        {
            buildingCostIndicator.gameObject.SetActive(false);
        }
    }
}
