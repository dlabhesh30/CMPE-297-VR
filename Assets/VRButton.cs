using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRButton : MonoBehaviour {

    //public MonoBehaviour onClickAction;

    public Material restTex, hoverTex, clickTex;

    Vector3 startPos, targetPos;

    // Use this for initialization
    void Start () {
        targetPos = transform.position;
        startPos = transform.position;
        GetComponent<MeshRenderer>().material = restTex;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = (transform.position + targetPos)/2;
        targetPos = startPos;
        GetComponent<MeshRenderer>().material = restTex;
    }

    public void OnHover()
    {
        targetPos = startPos + new Vector3(-.05f,0,0);
        GetComponent<MeshRenderer>().material = hoverTex;
    }

    public void OnClick()
    {
        targetPos = startPos + new Vector3(.05f, 0, 0);
        //onClickAction.Invoke();
        GetComponent<MeshRenderer>().material = clickTex;
        transform.parent.GetComponent<VRButtonController>().OnClick(transform.name);
        //Debug.Log(transform.name);
        //transform.BroadcastMessage("OnClick",transform.name);
    }
}
