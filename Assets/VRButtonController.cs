using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRButtonController : MonoBehaviour {

    public string lastButtonClicked = "";

    public void OnClick(string buttonName)
    {
        lastButtonClicked = buttonName;
    }
}
