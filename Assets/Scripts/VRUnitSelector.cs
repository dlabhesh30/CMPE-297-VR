using UnityEngine;
using System.Collections;
using System;

public class VRUnitSelector : MonoBehaviour
{
    //List of objects player can create
    //public Transform createObj; // = new Transform[3];

    public GameObject groundPlane;

    //place_obj_turret and place_obj_wall are the objects floating around your hand, placeObj is the building object you have currently selected to place
    //public Transform placeObj, place_obj_turret, place_obj_wall, place_obj_hut;

    //buildingSelector is the parent object of all the objects floating around your hand, so if you rotate it then those objects will rotate too
    //public Transform buildingSelector;

    //Vector3 startBuildPoint;
    //public Transform wallPrefab, turretPrefab, hutPrefab;

    //public int createObjSelected = 0;

    bool triggerHeld = false, triggerReleased = false;

    public GameObject tutorialGameObject;
    ControlsTutorial tutorial;

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

    float buildingRotation = 90;

    private enum selectingStepEnum
    {
        units,
        target,
        none
    }

    private selectingStepEnum selectingStep = selectingStepEnum.units;
    
    public enum TeleportType
    {
        TeleportTypeUseTerrain,
        TeleportTypeUseCollider,
        TeleportTypeUseZeroY
    }
    
    //public bool teleportOnClick = false;

    public TeleportType teleportType = TeleportType.TeleportTypeUseZeroY;
    
    //public bool selectOnClick = false;

    Transform reference
    {
        get
        {
            var top = SteamVR_Render.Top();
            return (top != null) ? top.origin : null;
        }
    }

    VRController vrcontroller;

    void Start()
    {
        vrcontroller = GetComponent<VRController>();
        //tutorial = tutorialGameObject.GetComponent<ControlsTutorial>(); //FindGameObjectWithTag("Tutorial").GetComponent<ControlsTutorial>();

        groundPlane = GameObject.FindGameObjectWithTag("GroundPlane");
        var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }

        trackedController.TriggerClicked += new ClickedEventHandler(DoTriggerClick);

        trackedController.TriggerUnclicked += new ClickedEventHandler(DoTriggerReleased);
        
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
    

    void DoTriggerReleased(object sender, ClickedEventArgs e)
    {
        //triggerHeld = false;
        //Select units inside selection rectangle
        GameObject[] list = GameObject.FindGameObjectsWithTag("VR Player's Unit");
        for (int i = 0; i < list.Length; i++)
        {
            Rect rect = new Rect(slpoint1.x, slpoint1.z, slpoint2.x - slpoint1.x, slpoint2.z - slpoint1.z);
            if (rect.Contains(new Vector2(list[i].GetComponent<Transform>().position.x, list[i].GetComponent<Transform>().position.z), true))
            {
                list[i].BroadcastMessage("Select");
                /*
                 * if (tutorial.tutorialStep == 0)
                {
                    tutorial.NextTutorialStep();
                }*/
            }
            else
            {
                list[i].BroadcastMessage("DeSelect");
            }
        }
    }

    void DoTriggerClick(object sender, ClickedEventArgs e)
    {
        //if (teleportOnClick)
        //{
            // First get the current Transform of the the reference space (i.e. the Play Area, e.g. CameraRig prefab)
            var t = reference;
            if (t == null)
                return;

            // Get the current Y position of the reference space
            float refY = t.position.y;

            // Create a plane at the Y position of the Play Area
            // Then create a Ray from the origin of the controller in the direction that the controller is pointing
            Plane plane = new Plane(Vector3.up, -refY);
            Ray ray = new Ray(this.transform.position, transform.forward);

            // Set defaults
            bool hasGroundTarget = false;
            float dist = 0f;
            if (teleportType == TeleportType.TeleportTypeUseTerrain) // If we picked to use the terrain
            {
                RaycastHit hitInfo;
                TerrainCollider tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
                hasGroundTarget = tc.Raycast(ray, out hitInfo, 1000f);
                dist = hitInfo.distance;
            }
            else if (teleportType == TeleportType.TeleportTypeUseCollider) // If we picked to use the collider
            {
                RaycastHit hitInfo;
                hasGroundTarget = Physics.Raycast(ray, out hitInfo);
                dist = hitInfo.distance;
            }
            else // If we're just staying flat on the current Y axis
            {
                // Intersect a ray with the plane that was created earlier
                // and output the distance along the ray that it intersects
                hasGroundTarget = plane.Raycast(ray, out dist);
            }

            if (hasGroundTarget)
            {
                // Get the current Camera (head) position on the ground relative to the world
                //Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.position.x, refY, SteamVR_Render.Top().head.position.z);

                // We need to translate the reference space along the same vector
                // that is between the head's position on the ground and the intersection point on the ground
                // i.e. intersectionPoint - headPosOnGround = translateVector
                // currentReferencePosition + translateVector = finalPosition
                
                if (selectingStep == selectingStepEnum.units)
                {
                    slpoint1 = (ray.origin + (ray.direction * dist));
                    selectingStep = selectingStepEnum.target;
                }
                else
                if (selectingStep == selectingStepEnum.target)
                {
                    targetPoint = (ray.origin + (ray.direction * dist));
                    selectingStep = selectingStepEnum.none;
                    GameObject[] list = GameObject.FindGameObjectsWithTag("VR Player's Unit");

                    ArrayList listSelected = new ArrayList();
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i].GetComponent<UnitController>().selected)
                        {
                            listSelected.Add(list[i]);
                        }
                    }

                    float width = Mathf.Sqrt(listSelected.Count);

                    if (listSelected.Count == 1)
                    {
                        ((GameObject)listSelected[0]).BroadcastMessage("Target", targetPoint);
                        //((GameObject)listSelected[0]).GetComponent<UnitController>().Target(targetPoint);
                    }
                    else
                    {
                        int row = 0, col = 0;
                        for (int i = 0; i < listSelected.Count; i++)
                        {
                            /*if (tutorial.tutorialStep == 1)
                            {
                                if (Vector2.Distance(new Vector2(tutorial.tutorialSlides[4].position.x, tutorial.tutorialSlides[4].position.z), new Vector2(targetPoint.x, targetPoint.z)) < 2)
                                {
                                    tutorial.NextTutorialStep();
                                }
                            }*/
                        Debug.Log("here");
                        ((GameObject)listSelected[i]).BroadcastMessage("Target", targetPoint + new Vector3((col) * .3f - width / 2 * .3f, 0, (row) * .3f - width / 2 * .3f));
                        //((GameObject)listSelected[i]).GetComponent<UnitController>().Target(targetPoint + new Vector3((col) * .3f - width / 2 * .3f, 0, (row) * .3f - width / 2 * .3f));
                            slpoint1 = new Vector3(0, 0, 0);
                            slpoint2 = new Vector3(0, 0, 0);
                            col++;
                            if (col > width)
                            {
                                col = 0;
                                row++;
                            }
                        }
                    }
                }
            }
        //}
    }

    private void Update()
    {
        var trackedController = GetComponent<SteamVR_TrackedController>();

        //////////////PAD SWIPE / SWIPING FOR TOOL SELECTION AND ZOOMING IN / OUT//////////////////
        SteamVR_TrackedObject trackedObj = GetComponent<SteamVR_TrackedObject>();

        var device = SteamVR_Controller.Input((int)trackedObj.index);
        
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

        if (trackedController.triggerPressed)
        {
            // Create a plane at the Y position of the Play Area
            // Then create a Ray from the origin of the controller in the direction that the controller is pointing

            // Set defaults
            
            if (hasGroundTarget && selectingStep == selectingStepEnum.target)
            {
                slpoint2 = (ray.origin + (ray.direction * dist));
            }
        }
        else
        {
            if (selectingStep == selectingStepEnum.none)
            {
                selectingStep = selectingStepEnum.units;
            }
        }
        
        GameObject[] cam = GameObject.FindGameObjectsWithTag("MainCamera");

        //IF THE TRIGGER IS HELD DOWN
        if (triggerHeld && !trackedController.triggerPressed)
            triggerReleased = true;
        else
            triggerReleased = false;
        triggerHeld = trackedController.triggerPressed;

        if (selectingStep == selectingStepEnum.target && vrcontroller.controlMode == VRController.controlModeState.selecting)
        {
            float height = .2f;
            //x1 to x2 on z1
            DrawLine(new Vector3(slpoint1.x, slpoint1.y + height, slpoint1.z),
                 new Vector3(slpoint2.x, slpoint1.y + height, slpoint1.z), Color.blue);

            //x1 to x2 on z2
            DrawLine(new Vector3(slpoint1.x, slpoint1.y + height, slpoint2.z),
                     new Vector3(slpoint2.x, slpoint1.y + height, slpoint2.z), Color.blue);

            //z1 to z2 on x1
            DrawLine(new Vector3(slpoint1.x, slpoint1.y + height, slpoint1.z),
                     new Vector3(slpoint1.x, slpoint1.y + height, slpoint2.z), Color.blue);

            //z1 to z2 on x2
            DrawLine(new Vector3(slpoint2.x, slpoint1.y + height, slpoint1.z),
                     new Vector3(slpoint2.x, slpoint1.y + height, slpoint2.z), Color.blue);
        }
    }

    //CREDIT FOR DRAWLINE SCRIPT GOES TO paranoidray from his answer on http://answers.unity3d.com/questions/8338/how-to-draw-a-line-using-script.html
    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.01f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.01f, 0.01f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
}
