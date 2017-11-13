using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTeam : MonoBehaviour {

    public Transform unitPrefab, townHallPrefab;

    public int team; //1 = VR, 2 = PC, 3 = AI

    // Use this for initialization
    void Start ()
    {
        Transform townHall = Instantiate(townHallPrefab, transform.position + new Vector3(-2, 0, 0), Quaternion.identity);
        if (team == 1)
            townHall.tag = "VR Player's Building";
        else
        if (team == 2)
            townHall.tag = "PC Player's Building";
        else
        if (team == 3)
            townHall.tag = "AI Player's Building";

        for (int ix = 0; ix < 5; ix++)
        {
            for (int iy = 0; iy < 2; iy++)
            {
                Transform unit = Instantiate(unitPrefab, transform.position + new Vector3(2 + ix * .4f, 0, iy * .4f), Quaternion.identity);
                unit.GetComponent<UnitController>().SetTeam(team);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
