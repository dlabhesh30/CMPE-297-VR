using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerUnitController : MonoBehaviour {

    public bool selected;

    public Vector3 targetPoint;

    Animator anim;

    public bool hasTarget = false;

    NavMeshAgent agent;
    Rigidbody rb;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        if (agent.velocity.magnitude > .2f)
        transform.rotation = Quaternion.LookRotation(agent.velocity + new Vector3(0, 90, 0));

        Animating(agent.velocity);

        if (selected)
        {
            
            //top
            DrawLine(new Vector3(x - .1f, y + .01f, z - .1f),
                     new Vector3(x + .1f, y + .01f, z - .1f), Color.blue);

            //bottom
            DrawLine(new Vector3(x - .1f, y + .01f, z + .1f),
                     new Vector3(x + .1f, y + .01f, z + .1f), Color.blue);

            //left
            DrawLine(new Vector3(x - .1f, y + .01f, z - .1f),
                     new Vector3(x - .1f, y + .01f, z + .1f), Color.blue);

            //right
            DrawLine(new Vector3(x + .1f, y + .01f, z - .1f),
                     new Vector3(x + .1f, y + .01f, z + .1f), Color.blue);
        }

        if (hasTarget == true)
        {
            //DRAW TARGET CROSS
            DrawLine(new Vector3(targetPoint.x - .1f, targetPoint.y + .01f, targetPoint.z - .1f),
                     new Vector3(targetPoint.x + .1f, targetPoint.y + .01f, targetPoint.z + .1f), Color.blue);
            
            DrawLine(new Vector3(targetPoint.x - .1f, targetPoint.y + .01f, targetPoint.z + .1f),
                     new Vector3(targetPoint.x + .1f, targetPoint.y + .01f, targetPoint.z - .1f), Color.blue);
        }

	}

    void Select()
    {
        selected = true;
        Debug.Log("Selected");
    }


    void Target(Vector3 target)
    {
        if (selected == true)
        {
            targetPoint = target;
            hasTarget = true;
            selected = false;
            Debug.Log("Target chosen, deselected");
            agent.SetDestination(targetPoint);
        }
    }


    void DeSelect()
    {
        selected = false;
        Debug.Log("Here4");
    }

    void Animating(Vector3 movement)
    {
        bool running = movement.magnitude > 0.2f;
        anim.SetBool("IsRunning", running);
        anim.speed = movement.magnitude * 3 + .5f;
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
