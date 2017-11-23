using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIUnitController : MonoBehaviour
{

    UnitController unitController;

    float randx, randz;

    // Use this for initialization
    void Start()
    {
        randx = Random.Range(-1f, 1f);
        randz = Random.Range(-1f, 1f);
        //Debug.Log("HERE3");
    }

    // Update is called once per frame
    void Update()
    {
        unitController = GetComponent<UnitController>();
        //Debug.Log("HERE2");
        if (!unitController.hasTarget)
        {
            //Debug.Log("HERE");
            GameObject nearestUncapturedBase = NearestUncapturedBase(GetComponent<UnitController>().team);
            if (nearestUncapturedBase != null)
            {
                if (Vector3.Distance(transform.position, nearestUncapturedBase.transform.position) > 2)
                {
                    unitController.Target(
                    new Vector3(
                        nearestUncapturedBase.transform.position.x + randx,
                        transform.position.y,
                        nearestUncapturedBase.transform.position.z + randz)
                );
                }
            }
        }
        //If all the bases are captured, chase after remaining enemy units and buildings

    }

    public GameObject NearestUncapturedBase(int team)
    {
        GameObject[] Flags = new GameObject[0];
        Flags = GameObject.FindGameObjectsWithTag("Flag");

        GameObject nearestUncapturedBase = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < Flags.Length; i++)
        {
            FlagController flagController = Flags[i].GetComponent<FlagController>();
            float dist = Vector3.Distance(Flags[i].transform.position, transform.position);
            if (dist < closestDistance
                && (flagController.team != team
                || flagController.captured < flagController.capturedMax)
                )
            {
                closestDistance = dist;
                nearestUncapturedBase = Flags[i];
            }
        }

        return nearestUncapturedBase;
    }
}
