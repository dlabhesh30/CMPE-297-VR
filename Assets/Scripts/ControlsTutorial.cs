using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsTutorial : MonoBehaviour {

    public int tutorialStep;

    public Transform[] tutorialSlides;

    public GameObject tutorialBase;
    public FlagController flagController;

    // Use this for initialization
    void Start () {

        flagController = tutorialBase.GetComponent<FlagController>();

        tutorialStep = 0;

        tutorialSlides = gameObject.GetComponentsInChildren<Transform>();
        
        for (int i = 3; i < tutorialSlides.Length; i++)
        {
            tutorialSlides[i].gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (tutorialStep == 2 && flagController.team == 1 && flagController.captured >= flagController.capturedMax)
        {
            NextTutorialStep();
        }
    }

    public int GetTutorialStep()
    {
        return tutorialStep;
    }

    public void NextTutorialStep()
    {
        tutorialStep++;
        if (tutorialStep == 1)
        {
            tutorialSlides[1].gameObject.SetActive(false);
            tutorialSlides[2].gameObject.SetActive(false);
            tutorialSlides[3].gameObject.SetActive(true);
            tutorialSlides[4].gameObject.SetActive(true);
        }
        if (tutorialStep == 2)
        {
            tutorialSlides[3].gameObject.SetActive(false);
            tutorialSlides[4].gameObject.SetActive(false);
            tutorialSlides[5].gameObject.SetActive(true);
            //tutorialSlides[6].gameObject.SetActive(true);
        }
        if (tutorialStep == 3)
        {
            tutorialSlides[5].gameObject.SetActive(false);
            //tutorialSlides[4].gameObject.SetActive(false);
            tutorialSlides[6].gameObject.SetActive(true);
            //tutorialSlides[6].gameObject.SetActive(true);
        }
    }

}
