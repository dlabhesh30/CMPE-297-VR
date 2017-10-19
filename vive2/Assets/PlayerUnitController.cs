using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerUnitController : MonoBehaviour {

    public bool selected;

    public Vector3 targetPoint;

    Animator anim;

    public bool hasTarget = false;

    public float health, maxHealth;

    public int team;

    GameObject healthBarBack, healthBarFront;
    Vector3 healthBarFrontScaleStart;

    NavMeshAgent agent;
    Rigidbody rb;

    Transform model;

    // Use this for initialization
    void Start () {

        maxHealth = 10;
        health = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        
        model = transform.GetChild(0);

        rb = GetComponent<Rigidbody>();
        anim = model.GetComponent<Animator>();

        transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 0));
        
        if (tag == "VR Player's Unit")
            {
            team = 1;
            }
        else
        if (tag == "PC Player's Unit")
            {
            team = 2;
            }

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
                {
                    teamColor = Color.red;
                }
                ts[i].GetComponent<MeshRenderer>().material.SetColor("_Color", teamColor);
            }
            if (ts[i].tag == "HealthBarFront")
            {
                healthBarFront = ts[i].gameObject;
                healthBarFrontScaleStart = healthBarFront.transform.localScale;
            }
            if (ts[i].tag == "HealthBarBack")
            {
                healthBarBack = ts[i].gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        //If health is less than max health, show health bar
        if (health < maxHealth)
        {
            healthBarFront.SetActive(true);
            healthBarBack.SetActive(true);
            healthBarFront.transform.localScale = new Vector3(healthBarFront.transform.localScale.x, healthBarFrontScaleStart.y * health / maxHealth, healthBarFront.transform.localScale.z);
            
        }
        else //If at full health don't show health bar
        {
            healthBarFront.SetActive(false);
            healthBarBack.SetActive(false);
        }
        if (agent.velocity.magnitude > .2f)
        transform.rotation = Quaternion.LookRotation(agent.velocity + new Vector3(0, 0, 0));

        Animating(agent.velocity);

        GameObject[] enemies = new GameObject[0];

        if (tag == "VR Player's Unit")
        {
            enemies = GameObject.FindGameObjectsWithTag("PC Player's Unit");
        }
        if (tag == "PC Player's Unit")
        {
            enemies = GameObject.FindGameObjectsWithTag("VR Player's Unit");
        }

        GameObject nearestEnemy = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < enemies.Length; i++)
        {
            float dist = Vector3.Distance(enemies[i].transform.position, transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearestEnemy = enemies[i];
            }
        }

        //BATTLE!!!
        if (nearestEnemy != null && Vector3.Distance(nearestEnemy.transform.position, transform.position) < 1)
        {
            //nearestEnemy.health--;
            nearestEnemy.BroadcastMessage("AddHealth", -1f * Time.deltaTime);
        }
        float height = .05f;
        if (selected)
        {
            //top
            DrawLine(new Vector3(x - .1f, y + height, z - .1f),
                     new Vector3(x + .1f, y + height, z - .1f), Color.blue);

            //bottom
            DrawLine(new Vector3(x - .1f, y + height, z + .1f),
                     new Vector3(x + .1f, y + height, z + .1f), Color.blue);

            //left
            DrawLine(new Vector3(x - .1f, y + height, z - .1f),
                     new Vector3(x - .1f, y + height, z + .1f), Color.blue);

            //right
            DrawLine(new Vector3(x + .1f, y + height, z - .1f),
                     new Vector3(x + .1f, y + height, z + .1f), Color.blue);
        }

        if (hasTarget == true)
        {
            //DRAW TARGET CROSS
            DrawLine(new Vector3(targetPoint.x - .1f, targetPoint.y + height, targetPoint.z - .1f),
                     new Vector3(targetPoint.x + .1f, targetPoint.y + height, targetPoint.z + .1f), Color.blue);
            
            DrawLine(new Vector3(targetPoint.x - .1f, targetPoint.y + height, targetPoint.z + .1f),
                     new Vector3(targetPoint.x + .1f, targetPoint.y + height, targetPoint.z - .1f), Color.blue);
        }

	}

    void Select()
    {
        selected = true;
    }

    void SetTeam(int newteam)
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

        SetTextureColors();
    }

    void Target(Vector3 target)
    {
        if (selected == true)
        {
            targetPoint = target + new Vector3(0,.05f,0);
            hasTarget = true;
            selected = false;
            Debug.Log("Target chosen, deselected");
            agent.SetDestination(targetPoint);
        }
    }
    
    void DeSelect()
    {
        selected = false;
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
        GameObject.Destroy(myLine, duration);
    }

    //Adds a value healthToAdd to current health value, this can be a positive or negative number
    public void AddHealth(float healthToAdd)
    {
        health += healthToAdd;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
