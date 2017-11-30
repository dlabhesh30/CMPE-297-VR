using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuildingsPlacer : MonoBehaviour
{
    Vector3 startBuildPoint;
    public Transform buildingCostIndicator, floatingNumberPrefab;
	public AudioClip constructionSoundClip;
	public AudioClip errorSoundClip;
	BucketGridController bucketGrid;

    bool called;

    GameObject resourceControllerGameObject;
    ResourceController resourceController;
	AudioManager audioManager;

    void Start()
    {
        bucketGrid = GameObject.FindGameObjectWithTag("BucketGridController").GetComponent<BucketGridController>();
        resourceControllerGameObject = GameObject.FindGameObjectWithTag("Resource Controller");
        resourceController = resourceControllerGameObject.GetComponent<ResourceController>();
		audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        buildingCostIndicator.gameObject.SetActive(called);
        called = false;
    }

    //Team, 1 = VR, 2 = PC, 3 = AI

    public void PlaceBuilding(Transform buildingPrefab, Transform hoverModel,
        Vector3 point, float yRotation, bool primaryPressed, bool primaryHeld, bool primaryReleased, bool lineDraw, 
        int team, int costCoins, Vector3 cameraPosition)
    {
        Transform actualPlaceObj;

        int teamCoins = 0;
        if (team == 1)
            teamCoins = resourceController.VRCoins;
        if (team == 2)
            teamCoins = resourceController.PCCoins;

        //If the mouse is released actualPlaceObj should be the actual building prefab to create with all components attached
        actualPlaceObj = buildingPrefab;

        //If the primary button is not released actualPlaceObj should be a hover object without objects properties and components
        if (!primaryReleased)
        {
            actualPlaceObj = hoverModel;
        }

        hoverModel.position = point;
        hoverModel.eulerAngles = new Vector3(0, yRotation, 0);

        buildingCostIndicator.position = point + new Vector3(0, 1, 0);
        buildingCostIndicator.eulerAngles = new Vector3(0, 90 - Mathf.Rad2Deg * (Mathf.Atan2(buildingCostIndicator.position.z - transform.position.z, buildingCostIndicator.position.x - transform.position.x)), 0);

        called = true;
        //If primary button is pressed
        if (primaryPressed)
        {
            //If line drawing, such as for walls, then set the start point
            if (lineDraw)
            {
                startBuildPoint = point;
            }
        }

        bool canAfford = false;

        int buildingsCreated = 0;
        
        if (primaryHeld || primaryReleased)
        {
            //If line drawing, such as for walls, then draw the line of buildings
            
            float dis = Vector3.Distance(startBuildPoint, point);

            float xpos = startBuildPoint.x;
            float zpos = startBuildPoint.z;

            int increments = 1;

            bool created = false;
            string reasonCantBuild = "Can't build for unknown reason";

            //List<string> tags = new List<string>();
            //tags.Add("PC Player's Building");
            //tags.Add("VR Player's Building");
            //tags.Add("AI Player's Building");

            string checkTagUnits = "";
            string checkTagBuildings = "";

            if (team == 1)
            {
                checkTagUnits = "VR Player's Unit";
                checkTagBuildings = "VR Player's Building";
            }
            if (team == 2)
            {
                checkTagUnits = "PC Player's Unit";
                checkTagBuildings = "PC Player's Building";
            }
            if (team == 3)
            {
                checkTagUnits = "AI Player's Unit";
                checkTagBuildings = "AI Player's Building";
            }


            bool spaceOccupied = false, inRangeOfUnit = false;


            if (dis > .5f && lineDraw)
            {
                dis = Math.Min(dis, 4);
                increments = Math.Min((int)(dis / .4f), 10);

                canAfford = (costCoins * increments <= teamCoins);
                
                if (!canAfford)
                {
                    reasonCantBuild = "Not enough coins";
					AudioSource buildingsAudioSource = gameObject.GetComponent<AudioSource> ();
					audioManager.PlayOneShot(buildingsAudioSource,errorSoundClip);
                }

                //Debug.Log("INCREMENTS: " + increments);

                float dir = -Mathf.Atan2(point.z - startBuildPoint.z, point.x - startBuildPoint.x) * 180 / Mathf.PI;
                //Angle(new Vector2(startBuildPoint.x, startBuildPoint.z) , new Vector2(placeObj.position.x, placeObj.position.z));

                buildingsCreated = increments;

                for (int i = 0; i <= increments; i++)
                {
                    Transform nearestBuilding = bucketGrid.getNearestObject(new Vector2(xpos, zpos), 6, checkTagBuildings);
                    Transform nearestUnit = bucketGrid.getNearestObject(new Vector2(xpos, zpos), 20, checkTagUnits);

                    float distanceToNearestBuilding, distanceToNearestUnit;
                }

                for (int i = 0; i <= increments; i++)
                {
                    Transform newBuilding = Instantiate(buildingPrefab, new Vector3(xpos, 0, zpos), new Quaternion(0, 0, 0, 0)); // startBuildPoint.y
                    newBuilding.eulerAngles = new Vector3(0, dir, 0);

                    if (team == 1)
                        newBuilding.tag = "VR Player's Building";
                    else
                    if (team == 2)
                        newBuilding.tag = "PC Player's Building";

                    if (!primaryReleased || !canAfford)
                    {
                        if (primaryReleased && !canAfford)
                        {
                            reasonCantBuild = "Not enough coins";
							AudioSource buildingsAudioSource = gameObject.GetComponent<AudioSource> ();
							audioManager.PlayOneShot(buildingsAudioSource,errorSoundClip);
                        }
                        GameObject.Destroy(newBuilding.gameObject, .02f);
                    }
                    else 
                    {
                        created = true;
                        newBuilding.gameObject.AddComponent<Bucketable>();
                        newBuilding.gameObject.AddComponent<PolygonOptimizer>();
						AudioSource buildingsAudioSource = gameObject.GetComponent<AudioSource> ();
						audioManager.PlayOneShot (buildingsAudioSource,constructionSoundClip);
                    }
                    xpos += Mathf.Cos(-Mathf.Deg2Rad * dir) * (dis / increments);
                    zpos += Mathf.Sin(-Mathf.Deg2Rad * dir) * (dis / increments);
                }
            }
            else if (primaryReleased)
            {
                
                
                /*
                float distanceToNearestBuilding;
                Transform nearestBuilding = bucketGrid.getNearestObject(new Vector2(point.x, point.z), 6, checkTagBuildings);

                if (nearestBuilding != null)
                {
                    distanceToNearestBuilding = Vector2.Distance(new Vector2(point.x, point.z), new Vector2(nearestBuilding.position.x, nearestBuilding.position.z));
                    if (distanceToNearestBuilding < .45f)
                    {
                        spaceOccupied = true;
                        reasonCantBuild = "Too close to building";
                    }
                }*/

                if (!PositionEmpty(point, checkTagBuildings))
                {
                    spaceOccupied = true;
                    reasonCantBuild = "Too close to building";
					AudioSource buildingsAudioSource = gameObject.GetComponent<AudioSource> ();
					audioManager.PlayOneShot(buildingsAudioSource,errorSoundClip);
                }

                Transform nearestUnit = bucketGrid.getNearestObject(new Vector2(point.x, point.z), 20, checkTagUnits);
                float distanceToNearestUnit;

                if (nearestUnit != null)
                {
                    distanceToNearestUnit = Vector2.Distance(new Vector2(point.x, point.z), new Vector2(nearestUnit.position.x, nearestUnit.position.z));
                    if (distanceToNearestUnit < 7f)
                    {
                        inRangeOfUnit = true;
                    }
                    else
                    {
                        reasonCantBuild = "Too far away from your units";
						AudioSource buildingsAudioSource = gameObject.GetComponent<AudioSource> ();
						audioManager.PlayOneShot(buildingsAudioSource,errorSoundClip);
                    }
                }

                if (costCoins > teamCoins)
                {
                    reasonCantBuild = "Not enough coins";
					AudioSource buildingsAudioSource = gameObject.GetComponent<AudioSource> ();
					audioManager.PlayOneShot(buildingsAudioSource,errorSoundClip);
                }

                if (costCoins <= teamCoins && inRangeOfUnit && !spaceOccupied)
                {
                    created = true;
                    buildingsCreated = 1;
                    //Transform nearestBuilding = 
                    Transform newBuilding = Instantiate(buildingPrefab, point, new Quaternion(0, yRotation, 0, 0));
                    newBuilding.eulerAngles = hoverModel.eulerAngles;

                    newBuilding.gameObject.AddComponent<Bucketable>();
                    newBuilding.gameObject.AddComponent<PolygonOptimizer>();
					AudioSource buildingsAudioSource = gameObject.GetComponent<AudioSource> ();
					audioManager.PlayOneShot(buildingsAudioSource,constructionSoundClip);

                    if (team == 1)
                        newBuilding.tag = "VR Player's Building";
                    else
                        if (team == 2)
                        newBuilding.tag = "PC Player's Building";
                }
            }

            if (primaryReleased)
            {
                //Create floating number
                //if (created) //costCoins * buildingsCreated <= teamCoins)
                //{
                    Vector3 floatingNumberPosition;
                    //startBuildPoint, placeObj.position
                    if (increments > 1)
                        floatingNumberPosition = new Vector3((startBuildPoint.x + point.x) / 2, 1, (startBuildPoint.z + point.z) / 2);
                    else
                        floatingNumberPosition = new Vector3(point.x, 1, point.z);

                    Transform floatingNumber = Instantiate(floatingNumberPrefab, floatingNumberPosition,
                         buildingCostIndicator.rotation);

                    floatingNumber.eulerAngles = new Vector3(0,
                        90 - Mathf.Rad2Deg * (Mathf.Atan2(buildingCostIndicator.position.z - cameraPosition.z, buildingCostIndicator.position.x - cameraPosition.x)), 0);
                    if (created)
                    {
                        floatingNumber.GetComponent<TextMesh>().text = "-" + costCoins * buildingsCreated + " Coins";
                        floatingNumber.GetComponent<TextMesh>().color = Color.green;
                        if (team == 1)
                            resourceController.addVRCoins(-costCoins * buildingsCreated);
                        if (team == 2)
                            resourceController.addPCCoins(-costCoins * buildingsCreated);
                    }
                    else
                    {
                        floatingNumber.GetComponent<TextMesh>().text = reasonCantBuild;
                        floatingNumber.GetComponent<TextMesh>().color = Color.red;
                    }
               // }
                
            }
        }
    }

    bool PositionEmpty(Vector3 position, string checkTagBuildings)
    {
        bool spaceOccupied = false;
        float distanceToNearestBuilding;
        Transform nearestBuilding = bucketGrid.getNearestObject(new Vector2(position.x, position.z), 6, checkTagBuildings);

        if (nearestBuilding != null)
        {
            distanceToNearestBuilding = Vector2.Distance(new Vector2(position.x, position.z), new Vector2(nearestBuilding.position.x, nearestBuilding.position.z));
            if (distanceToNearestBuilding < .45f)
            {
                spaceOccupied = true;
            }
        }
        return !spaceOccupied;
    }
}