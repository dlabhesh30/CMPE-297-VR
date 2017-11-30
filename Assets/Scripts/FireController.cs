using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {

    GameObject fire;
    HealthBar healthBar;

    AudioManager audioManager;
    AudioSource audioSource;
    
// Use this for initialization
void Start () {
        
        audioManager = FindObjectOfType<AudioManager>();        

        healthBar = gameObject.GetComponent<HealthBar>();

        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < ts.Length; i++)
        {
            if (ts[i].tag == "Fire Effect")
            {
                fire = ts[i].gameObject;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        
        if (healthBar.health < healthBar.maxHealth / 2 && healthBar.underConstruction == false)
        {
			audioSource = transform.GetComponent<AudioSource>();
            var fireSound = audioSource;            
            fire.SetActive(true);
            if (!audioManager.isPlaying(fireSound))
            {
                audioManager.Play(fireSound);
            }
        }
        else
        {
            var fireSound = audioSource;
            audioManager.Stop(fireSound);
            fire.SetActive(false);
        }
	}
}
