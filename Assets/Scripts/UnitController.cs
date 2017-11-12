using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour {

    //Animation Variables
    Animator animVR, animPC;

    //Team Variables
    public int team;
    
    //Target and Selection Indicator Variables
    GameObject selectCircle;
    public Vector3 targetPoint;
    public bool selected, hasTarget = false;
    bool targetIndicatorSet = false, selectionIndicatorSet = false;
    GameObject[] targetIndicator = new GameObject[2], selectionIndicator = new GameObject[4];
    
    //Pathfinding and Collisions Variables
    NavMeshAgent agent;
    Rigidbody rb;
    Transform modelVR, modelPC;

    public GameObject targetPrefab;

    // Use this for initialization
    void Start ()
    {
        //Navigation Agent
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        //3D Model
        //modelVR = transform.GetChild(0);
        //modelPC = transform.GetChild(0);

        rb = GetComponent<Rigidbody>();

        transform.rotation = Quaternion.Euler(0,0,0); //LookRotation(new Vector3(0, 0, 0));
        //selectCircle = transform.GetChild(1).gameObject;

        for (int i = 0; i < transform.childCount; i++)
        {

            if (transform.GetChild(i).gameObject.name == "HighPolyModelVR")
            {
                modelVR = transform.GetChild(i);
            }
            if (transform.GetChild(i).gameObject.name == "HighPolyModelPC")
            {
                modelPC = transform.GetChild(i);
            }

            if (transform.GetChild(i).gameObject.name == "SelectCircle")
            {
                selectCircle = transform.GetChild(i).gameObject;
            }
        }

        animVR = modelVR.GetComponent<Animator>();
        animPC = modelPC.GetComponent<Animator>();

        //Set team
        if (tag == "VR Player's Unit")
            {
            team = 1;
            }
        else
        if (tag == "PC Player's Unit")
            {
            team = 2;
            }
        else
        if (tag == "AI Player's Unit")
        {
            team = 3;
        }

        //If this unit is not an AI unit then disable the AI Controller Component
        AIUnitController aiUnitController = GetComponent<AIUnitController>();
        if (team < 3)
            aiUnitController.enabled = false;
        
        SetTextureColors();

        //renderer.material.SetColor("_SpecColor", Color.red);
        }

    
    void SetTextureColors()
    {
        List<Transform> gs = new List<Transform>();
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < ts.Length; i++)
        {
            if (ts[i].tag == "ColorTex")
            {
                //Debug.Log(ts[i].name);
                Color teamColor;
                if (tag == "VR Player's Unit")
                {
                    teamColor = Color.blue;
                }
                else
                if (tag == "PC Player's Unit")
                {
                    teamColor = Color.red;
                }
                else
                if (tag == "AI Player's Unit")
                {
                    teamColor = Color.yellow;
                }
                else
                {
                    teamColor = Color.white;
                }
                ts[i].GetComponent<MeshRenderer>().material.SetColor("_Color", teamColor);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        /*
        float sli_rx = selectionIndicator[0].transform.eulerAngles.x;
        float sli_ry = selectionIndicator[0].transform.eulerAngles.y;
        float sli_rz = selectionIndicator[0].transform.eulerAngles.z;
        // + 1 * Time.deltaTime
        selectionIndicator[0].transform.Rotate(sli_rx, sli_ry, sli_rz); //rotation = selectionIndicator[0].transform.rotation + new Quaternion(0, 1 * Time.deltaTime, 0, 0);
        */
        

        if (agent.velocity.magnitude > .2f)
        transform.rotation = Quaternion.LookRotation(agent.velocity + new Vector3(0, 0, 0));

        if (modelVR.gameObject.activeSelf || modelPC.gameObject.activeSelf)
            Animating(agent.velocity);

        float height = -.02f; // .01f;

        selectCircle.SetActive(selected);
        
        if (hasTarget == true)
        {			
            if (Vector3.Distance(transform.position, targetPoint) < .1)
            {
                //If close to target stop drawing target indicator
                if (targetIndicatorSet)
                {
                    hasTarget = false;
                    Destroy(targetIndicator[0].gameObject);
                    //Destroy(targetIndicator[1].gameObject);
                    targetIndicatorSet = false;
					transform.GetComponent<AudioSource> ().Stop ();
					//FindObjectOfType<AudioManager> ().Stop ("SoldiersMarching");
                }
            }
            else
            if (!targetIndicatorSet)
            {
                targetIndicatorSet = true;

                //targetPoint.y + height
                //DRAW TARGET CROSS
                targetIndicator[0] = Instantiate(targetPrefab, new Vector3(targetPoint.x - .1f, .01f, targetPoint.z - .1f), Quaternion.identity);				
					/*DrawLine(new Vector3(targetPoint.x - .1f, targetPoint.y + height, targetPoint.z - .1f),
                         new Vector3(targetPoint.x + .1f, targetPoint.y + height, targetPoint.z + .1f), Color.blue, false);
				
                targetIndicator[1] = DrawLine(new Vector3(targetPoint.x - .1f, targetPoint.y + height, targetPoint.z + .1f),
                         new Vector3(targetPoint.x + .1f, targetPoint.y + height, targetPoint.z - .1f), Color.blue, false);*/
            }
        }

	}

    void Select()
    {
        selected = true;
    }

    public void SetTeam(int newteam)
    {
        team = newteam;
        if (newteam == 1)
        {
            tag = "VR Player's Unit";
        }
        else
        if (newteam == 2)
        {
            tag = "PC Player's Unit";
        }
        else
        if (newteam == 3)
        {
            tag = "AI Player's Unit";
        }

        SetTextureColors();
    }

    public void Target(Vector3 target)
    {
        //if (selected == true)
        //{
            if (targetIndicatorSet)
            {
                Destroy(targetIndicator[0]);
                targetIndicatorSet = false;				
            }
			transform.GetComponent<AudioSource> ().Play ();
            targetPoint = target + new Vector3(0,.05f,0);
            hasTarget = true;
            selected = false;
            agent.SetDestination(targetPoint);

        //}
    }
    
    void DeSelect()
    {
        selected = false;
    }

    void Animating(Vector3 movement)
    {
        
        bool running = movement.magnitude > 0.2f;
        bool fighting = GetComponent<Attack>().fighting;
        if (modelVR.gameObject.activeSelf)
        {
            animVR.SetBool("IsRunning", running);
            animVR.SetBool("IsFighting", fighting);
            animVR.speed = movement.magnitude * 3 + .5f;
        }
        if (modelPC.gameObject.activeSelf)
        {
            animPC.SetBool("IsRunning", running);
            animPC.SetBool("IsFighting", fighting);
            animPC.speed = movement.magnitude * 3 + .5f;
        }
        
    }

    //DRAWLINE SCRIPT ADAPTED FROM paranoidray's script from his answer on http://answers.unity3d.com/questions/8338/how-to-draw-a-line-using-script.html
    GameObject DrawLine(Vector3 start, Vector3 end, Color color, bool setParent) //, float duration = 0.05f)
    {
        GameObject myLine = new GameObject();
        
        if (setParent)
        {
            myLine.transform.SetParent(transform, false);
            
            Debug.Log("Parent set");
        }

        if (team == 1)
        {
            myLine.layer = 6;
        }
        if (team == 2)
        {
            myLine.layer = 5;
        }

        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();

        //Shader lineShader = new Shader();
        //lineShader.
        //lr.material = new Material(lineShader); // Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.01f, 0.01f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
        return myLine;
    }

}
