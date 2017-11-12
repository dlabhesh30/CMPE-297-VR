using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    GameObject healthBarBack, healthBarFront;
    public float health, maxHealth;
    Vector3 healthBarFrontScaleStart, healthBarFrontPosStart;

    // Use this for initialization
    void Start()
    {
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < ts.Length; i++)
        {
            if (ts[i].tag == "HealthBarFront")
            {
                healthBarFront = ts[i].gameObject;
                healthBarFrontScaleStart = healthBarFront.transform.localScale; // Vector3.Scale( healthBarFront.transform.lossyScale, healthBarFront.transform.localScale); // lossyScale; //healthBarFront.transform.localScale * 
                healthBarFrontPosStart = healthBarFront.transform.position;
                //Debug.Log("Here1");
            }
            if (ts[i].tag == "HealthBarBack")
            {
                healthBarBack = ts[i].gameObject;
                //Debug.Log("Here2");
            }
        }
        //Debug.Log(gameObject.name);
        //healthBarFront.SetActive(false);
        //healthBarBack.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //If health is less than max health, show health bar
        if (health < maxHealth)
        {
            healthBarFront.SetActive(true);
            healthBarBack.SetActive(true);                                                          //startPos.y - startScale.y + captured / capturedMax * startScale.y

            //healthBarFront.transform.position = new Vector3(healthBarFront.transform.position.x, healthBarFrontPosStart.y - healthBarFrontScaleStart.y * 2 + health / maxHealth * healthBarFrontScaleStart.y , healthBarFront.transform.position.z);

            healthBarFront.transform.localScale = new Vector3(healthBarFront.transform.localScale.x, healthBarFrontScaleStart.y * health / maxHealth, healthBarFront.transform.localScale.z);
        }
        else //If at full health don't show health bar
        {
            healthBarFront.SetActive(false);
            healthBarBack.SetActive(false);
        }
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
