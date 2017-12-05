using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

	public int team;

	[HideInInspector]
	public GameObject arrowOwner;

	
	void Start(){
		//destroy arrow after 5 seconds
		Destroy(gameObject, 5);
	}

	void OnTriggerEnter(Collider other){
		//freeze arrow when it hits an enemy and parent it to the enemy to move with it
			int hitTeam = 0;
			if (other.gameObject.tag == "VR Player's Unit")
				hitTeam = 1;
			if (other.gameObject.tag == "PC Player's Unit")
				hitTeam = 2;
			if (other.gameObject.tag == "AI Player's Unit")
				hitTeam = 3;
			//Debug.Log("hitTeam = " + hitTeam);
			//Subtract from the health of the unit
			if (hitTeam != 0 && hitTeam != team)
			{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().isKinematic = true;
		transform.parent = other.gameObject.transform;
			// detuct health from the hit object
			Debug.Log("Hit warrior " + transform.parent.gameObject.name);
			transform.parent.transform.GetComponent<HealthBar>().AddHealth(-10);
		}
		else if(other.gameObject.tag == "GroundPlane"){
		//destroy arrow when it hits the ground
		Destroy(gameObject);	
		}
	}

	void OnCollisionEnter(Collision collision)
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
			collision.transform.GetComponent<HealthBar>().AddHealth(-10);
			Destroy(gameObject);
		}
	}
}
