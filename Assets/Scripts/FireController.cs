using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {

    GameObject fire;
    HealthBar healthBar;

    // Use this for initialization
    void Start () {
        healthBar = gameObject.GetComponent<HealthBar>();

        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < ts.Length; i++)
        {
            if (ts[i].tag == "Fire Effect")
            {
                fire = ts[i].gameObject;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        
        if (healthBar.health < healthBar.maxHealth / 2)
        {
            fire.SetActive(true);
        }
        else
        {
            fire.SetActive(false);
        }
	}
}
