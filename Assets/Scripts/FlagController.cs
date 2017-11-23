using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour {
    public Transform healthBarOuter, ring; //whiteFlag, blueFlag, redFlag,

    //public GameObject tutorialGameObject;
    //ControlsTutorial tutorial;

    bool createdFort = false;
    const int 
        VRteam = 1,
        PCteam = 2,
        AIteam = 3;

    public int team;

    public float captured, capturedMax, barUpdateTimer;

    Vector3 startScale, startPos;
    
    float coinTimer = 1;

    public Transform coinPrefab, unitPrefab;

    GameObject flag, resourceControllerGameObject;

    public Transform tepeePrefab, wallPrefab, turretPrefab;

    ResourceController resourceController;
    GameSettings gameSettings;

    // Use this for initialization
    void Start()
    {
        gameSettings = GameObject.FindGameObjectWithTag("GameSettings").GetComponent<GameSettings>();
        barUpdateTimer = 0;
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
        barUpdateTimer -= Time.deltaTime;

        if (captured == capturedMax)
        {
            coinTimer -= Time.deltaTime;

            //Create Coins
            if (coinTimer <= 0)
            {
                coinTimer = 5;
                Transform newobj = Instantiate(coinPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.Euler(90, 0, 0));

                //Transform newobj = Instantiate(unitPrefab, transform.position + new Vector3(Random.value*2-1, 0, Random.value * 2 - 1), Quaternion.Euler(90, 0, 0));
                //newobj.BroadcastMessage("SetTeam" , team);
                //GameObject polyOptimizer = GameObject.FindGameObjectWithTag("PolygonOptimizer");
                //polyOptimizer.GetComponent<GlobalPolygonOptimizer>().addModel(newobj.gameObject.GetComponent<DistancePolyOptimizeInit>());

                //Add coins
                //coins += 
                if (team == 1)
                    resourceController.addVRCoins(10);
                if (team == 2)
                    resourceController.addPCCoins(10);
                if (team == 3)
                    resourceController.addAICoins(10);

                //Debug.Log("Coin Created");
                //Debug.Log("Unit Created");
            }
        }

        if (barUpdateTimer <= 0)
        {
            barUpdateTimer = .1f;

            healthBarOuter.position = new Vector3(startPos.x, startPos.y - startScale.y + captured / capturedMax * startScale.y, startPos.z);
            
            healthBarOuter.transform.localScale = new Vector3(startScale.x, captured / capturedMax * startScale.y, startScale.z);
        }
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
    
    public void Capture(int unitTeam)
    {
        if (team == unitTeam)
        {
            if (captured < capturedMax)
            {
                captured += 10 * Time.deltaTime * gameSettings.gameSpeed;
                if (captured > capturedMax)
                {
                    
                    var trumpetSound = transform.GetComponent<AudioSource>();
                    FindObjectOfType<AudioManager>().Play(trumpetSound);
                    captured = capturedMax;
                    if (team == VRteam)
                    {
                        SetAllColors(Color.blue);
                        createdFort = false;
                    }
                    if (team == PCteam)
                    {
                        SetAllColors(Color.red);
                        createdFort = false;
                    }
                    if (team == AIteam)
                    {
                        SetAllColors(Color.yellow);

                        if (createdFort == false)
                        {
                            createdFort = true;
                            double walls, tepees, turrets, wallsDistance, tepeesDistance;

                            walls = 34d;
                            tepees = 4;

                            wallsDistance = 3;
                            tepeesDistance = 1.5f;

                            //If captured by AI then create AI buildings
                            //for (int dir = 0; dir < 360; dir += 360 / (walls - 1))
                            for (double i = 0d; i < tepees; i += 1d)
                            {
                                double dir = (360d / (tepees)) * i + 45;
                                Transform newTepee = Instantiate(tepeePrefab,
                                    new Vector3((float)(Math.Cos(dir * Mathf.Deg2Rad) * tepeesDistance + transform.position.x),
                                    0, (float)(Math.Sin(dir * Mathf.Deg2Rad) * tepeesDistance + transform.position.z)),
                                    Quaternion.Euler(0, (float)(-dir - 90 + UnityEngine.Random.Range(-10, 10)), 0));

                                newTepee.gameObject.AddComponent<Bucketable>();
                                newTepee.gameObject.AddComponent<PolygonOptimizer>();
                                newTepee.tag = "AI Player's Building";
                            }
                            for (double i = 0d; i < walls; i += 1d)
                            {
                                double dir = (360d / (walls)) * i;        // i / (walls+1f) * 360     
                                if (Math.Abs(dir) > 20
                                    && Math.Abs(dir - 90) > 20
                                    && Math.Abs(dir - 180) > 20
                                    && Math.Abs(dir - 270) > 20)
                                {
                                    Transform newWall = Instantiate(wallPrefab,
                                        new Vector3((float)(Math.Cos(dir * Mathf.Deg2Rad) * wallsDistance + transform.position.x),
                                        0, (float)(Math.Sin(dir * Mathf.Deg2Rad) * wallsDistance + transform.position.z)),
                                        Quaternion.Euler(0, (float)(-dir + 90 + UnityEngine.Random.Range(-10, 10)), 0));

                                    newWall.gameObject.AddComponent<Bucketable>();
                                    newWall.gameObject.AddComponent<PolygonOptimizer>();
                                    newWall.tag = "AI Player's Building";
                                }
                                else
                                if (Math.Abs(dir) > 10
                                    && Math.Abs(dir - 90) > 10
                                    && Math.Abs(dir - 180) > 10
                                    && Math.Abs(dir - 270) > 10)
                                {
                                    Transform newTurret = Instantiate(turretPrefab,
                                        new Vector3((float)(Math.Cos(dir * Mathf.Deg2Rad) * wallsDistance + transform.position.x),
                                        0, (float)(Math.Sin(dir * Mathf.Deg2Rad) * wallsDistance + transform.position.z)),
                                        Quaternion.Euler(0, (float)(-dir + 90 + UnityEngine.Random.Range(-10, 10)), 0));
                                    
                                    newTurret.gameObject.AddComponent<Bucketable>();
                                    newTurret.gameObject.AddComponent<PolygonOptimizer>();
                                    newTurret.tag = "AI Player's Building";
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            captured -= 10 * Time.deltaTime;
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
