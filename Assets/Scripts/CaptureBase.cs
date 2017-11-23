using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureBase : MonoBehaviour {

    int team;
    float capturebaseTimer;

    GameObject[] flags;

    // Use this for initialization
    void Start () {

        flags = GameObject.FindGameObjectsWithTag("Flag");
        team = GetComponent<UnitController>().team;
        capturebaseTimer = 0;

    }

    // Update is called once per frame
    void Update() {
        //GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");

        capturebaseTimer -= Time.deltaTime;

        if (capturebaseTimer <= 0)
        {
            capturebaseTimer = .1f;
            GameObject nearestFlag = null;

            float smallestDist = float.MaxValue;

            for (int i = 0; i < flags.Length; i++)
            {
                GameObject currentFlag = flags[i];
                float dist;
                dist = Vector3.Distance(currentFlag.transform.position, transform.position);
                if (dist < smallestDist && dist < 2.5)
                {
                    smallestDist = dist;
                    nearestFlag = currentFlag;
                }
            }

            if (nearestFlag != null)
            {
                nearestFlag.GetComponent<FlagController>().Capture(team);
            }
        }
    }
}
