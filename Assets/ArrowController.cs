using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour {

    public int team;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        int hitTeam = 0;
        if (collision.gameObject.tag == "VR Player's Unit")
            hitTeam = 1;
        if (collision.gameObject.tag == "PC Player's Unit")
            hitTeam = 2;
        if (collision.gameObject.tag == "AI Player's Unit")
            hitTeam = 3;
        //Debug.Log("hitTeam = " + hitTeam);
        //Subtract from the health of the unit
        if (hitTeam != 0 && hitTeam != team)
        {
            collision.transform.GetComponent<HealthBar>().AddHealth(-8);
            Destroy(gameObject);
        }
    }
}
