using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour {
    public Transform whiteFlag, blueFlag, redFlag, healthBarOuter, ring;

    public int team;
    
    float captured, capturedMax;

    Vector3 startScale;


    // Use this for initialization
    void Start() {
        capturedMax = 200;
        whiteFlag = transform.GetChild(0);
        blueFlag = transform.GetChild(1);
        redFlag = transform.GetChild(2);
        healthBarOuter = transform.GetChild(3);
        ring = transform.GetChild(4);
        captured = 0;

        whiteFlag.gameObject.SetActive(true);
        blueFlag.gameObject.SetActive(false);
        redFlag.gameObject.SetActive(false);

        startScale = healthBarOuter.transform.localScale;

    }

    // Update is called once per frame
    void Update() {
        if (captured == 100)
        {
            //Create Coins
        }

        healthBarOuter.transform.localScale = new Vector3(startScale.x, captured / capturedMax * startScale.y, startScale.z);
    }

    void Capture(int unitTeam)
    {
        if (team == unitTeam)
        {
            if (captured < capturedMax)
            {
                captured += 1 * Time.deltaTime;
                if (captured >= capturedMax)
                {
                    captured = capturedMax;
                    whiteFlag.gameObject.SetActive(false);
                    //Debug.Log("White flag disabled, team = "+team+" unitTeam = "+unitTeam);

                    Renderer flag_rend = healthBarOuter.GetComponent<Renderer>();
                    Renderer ring_rend = ring.GetComponent<Renderer>();

                    if (team == 1)
                    {
                        blueFlag.gameObject.SetActive(true);
                        redFlag.gameObject.SetActive(false);
                    }
                    if (team == 2)
                    {
                        blueFlag.gameObject.SetActive(false);
                        redFlag.gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            captured -= 1 * Time.deltaTime;
            if (captured <= 0)
            {
                Debug.Log("team set to" + unitTeam);
                team = unitTeam;
                captured = 0;
                blueFlag.gameObject.SetActive(false);
                redFlag.gameObject.SetActive(false);
                whiteFlag.gameObject.SetActive(true);

                Renderer flag_rend = healthBarOuter.GetComponent<Renderer>();
                Renderer ring_rend = ring.GetComponent<Renderer>();

                if (team == 1)
                {
                    flag_rend.material.SetColor("_Color", Color.blue);
                    ring_rend.material.SetColor("_Color", Color.blue);
                }
                if (team == 2)
                {
                    flag_rend.material.SetColor("_Color", Color.red);
                    ring_rend.material.SetColor("_Color", Color.red);
                }

                //flag_rend.material.SetColor("_Color", Color.white);
                //ring_rend.material.SetColor("_Color", Color.white);
                
            }
        }
    }
}
