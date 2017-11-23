using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour {

    public enum controlModeState
    {
        selecting,
        building_hut,
        building_turret,
        building_wall,
        building_gate

    }

    public controlModeState controlMode = controlModeState.selecting;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
