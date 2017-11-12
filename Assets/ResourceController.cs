using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour {

    public int VRCoins, PCCoins, AICoins, costWall, costTepee, costTurret;

    public Transform PCCoinsLabel, VRCoinsLabel, wallCostLabelPC, tepeeCostLabelPC, turretCostLabelPC, wallCostLabelVR, tepeeCostLabelVR, turretCostLabelVR;

    // Use this for initialization
    void Start () {
        VRCoins = 250;
        PCCoins = 250;
        AICoins = 250;

        costWall = 5;
        costTepee = 50;
        costTurret = 40;

        wallCostLabelPC.GetComponent<Text>().text = "" + costWall;
        tepeeCostLabelPC.GetComponent<Text>().text = "" + costTepee;
        turretCostLabelPC.GetComponent<Text>().text = "" + costTurret;

        wallCostLabelVR.GetComponent<TextMesh>().text = "" + costWall;
        tepeeCostLabelVR.GetComponent<TextMesh>().text = "" + costTepee;
        turretCostLabelVR.GetComponent<TextMesh>().text = "" + costTurret;

        //updateVRCoinLabel();
        //updatePCCoinLabel();
    }

    public void addVRCoins(int coins)
    {
        VRCoins += coins;
        updateVRCoinLabel();
    }

    public void addPCCoins(int coins)
    {
        PCCoins += coins;
        updatePCCoinLabel();
    }

    public void addAICoins(int coins)
    {
        AICoins += coins;
    }

    public void updateVRCoinLabel()
    {
        VRCoinsLabel.GetComponent<TextMesh>().text = "" + VRCoins;
    }

    public void updatePCCoinLabel()
    {
        PCCoinsLabel.GetComponent<Text>().text = "Coins: " + PCCoins;
    }

    // Update is called once per frame
    //void Update () {

    //}
}
