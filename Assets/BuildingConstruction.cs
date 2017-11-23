using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConstruction : MonoBehaviour {

    float constructed;
    public float secondsToConstruct; //number of seconds it takes to build this building
    HealthBar healthBar;

    // Use this for initialization
    void Start () {
        constructed = 0;

        healthBar = GetComponent<HealthBar>();
        healthBar.health = 1;

        healthBar.setFrontBarColor(Color.yellow);
        healthBar.setBackBarColor(Color.black);
    }
	
	// Update is called once per frame
	void Update () {
        if (constructed < secondsToConstruct)
        {
            constructed = healthBar.health / healthBar.maxHealth * secondsToConstruct;
            constructed += Time.deltaTime;
            healthBar.health = constructed / secondsToConstruct * healthBar.maxHealth;
            if (constructed >= secondsToConstruct)
            {
                healthBar.underConstruction = false;
                healthBar.setFrontBarColor(Color.green);
                healthBar.setBackBarColor(Color.red);
            }
        }


    }
}
