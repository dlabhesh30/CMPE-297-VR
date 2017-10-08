using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraControllerPC : MonoBehaviour {

    //Rigidbody rb;

    Vector3 mouse_start_pos;
    Vector3 view_start_pos;
    
    public Vector3 slpoint1;
    public Vector3 slpoint2;

    Camera cam;

    public Transform target;

    Transform childCam; 

    // Use this for initialization
    void Start () {
        //rb = transform.GetComponent<Rigidbody>();
        childCam = transform.GetChild(0); //.gameObject;
        cam = childCam.GetComponent<Camera>();
        //PlayerSettings.virtualRealitySupported = false;
    }
    /*
    void OnGUI()
    {
        Vector3 p = new Vector3();
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + c.pixelWidth + ":" + c.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + p.ToString("F3"));
        GUILayout.EndArea();
    }
    */
    // Update is called once per frame
    void Update () {
        //rb.

        
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y ));

        float rx = transform.eulerAngles.x;
        float ry = transform.eulerAngles.y;
        float rz = transform.eulerAngles.z;

        if (Input.GetMouseButtonDown(0))
        {
            //GameObject x = Instantiate(prefab, new Vector2(Input.mousePosition.x, Input.mousePosition.y, Quaternion.identity);
            slpoint1 = mouseWorldPos;
        }

        if (Input.GetMouseButton(0))
        {
            //GameObject x = Instantiate(prefab, new Vector2(Input.mousePosition.x, Input.mousePosition.y, Quaternion.identity);
            slpoint2 = mouseWorldPos;
        }

        if (Input.GetMouseButtonDown(2))
            {
            mouse_start_pos = Input.mousePosition;
            view_start_pos = transform.position;
            }

        if (Input.GetMouseButton(2))
            {
            Vector3 vec = Quaternion.AngleAxis(ry, Vector3.up) * new Vector3(
                view_start_pos.x - (Input.mousePosition.x - mouse_start_pos.x) / 500 * transform.position.y, 
                view_start_pos.y, 
                view_start_pos.z - (Input.mousePosition.y - mouse_start_pos.y) / 500 * transform.position.y) 
                ;
                //new Vector3(view_start_pos.x + (-Input.mousePosition.x - mouse_start_pos.x) / 1000, transform.position.y , view_start_pos.z + (-Input.mousePosition.y - mouse_start_pos.y) / 1000);
            transform.position = vec;
            }
        float scale = transform.position.y / 2;
        transform.position += Quaternion.AngleAxis(ry, Vector3.up) * new Vector3( Input.GetAxis("Horizontal") * 20 , -Input.GetAxisRaw("Mouse ScrollWheel") * 100 * scale, Input.GetAxis("Vertical") * 20 + Input.GetAxisRaw("Mouse ScrollWheel") * 100 * scale) * Time.deltaTime;
        
        //Rotate the view
        ry += Input.GetAxisRaw("Horizontal Rotation");

        transform.eulerAngles = new Vector3( rx, ry, rz);

        
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

    void DrawRectangle(Vector3 point1, Vector3 point2)
    {
        //x1 to x2 on z1
        DrawLine(new Vector3(point1.x, point1.y + .1f, point1.z),
                 new Vector3(point1.x, point1.y + .1f, point1.z), Color.red);

        //x1 to x2 on z2
        DrawLine(new Vector3(point1.x, point1.y + .1f, point1.z),
                 new Vector3(point1.x, point1.y + .1f, point1.z), Color.blue);

        //z1 to z2 on x1
        DrawLine(new Vector3(point1.x, point1.y + .1f, point1.z),
                 new Vector3(point1.x, point1.y + .1f, point2.z), Color.blue);

        //z1 to z2 on x2
        DrawLine(new Vector3(point2.x, point1.y + .1f, point1.z),
                 new Vector3(point2.x, point1.y + .1f, point2.z), Color.blue);
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
