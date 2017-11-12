using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fpsUpdater : MonoBehaviour {

    Text fpsLabel;// = new Text();

    int count = 100;

	// Use this for initialization
	void Start () {
        fpsLabel = GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {
        if (count == 100)
        {
            float fps = 1.0f / Time.deltaTime;

            fpsLabel.text = "FPS: " +(int)fps;  //.SetText("FPS: "+fps);

            count = 0;
        }
        else
            count++;
    }
}
