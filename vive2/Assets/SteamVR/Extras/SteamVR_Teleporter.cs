using UnityEngine;
using System.Collections;

public class SteamVR_Teleporter : MonoBehaviour
{
    //List of objects player can create
    public Transform createObj; // = new Transform[3];
    public int createObjSelected = 0;

    public Vector3 slpoint1;
    public Vector3 slpoint2;

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

		trackedController.TriggerClicked += new ClickedEventHandler(DoClick);

        trackedController.TriggerUnclicked += new ClickedEventHandler(DoClick);


        if (teleportType == TeleportType.TeleportTypeUseTerrain)
		{
			// Start the player at the level of the terrain
			var t = reference;
			if (t != null)
				t.position = new Vector3(t.position.x, Terrain.activeTerrain.SampleHeight(t.position), t.position.z);
		}
	}

    void DoReleased(object sender, ClickedEventArgs e)
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("Player's Unit");
        for (int i = 0; i < list.Length; i++)
        {
        Rect rect = new Rect(slpoint1.x, slpoint1.y, slpoint2.x, slpoint2.y);
            if (rect.Contains(new Vector2(list[i].GetComponent<Transform>().position.x, list[i].GetComponent<Transform>().position.y)))
                list[i].BroadcastMessage("Select");
        }
    }

	void DoClick(object sender, ClickedEventArgs e)
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
				Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.position.x, refY, SteamVR_Render.Top().head.position.z);

                // We need to translate the reference space along the same vector
                // that is between the head's position on the ground and the intersection point on the ground
                // i.e. intersectionPoint - headPosOnGround = translateVector
                // currentReferencePosition + translateVector = finalPosition

                /*
                Vector3 createPoint = new Vector3();
                createPoint = t.position + (ray.origin + (ray.direction * dist)) + new Vector3(0,0,0);
                Instantiate(createObj, createPoint, Quaternion.identity); //[createObjSelected]
                */

                slpoint1 = t.position + (ray.origin + (ray.direction * dist));

                //teleport
                //t.position = t.position + (ray.origin + (ray.direction * dist)) - headPosOnGround;
            }
		}
	}

    private void Update()
    {
        var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController.triggerPressed)
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
                slpoint2 = t.position + (ray.origin + (ray.direction * dist));
            }
        }
        
        //x1 to x2 on z1
        DrawLine(new Vector3(slpoint1.x, slpoint1.y + .1f, slpoint1.z),
                 new Vector3(slpoint2.x, slpoint1.y + .1f, slpoint1.z), Color.blue);

        //x1 to x2 on z2
        DrawLine(new Vector3(slpoint1.x, slpoint1.y + .1f, slpoint2.z),
                 new Vector3(slpoint2.x, slpoint1.y + .1f, slpoint2.z), Color.blue);

        //z1 to z2 on x1
        DrawLine(new Vector3(slpoint1.x, slpoint1.y + .1f, slpoint1.z),
                 new Vector3(slpoint1.x, slpoint1.y + .1f, slpoint2.z), Color.blue);

        //z1 to z2 on x2
        DrawLine(new Vector3(slpoint2.x, slpoint1.y + .1f, slpoint1.z),
                 new Vector3(slpoint2.x, slpoint1.y + .1f, slpoint2.z), Color.blue);
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
