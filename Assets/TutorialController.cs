using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public int tutorialStep;
    List<string> tutorialText;
    TextMesh messageText;

    VRButtonController vrButtonController;

	// Use this for initialization
	void Start () {
        vrButtonController = GetComponent<VRButtonController>();
        tutorialStep = 0;
        tutorialText = new List<string>();
        tutorialText.Add(ParagraphString("Welcome to Strategeality. This tutorial will teach you the controls. Click next to continue.",35));
        tutorialText.Add(ParagraphString("To move your units, press the trigger and point at the ground and draw a rectangle around them, then point and click again to tell them where to go. Click next to continue.", 35));
        tutorialText.Add(ParagraphString("Bases produce gold when captured. To capture a base, move your warriors within it's radius.",35));
        tutorialText.Add(ParagraphString("Once the bar gets full, and the flag turns blue, then you have captured the base, and you will start to see coins floating out of it.",35));
        tutorialText.Add(ParagraphString("Once you have captured the base, you can fortify it with some defensive structures.",35));
        tutorialText.Add(ParagraphString("To build some walls, swipe left on the Vive pad with your thumb until the wall is on top, then you can click and drag on the ground to place them.",35));
        tutorialText.Add(ParagraphString("Buildings cost coins. To see how many coins you currently have, look at the indicator on your Vive controller.",35));
        tutorialText.Add(ParagraphString("To stop building some walls, swipe left on the Vive pad with your thumb until 'Select' is on top. You can now select units again.", 35));
        tutorialText.Add(ParagraphString("You can teleport around the map by pointing to where you want to teleport and then clicking down on the Vive pad.",35));
        tutorialText.Add(ParagraphString("To win the game, defeat all your opponents, or destroy their town hall.",35));
        tutorialText.Add(ParagraphString("You will lose the game if all your people are killed or your town hall is destroyed, so protect them at all costs.",35));
        messageText = transform.GetChild(0).GetComponent<TextMesh>();
        updateTutorialText();
    }
    string ParagraphString(string str , int maxCharactersPerLine)
    {
        int charactersSinceLastNewLine = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (charactersSinceLastNewLine > maxCharactersPerLine && str[i] == ' ')
            {
                str = replaceCharAt(str, i, '\n');
                //Debug.Log("replaced");
                charactersSinceLastNewLine = 0;
            }
            charactersSinceLastNewLine++;
        }

        return str;
    }
    public void Update()
    {
        if (vrButtonController.lastButtonClicked != "")
        {
            OnClick(vrButtonController.lastButtonClicked);
            vrButtonController.lastButtonClicked = "";
        }
    }

    public void OnClick( string buttonName )
    {
        if (buttonName == "NextButton")
        {

            if (tutorialStep < tutorialText.Count)
            {
                tutorialStep++;
                updateTutorialText();
                if (tutorialStep >= 10)
                {
                    Destroy(transform.gameObject);
                }
            }
        }
        if (buttonName == "ExitButton")
        {
            Destroy(transform.gameObject);
        }
    }

    void updateTutorialText()
    {
        messageText.text = tutorialText[tutorialStep];
    }

    string replaceCharAt(string str, int pos, char newChar)
    {
        // Sample: We want to replace 'd' with 'X' (index=3)
        string somestring = str;
        char[] ch = somestring.ToCharArray();
        ch[pos] = newChar; // index starts at 0!
        string newstring = new string(ch);
        return newstring;
    }
}
