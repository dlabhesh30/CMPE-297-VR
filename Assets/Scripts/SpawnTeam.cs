using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTeam : MonoBehaviour {

    public Transform unitPrefab, townHallPrefab;

    public int team; //1 = VR, 2 = PC, 3 = AI

    // Use this for initialization
    void Start ()
    {
        //Create team's town hall
        Transform townHall = Instantiate(townHallPrefab, transform.position + new Vector3(-2,0,0), Quaternion.identity);
        if (team == 1)
            townHall.tag = "VR Player's Building";
        if (team == 2)
            townHall.tag = "PC Player's Building";
        if (team == 3)
            townHall.tag = "AI Player's Building";
        
        //Create team's warriors
        for (int ix = 0; ix < 5; ix++)
        for (int iy = 0; iy < 2; iy++)
            {
                Transform newUnit = Instantiate(unitPrefab, transform.position + new Vector3(2 + ix * .4f, 0, iy * .4f), Quaternion.identity);
                newUnit.GetComponent<UnitController>().SetTeam(team);
                newUnit.gameObject.AddComponent<Bucketable>();
                newUnit.gameObject.AddComponent<PolygonOptimizer>();
            }
        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
