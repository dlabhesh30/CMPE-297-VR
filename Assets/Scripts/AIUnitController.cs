using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIUnitController : MonoBehaviour {

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
    void Start()
    {
        maxHealth = 10;
        health = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        model = transform.GetChild(0);

        rb = GetComponent<Rigidbody>();
        anim = model.GetComponent<Animator>();

        transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 0));

        if (tag == "AI Player's Unit")
        {
            team = 3;
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
    void Update () {
		
	}

    void Target(Vector3 target)
    {
        if (selected == true)
        {
            targetPoint = target + new Vector3(0, .05f, 0);
            hasTarget = true;
            selected = false;
            Debug.Log("Target chosen, deselected");
            agent.SetDestination(targetPoint);
        }
    }
    
    void Animating(Vector3 movement)
    {
        bool running = movement.magnitude > 0.2f;
        anim.SetBool("IsRunning", running);
        anim.speed = movement.magnitude * 3 + .5f;
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
