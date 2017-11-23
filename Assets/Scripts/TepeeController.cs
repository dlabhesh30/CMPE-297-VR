using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TepeeController : MonoBehaviour {

    public Transform unitPrefab;

    float spawnTimer = 10, spawnInterval = 10;

    ResourceController rc;
    HealthBar healthBar;


    GameSettings gameSettings;

    // Use this for initialization
    void Start ()
    {
        gameSettings = GameObject.FindGameObjectWithTag("GameSettings").GetComponent<GameSettings>();
        GameObject resourceController;
        resourceController = GameObject.FindGameObjectWithTag("Resource Controller");
        rc = resourceController.GetComponent<ResourceController>();
        healthBar = GetComponent<HealthBar>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!healthBar.underConstruction)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                //Determine team
                int team = 1;
                if (tag == "PC Player's Building")
                    team = 2;
                if (tag == "AI Player's Building")
                    team = 3;

                //Enforce max unit count
                bool canSpawn = false;

                if (team == 1 && rc.VRUnits < 100)
                    canSpawn = true;

                if (team == 2 && rc.PCUnits < 100)
                    canSpawn = true;

                if (team == 3 && rc.AIUnits < 100)
                    canSpawn = true;

                if (canSpawn)
                {
                    //Spawn Units
                    Transform newUnit = Instantiate(unitPrefab, transform.position + Quaternion.Euler(0, transform.localEulerAngles.y, 0) * new Vector3(0, 0, 1f), Quaternion.identity);

                    newUnit.gameObject.AddComponent<Bucketable>();
                    newUnit.gameObject.AddComponent<PolygonOptimizer>();
                    //Increment resource controller unit count
                    switch (team)
                    {
                        case 1:
                            rc.VRUnits += 1;
                            break;
                        case 2:
                            rc.PCUnits += 1;
                            break;
                        case 3:
                            rc.AIUnits += 1;
                            break;
                    }

                    //Set the units team
                    newUnit.GetComponent<UnitController>().SetTeam(team);
                }
                //Reset spawn timer
                spawnTimer = spawnInterval;
            }
        }
    }
}
