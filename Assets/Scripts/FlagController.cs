using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour {
    public Transform healthBarOuter, ring; //whiteFlag, blueFlag, redFlag,

    //public GameObject tutorialGameObject;
    //ControlsTutorial tutorial;

    const int 
        VRteam = 1,
        PCteam = 2,
        AIteam = 3;

    public int team;

    public float captured, capturedMax;

    Vector3 startScale, startPos;
    
    float coinTimer = 1;

    public Transform coinPrefab, unitPrefab;

    GameObject flag, resourceControllerGameObject;

    ResourceController resourceController;

    // Use this for initialization
    void Start() {
        //tutorial = GameObject.FindGameObjectWithTag("Tutorial").GetComponent<ControlsTutorial>(); //tutorialGameObject.GetComponent<ControlsTutorial>(); //FindGameObjectWithTag("Tutorial").GetComponent<ControlsTutorial>();

        resourceControllerGameObject = GameObject.FindGameObjectWithTag("Resource Controller");

        resourceController = resourceControllerGameObject.GetComponent<ResourceController>();

        if (tag == "VR Player's Unit")
        {
            team = VRteam;
        }
        if (tag == "PC Player's Unit")
        {
            team = PCteam;
        }
        if (tag == "AI Player's Unit")
        {
            team = AIteam;
        }

        flag = transform.GetChild(0).gameObject;
        capturedMax = 200;
        
        healthBarOuter = transform.GetChild(2);
        ring = transform.GetChild(3);
        captured = 0;
        
        startScale = healthBarOuter.transform.localScale;
        startPos = healthBarOuter.position;
    }

    // Update is called once per frame
    void Update() {

        healthBarOuter.position = new Vector3(startPos.x, startPos.y - startScale.y + captured / capturedMax * startScale.y, startPos.z);

        if (captured == capturedMax)
        {
            //Create Coins
            coinTimer -= Time.deltaTime;
            if (coinTimer <= 0)
            {
                coinTimer += 5;
                Transform newobj = Instantiate(coinPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.Euler(90, 0, 0));
                //Transform newobj = Instantiate(unitPrefab, transform.position + new Vector3(Random.value*2-1, 0, Random.value * 2 - 1), Quaternion.Euler(90, 0, 0));
                //newobj.BroadcastMessage("SetTeam" , team);
                GameObject polyOptimizer = GameObject.FindGameObjectWithTag("PolygonOptimizer");
                polyOptimizer.GetComponent<GlobalPolygonOptimization>().addModel(newobj.gameObject.GetComponent<DistancePolyOptimizeInit>());

                if (team == 1)
                    resourceController.addVRCoins(1);
                if (team == 2)
                    resourceController.addPCCoins(1);
                if (team == 3)
                    resourceController.addAICoins(1);

                //Debug.Log("Coin Created");
                //Debug.Log("Unit Created");
            }
        }

        healthBarOuter.transform.localScale = new Vector3(startScale.x, captured / capturedMax * startScale.y, startScale.z);
    }

    void SetAllColors(Color newColor)
    {
        SetFlagColor(newColor);
        SetBarColor(newColor);
        SetRingColor(newColor);
    }

    void SetFlagColor(Color newColor)
    {
        Renderer flag_rend = flag.GetComponent<Renderer>();

        flag_rend.material.SetColor("_Color", newColor);
    }

    void SetBarColor(Color newColor)
    {
        Renderer bar_rend = healthBarOuter.GetComponent<Renderer>();

        bar_rend.material.SetColor("_Color", newColor);
    }

    void SetRingColor(Color newColor)
    {
        Renderer ring_rend = ring.GetComponent<Renderer>();

        ring_rend.material.SetColor("_Color", newColor);
    }
    
    void Capture(int unitTeam)
    {
        if (team == unitTeam)
        {
            if (captured < capturedMax)
            {
                captured += 1 * Time.deltaTime;
                if (captured > capturedMax)
                {
                    var trumpetSound = transform.GetComponent<AudioSource>();
                    FindObjectOfType<AudioManager>().Play(trumpetSound);
                    captured = capturedMax;
                    if (team == VRteam)
                    {
                        SetAllColors(Color.blue);
                    }
                    if (team == PCteam)
                    {
                        SetAllColors(Color.red);
                    }
                    if (team == AIteam)
                    {
                        SetAllColors(Color.yellow);
                    }
                }
            }
        }
        else
        {
            captured -= 1 * Time.deltaTime;
            if (captured <= 0)
            {
                team = unitTeam;
                captured = 0;
                
                SetAllColors(Color.white);

                if (team == VRteam)
                {
                    SetBarColor(Color.blue);
                }
                if (team == PCteam)
                {
                    SetBarColor(Color.red);
                }
                if (team == AIteam)
                {
                    SetBarColor(Color.yellow);
                }
            }
        }
    }
}
