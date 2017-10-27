using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerUnitController : MonoBehaviour {

    //Animation Variables
    Animator anim;

    //Team Variables
    public int team;

    //Healthbar Variables
    //GameObject healthBarBack, healthBarFront;
    //public float health, maxHealth;
    //Vector3 healthBarFrontScaleStart;

    //Target and Selection Indicator Variables
    GameObject selectCircle;
    public Vector3 targetPoint;
    public bool selected, hasTarget = false;
    bool targetIndicatorSet = false, selectionIndicatorSet = false;
    GameObject[] targetIndicator = new GameObject[2], selectionIndicator = new GameObject[4];

    //Pathfinding and Collisions Variables
    NavMeshAgent agent;
    Rigidbody rb;
    Transform model;

    public GameObject targetPrefab;

    // Use this for initialization
    void Start ()
    {
        //maxHealth = 10;
        //health = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        
        model = transform.GetChild(0);

        rb = GetComponent<Rigidbody>();
        anim = model.GetComponent<Animator>();

        transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 0));
        selectCircle = transform.GetChild(1).gameObject;

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
            /*
            if (ts[i].tag == "HealthBarFront")
            {
                healthBarFront = ts[i].gameObject;
                healthBarFrontScaleStart = healthBarFront.transform.localScale;
            }
            if (ts[i].tag == "HealthBarBack")
            {
                healthBarBack = ts[i].gameObject;
            }*/
        }
    }
    /*
    public void AddHealth(float healthToAdd)
    {
        Component healthComponent = GetComponent<HealthBar>();
        healthComponent.BroadcastMessage("AddHealth", healthToAdd);
    }*/

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
        /*
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
        }*/

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
        else
        {
            //IF HAS NO NEAREST ENEMY IN RANGE, CHECK FOR ENEMY BUILDINGS THAT ARE IN RANGE
            GameObject[] enemyBuildings = new GameObject[0];

            if (tag == "VR Player's Unit")
            {
                enemies = GameObject.FindGameObjectsWithTag("PC Player's Building");
            }
            if (tag == "PC Player's Unit")
            {
                enemies = GameObject.FindGameObjectsWithTag("VR Player's Building");
            }

            closestDistance = float.MaxValue;
            for (int i = 0; i < enemies.Length; i++)
            {
                float dist = Vector3.Distance(enemies[i].transform.position, transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    nearestEnemy = enemies[i];
                }
            }
            if (nearestEnemy != null && Vector3.Distance(nearestEnemy.transform.position, transform.position) < 1)
            {
                //nearestEnemy.health--;
                nearestEnemy.BroadcastMessage("AddHealth", -1f * Time.deltaTime);
            }

        }


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
            if (targetIndicatorSet)
            {
                Destroy(targetIndicator[0]);
                targetIndicatorSet = false;
            }
            targetPoint = target + new Vector3(0,.05f,0);
            hasTarget = true;
            selected = false;
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
