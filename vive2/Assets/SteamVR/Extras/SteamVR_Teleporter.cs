using UnityEngine;
using System.Collections;

public class SteamVR_Teleporter : MonoBehaviour
{
    //List of objects player can create
    public Transform createObj; // = new Transform[3];

    //place_obj_turret and place_obj_wall are the objects floating around your hand, placeObj is the building object you have currently selected to place
    public Transform placeObj, place_obj_turret, place_obj_wall, place_obj_hut;

    //buildingSelector is the parent object of all the objects floating around your hand, so if you rotate it then those objects will rotate too
    public Transform buildingSelector;

    public Transform wallPrefab, turretPrefab, hutPrefab;

    public int createObjSelected = 0;

    public Vector3 slpoint1;
    public Vector3 slpoint2;
    public Vector3 targetPoint;

    bool clicked = false;

    float selectionTimer = 0;

    float lastPx = 0;

    float scale = 0;
    bool padTouched = false;

    float buildingRotation = 0;
    
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

    public bool teleportOnClick = false;

    public bool selectOnClick = false;

    public TeleportType teleportType = TeleportType.TeleportTypeUseZeroY;

	Transform reference
	{
		get
		{
		var top = SteamVR_Render.Top();
		return (top != null) ? top.origin : null;
		}
	}

	void Start()
	{
		var trackedController = GetComponent<SteamVR_TrackedController>();
		if (trackedController == null)
		{
			trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
		}

		trackedController.TriggerClicked += new ClickedEventHandler(DoTriggerClick);

        trackedController.TriggerUnclicked += new ClickedEventHandler(DoTriggerReleased);

        trackedController.PadClicked += new ClickedEventHandler(DoPadPressed);
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

    void DoPadPressed(object sender, ClickedEventArgs e)
    {
        //Teleport if the pad is pressed
        if (teleportOnClick)
        {
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
                Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.position.x, refY, SteamVR_Render.Top().head.position.z);

                // We need to translate the reference space along the same vector
                // that is between the head's position on the ground and the intersection point on the ground
                // i.e. intersectionPoint - headPosOnGround = translateVector
                // currentReferencePosition + translateVector = finalPosition

                /*
                //Create objects where the controller is pointed
                Vector3 createPoint = new Vector3();
                createPoint = t.position + (ray.origin + (ray.direction * dist)) + new Vector3(0,0,0);
                Instantiate(createObj, createPoint, Quaternion.identity); //[createObjSelected]
                */
                
                //teleport
                t.position = t.position + (ray.origin + (ray.direction * dist)) - headPosOnGround;
            }
        }
    }

    void DoTriggerReleased(object sender, ClickedEventArgs e)
    {
        //Select units inside selection rectangle
        GameObject[] list = GameObject.FindGameObjectsWithTag("Player's Unit");
        for (int i = 0; i < list.Length; i++)
        {
        Rect rect = new Rect(slpoint1.x, slpoint1.z, slpoint2.x - slpoint1.x, slpoint2.z - slpoint1.z);
            if (rect.Contains(new Vector2(list[i].GetComponent<Transform>().position.x, list[i].GetComponent<Transform>().position.z), true))
            {
                list[i].BroadcastMessage("Select");
            }
            else
            {
                list[i].BroadcastMessage("DeSelect");
            }
        }
    }

	void DoTriggerClick(object sender, ClickedEventArgs e)
	{
        if (teleportOnClick)
		{
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

                /*
                Vector3 createPoint = new Vector3();
                createPoint = t.position + (ray.origin + (ray.direction * dist)) + new Vector3(0,0,0);
                Instantiate(createObj, createPoint, Quaternion.identity); //[createObjSelected]
                */

                if (selectingStep == selectingStepEnum.units)
                {
                    slpoint1 = (ray.origin + (ray.direction * dist)); //t.position + 
                    selectingStep = selectingStepEnum.target;
                }
                else
                if (selectingStep == selectingStepEnum.target)
                {
                    targetPoint = (ray.origin + (ray.direction * dist)); //t.position + 
                    selectingStep = selectingStepEnum.none;
                    GameObject[] list = GameObject.FindGameObjectsWithTag("Player's Unit");

                    float width = Mathf.Sqrt(list.Length);

                    for (int i = 0; i < list.Length; i++)
                        {
                        list[i].BroadcastMessage("Target", targetPoint + new Vector3((i - width / 2) * .3f , 0, 0));
                        slpoint1 = new Vector3(0, 0, 0);
                        slpoint2 = new Vector3(0, 0, 0);
                    }
                }
                //teleport
                //t.position = t.position + (ray.origin + (ray.direction * dist)) - headPosOnGround;
            }
		}
	}

    private void Update()
    {
        var trackedController = GetComponent<SteamVR_TrackedController>();

        //////////////PAD SWIPING FOR TOOL SELECTION//////////////////
        SteamVR_TrackedObject trackedObj = GetComponent<SteamVR_TrackedObject>();
        
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        float px = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).x;
        float py = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0).y;

        if (padTouched && Mathf.Abs(py) < .3)
        {
            selectionTimer = 30;
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

            if (selectionTimer > 0)
                selectionTimer -= 20 * Time.deltaTime;
            else
                selectionTimer = 0;
            if (selectionTimer < 10)
            {
                scale = Mathf.Lerp(0,1, selectionTimer / 10); //(scale + 0) / 2;
                
            }
            buildingRotation = (Mathf.Round(buildingRotation / 90) * 90 + buildingRotation) / 2;
        }

        buildingSelector.localScale = new Vector3(scale, scale, scale);
        buildingSelector.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + buildingRotation);
        lastPx = px;
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

        if (trackedController.triggerPressed)
        {

            // Create a plane at the Y position of the Play Area
            // Then create a Ray from the origin of the controller in the direction that the controller is pointing
            
            // Set defaults
            //bool hasGroundTarget = false;
            //float dist = 0f;
            

            if (hasGroundTarget && selectingStep == selectingStepEnum.target)
            {
                slpoint2 = (ray.origin + (ray.direction * dist)); //t.position + 
            }
        }
        else
        {
            if (selectingStep == selectingStepEnum.none)
            {
                selectingStep = selectingStepEnum.units;
                
            }
        }
        
        if (selectingStep == selectingStepEnum.target)
        {
            //x1 to x2 on z1
            DrawLine(new Vector3(slpoint1.x, slpoint1.y + .01f, slpoint1.z),
                 new Vector3(slpoint2.x, slpoint1.y + .01f, slpoint1.z), Color.blue);

            //x1 to x2 on z2
            DrawLine(new Vector3(slpoint1.x, slpoint1.y + .01f, slpoint2.z),
                     new Vector3(slpoint2.x, slpoint1.y + .01f, slpoint2.z), Color.blue);

            //z1 to z2 on x1
            DrawLine(new Vector3(slpoint1.x, slpoint1.y + .01f, slpoint1.z),
                     new Vector3(slpoint1.x, slpoint1.y + .01f, slpoint2.z), Color.blue);

            //z1 to z2 on x2
            DrawLine(new Vector3(slpoint2.x, slpoint1.y + .01f, slpoint1.z),
                     new Vector3(slpoint2.x, slpoint1.y + .01f, slpoint2.z), Color.blue);
        }


        Transform placePrefab = turretPrefab;

        bool placing = false;

        if (buildingRotation > 360)
            buildingRotation -= 360;
        if (buildingRotation < 0)
            buildingRotation += 360;

        if (Mathf.Round(buildingRotation) == 0)
        {
            placeObj = place_obj_turret;
            placePrefab = turretPrefab;
            placing = true;
        }

        if (Mathf.Round(buildingRotation) == 270)
        {
            placeObj = place_obj_wall;
            placePrefab = wallPrefab;
            placing = true;
        }

        if (Mathf.Round(buildingRotation) == 180)
        {
            placeObj = place_obj_hut;
            placePrefab = hutPrefab;
            placing = true;
        }

        GameObject[] cam = GameObject.FindGameObjectsWithTag("MainCamera");

        placeObj.position = (ray.origin + (ray.direction * dist));
        placeObj.eulerAngles = new Vector3(0, -transform.eulerAngles.z - cam[0].transform.eulerAngles.y, 0);

        
        if (trackedController.triggerPressed)
        {
            if (!clicked && placing)
            {
                clicked = true;
                //Debug.Log("Instantiated");
                Transform newobj = Instantiate(placePrefab, placeObj.position, new Quaternion(0, 0, 0, 0));
                newobj.eulerAngles = new Vector3(0, transform.eulerAngles.z, 0);
            }
        }
        else
        {
            clicked = false;
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
