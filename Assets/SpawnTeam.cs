using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTeam : MonoBehaviour {

    public Transform unitPrefab, townHallPrefab;

    public int team; //1 = VR, 2 = PC, 3 = AI

    // Use this for initialization
    void Start ()
    {
        Instantiate(townHallPrefab, transform.position + new Vector3(-2,0,0), Quaternion.identity);
        for (int ix = 0; ix < 5; ix++)
        for (int iy = 0; iy < 2; iy++)
            {
                Transform newobj = Instantiate(unitPrefab, transform.position + new Vector3(2 + ix * .4f, 0, iy * .4f), Quaternion.identity);
                newobj.GetComponent<UnitController>().SetTeam(team);
            }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
