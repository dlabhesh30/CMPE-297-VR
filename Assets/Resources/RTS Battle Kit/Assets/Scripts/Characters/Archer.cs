//This should be the 7th child of Turret
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Archer : MonoBehaviour {
	
	//variables visible in the inspector
	//public GameObject arrow;
	public Transform arrowSpawner;
	//public GameObject animationArrow;
	public float damage;
	public float minAttackDistance;
	public int team;
	public Transform projectilePrefab;

	BucketGridController bucketGridController;
	float range;
	GameSettings gameSettings;

	//not visible in the inspector
	private bool shooting;
	private bool addArrowForce;
	private GameObject newArrow;
	private float shootingForce;
	private Animator animator;
	private Transform nearestEnemy= null;
	private TurretController parentTurretController;
	bool isActive;

	void Start(){
		isActive = false;
		parentTurretController = transform.parent.gameObject.GetComponent<TurretController>();
		animator = transform.GetChild(1).GetComponent<Animator>();
		Debug.Log(animator.name);
		gameSettings = GameObject.FindGameObjectWithTag("GameSettings").GetComponent<GameSettings>();
		bucketGridController = GameObject.FindGameObjectWithTag("BucketGridController").transform.GetComponent<BucketGridController>();
	}
	
	void Update(){

		//Shoot at nearest enemy
		List<string> tags = new List<string>();
		if (parentTurretController.team != 3) {
			tags.Add ("AI Player's Unit");
		}
		if (parentTurretController.team != 2) {
			tags.Add ("PC Player's Unit");
		}
		if (parentTurretController.team != 1) {
			tags.Add ("VR Player's Unit");
		}

		if (nearestEnemy == null || Vector2.Distance(new Vector2(nearestEnemy.position.x, nearestEnemy.position.z), new Vector2(transform.position.x, transform.position.z)) > range)
		{
			nearestEnemy = bucketGridController.getNearestObject(new Vector2(transform.position.x, transform.position.z), range, tags);
			Debug.Log("No enemies found");
			animator.SetBool ("Attacking", false);
		}
		//getNearestObject(new Vector2(transform.position.x, transform.position.z), 10, tags); // "AI Player's Unit");

		//If there's a currentTarget and its within the attack range, move agent to currenttarget
		if (nearestEnemy != null && Vector3.Distance (transform.position, nearestEnemy.transform.position) < minAttackDistance) {
			var currentTargetPosition = nearestEnemy.position;
			currentTargetPosition.y = transform.position.y;
			transform.LookAt (currentTargetPosition);	
			animator.SetBool ("Attacking", true);
			Debug.Log("Enemies found");
			//nearestEnemy.gameObject.GetComponent<Character>().lives -= Time.deltaTime * damage;
		}

		//only shoot when animation is almost done (when the character is shooting)
		if(animator.GetBool("Attacking") == true && animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.95f && !shooting){
			StartCoroutine(shoot());
		}
		/*
		//set an extra arrow active to make illusion of shooting more realistic
		if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.25f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.95f){
			animationArrow.SetActive(true);
		}
		else{
			animationArrow.SetActive(false);
		}*/
	}
	/*
	void LateUpdate(){
		//check if the archer shoots an arrow
		if(addArrowForce && this.gameObject != null && nearestEnemy != null && newArrow != null && arrowSpawner != null){
			
			//create a shootingforce
			shootingForce = Vector3.Distance(transform.position, nearestEnemy.transform.position);
			//add shooting force to the arrow
			newArrow.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(new Vector3(0, shootingForce * 12 + 
				((nearestEnemy.transform.position.y - transform.position.y) * 45), shootingForce * 55)));
			
			addArrowForce = false;
		}
	}*/
	
	IEnumerator shoot(){
		//archer is currently shooting
		shooting = true;

		/*
		//add a new arrow
		newArrow = Instantiate(arrow, arrowSpawner.position, arrowSpawner.rotation) as GameObject;
		newArrow.GetComponent<Arrow>().arrowOwner = this.gameObject;
		//shoot it using rigidbody addforce
		addArrowForce = true;
		*/

		// shoot arrow
		float xzdistance = Vector2.Distance(new Vector2(nearestEnemy.position.x, nearestEnemy.position.z), new Vector2(transform.position.x, transform.position.z));

		float shootDirectionY = Mathf.Atan2(nearestEnemy.position.x - transform.position.x, nearestEnemy.position.z - transform.position.z) * Mathf.Rad2Deg - 90;
		float shootDirectionZ = Mathf.Atan2(nearestEnemy.position.y - (transform.position.y + 1), xzdistance) * Mathf.Rad2Deg + 15;

		float projectileSpeed = 8;

		Transform newProjectile = Instantiate(projectilePrefab, arrowSpawner.position, arrowSpawner.rotation);
		newProjectile.GetComponent<Rigidbody>().velocity = Quaternion.Euler(0, shootDirectionY, shootDirectionZ) * new Vector3(projectileSpeed, 0, 0);
		newProjectile.GetComponent<ArrowController>().team = team;
		Destroy(newProjectile.gameObject, 5);
	
		//wait and set shooting back to false
		yield return new WaitForSeconds(0.5f);
		shooting = false;
	}
}
