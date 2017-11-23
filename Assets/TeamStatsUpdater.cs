using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamStatsUpdater : MonoBehaviour {

    ResourceController resourceController;
    // Use this for initialization
    void Start () {

        GameObject resourceControllerGameObject = GameObject.FindGameObjectWithTag("Resource Controller");
        resourceController = resourceControllerGameObject.GetComponent<ResourceController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
