using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingNumber : MonoBehaviour {

    float ystart;

    // Use this for initialization
    void Start()
    {
        ystart = transform.position.y;


    }

    // Update is called once per frame
    void Update () {
        transform.position += new Vector3(0, Time.deltaTime, 0);

        //transform.Rotate(0, transform.rotation.eulerAngles.y + Time.deltaTime, 0);

        if (transform.position.y > ystart + 2)
        {
            Destroy(gameObject, 0);
        }
    }
}
