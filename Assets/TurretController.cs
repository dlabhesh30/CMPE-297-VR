using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour {

    float shootTimer;

    public int team;

    BucketGridController bucketGridController;

    public Transform projectilePrefab;
    
    Transform nearestEnemy = null;

    public HealthBar healthBar;

    float range;

    GameSettings gameSettings;

	bool archerIsActive;

	GameObject archer;

    // Use this for initialization
    void Start () {
		archer = gameObject.transform.GetChild (7).gameObject;
		archer.gameObject.SetActive (false);
		archerIsActive = false;
        gameSettings = GameObject.FindGameObjectWithTag("GameSettings").GetComponent<GameSettings>();
        shootTimer = 0;
        bucketGridController = GameObject.FindGameObjectWithTag("BucketGridController").transform.GetComponent<BucketGridController>();
        healthBar = GetComponent<HealthBar>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!healthBar.underConstruction)
        {
			if (!archerIsActive) {
				archerIsActive = true;
				archer.gameObject.SetActive (true);
			}
			
            //if (shootTimer <= 0)
            //{
                if (tag == "VR Player's Building")
                    team = 1;
                if (tag == "PC Player's Building")
                    team = 2;
                if (tag == "AI Player's Building")
                    team = 3;

                //Shoot at nearest enemy

                List<string> tags = new List<string>();
                if (team != 3)
                    tags.Add("AI Player's Unit");
                if (team != 2)
                    tags.Add("PC Player's Unit");
                if (team != 1)
                    tags.Add("VR Player's Unit");

                if (nearestEnemy == null || Vector2.Distance(new Vector2(nearestEnemy.position.x, nearestEnemy.position.z), new Vector2(transform.position.x, transform.position.z)) > range)
                {
                    nearestEnemy = bucketGridController.getNearestObject(new Vector2(transform.position.x, transform.position.z), range, tags);
                }
                //getNearestObject(new Vector2(transform.position.x, transform.position.z), 10, tags); // "AI Player's Unit");

                if (nearestEnemy != null)
                {
                    //Debug.Log("Enemies found");
                    /*
					float xzdistance = Vector2.Distance(new Vector2(nearestEnemy.position.x, nearestEnemy.position.z), new Vector2(transform.position.x, transform.position.z));

                    float shootDirectionY = Mathf.Atan2(nearestEnemy.position.x - transform.position.x, nearestEnemy.position.z - transform.position.z) * Mathf.Rad2Deg - 90;
                    float shootDirectionZ = Mathf.Atan2(nearestEnemy.position.y - (transform.position.y + 1), xzdistance) * Mathf.Rad2Deg + 15;

                    float projectileSpeed = 8;

                    Transform newProjectile = Instantiate(projectilePrefab, transform.position + new Vector3(0, 1, 0), Quaternion.Euler(0, shootDirectionY, 90 + shootDirectionZ));
                    newProjectile.GetComponent<Rigidbody>().velocity = Quaternion.Euler(0, shootDirectionY, shootDirectionZ) * new Vector3(projectileSpeed, 0, 0);
                    newProjectile.GetComponent<ArrowController>().team = team;
                    Destroy(newProjectile.gameObject, 5);*/
                }
                else
                {
                    //Debug.Log("No enemies found");
                }
                //shootTimer = 2;
            //}
            //else
            //{
            //    shootTimer -= Time.deltaTime * gameSettings.gameSpeed;
            //}
        }
	}
}
