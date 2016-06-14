using UnityEngine;
using System.Collections;

public class DetectionConeScript : MonoBehaviour {
	public Transform player;
	private Transform enemy;
	private EnemyScript enemyScript;
	private Camera camera;
	public float viewDistance;
	public SpriteRenderer sprite;
	public float regularFadeSpeed = 3f;
	public float enemyOnScreenFadeSpeed = 5f;

	public bool enemyOnScreen = false;
	public bool enemyVisibleByPlayer = false;

	// Use this for initialization
	void Start () {
		enemy = this.transform.parent;
		enemyScript = enemy.GetComponent<EnemyScript> ();
		player = GameObject.Find ("Player").transform;
		camera = Camera.main;
		this.transform.parent = camera.transform;
	}

	void Update () {
		if (GlobalScript.currentGameState == GlobalScript.GameState.InGame)
			InGame ();
	}
	
	void InGame () {

		//turn off if entity is visible
		enemyOnScreen = false;
		enemyVisibleByPlayer = false;
		Vector3 enemyPoint = camera.WorldToViewportPoint(enemy.transform.position);
		if ((enemyPoint.x > 0 && enemyPoint.x < 1 && enemyPoint.y > 0 && enemyPoint.y < 1) && enemyPoint.z > 0) {
			enemyOnScreen = true;
		}

		if (!enemyOnScreen) {
			// Detect if target within viewDistance
			Vector3 direction = enemy.position - player.position;
			RaycastHit hit;
			if (Physics.Raycast (player.position, direction, out hit, viewDistance, GlobalScript.IgnoreInteractableLayerMask)) {
				if (hit.transform == enemy) {
					//set location to edge of screen
					Vector3 cameraTopRight = camera.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0));
					Vector3 cameraBottomLeft = camera.ScreenToWorldPoint (new Vector3 (0, 0, 0));
					Vector3 cameraCenter = camera.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, 0));

					bool fullRight = false;
					bool fullLeft = false;
					bool fullUp = false;
					bool fullDown = false;

					float x = enemy.position.x;
					if (x > cameraTopRight.x) {
						x = cameraTopRight.x;
						fullRight = true;
					} else if (x < cameraBottomLeft.x) {
						x = cameraBottomLeft.x;
						fullLeft = true;
					}
					float z = enemy.position.z;
					if (z > cameraTopRight.z) {
						z = cameraTopRight.z;
						fullUp = true;
					} else if (z < cameraBottomLeft.z) {
						z = cameraBottomLeft.z;
						fullDown = true;
					}

					Vector3 detectionPosition = new Vector3 (x, enemy.position.y, z);
					this.transform.position = detectionPosition;

					//set rotation appropriately
					Quaternion rotation = new Quaternion ();
					if (fullUp && fullRight) {
						rotation = Quaternion.Euler (90, 135, 0);
					} else if (fullUp && fullLeft) {
						rotation = Quaternion.Euler (90, 45, 0);
					} else if (fullUp) {
						rotation = Quaternion.Euler (90, 90, 0);
					} else if (fullDown && fullRight) {
						rotation = Quaternion.Euler (90, -135, 0);
					} else if (fullDown && fullLeft) {
						rotation = Quaternion.Euler (90, -45, 0);
					} else if (fullDown) {
						rotation = Quaternion.Euler (90, -90, 0);
					} else if (fullRight) {
						rotation = Quaternion.Euler (90, 180, 0);
					} else if (fullLeft) {
						rotation = Quaternion.Euler (90, 0, 0);
					}
					//				transform.rotation = Quaternion.Slerp(transform.position, rotation, Time.deltaTime*10f);
					transform.rotation = rotation;

					enemyVisibleByPlayer = true;
				}
			}
			Debug.DrawRay (player.position, direction, Color.blue);
		}

		//set color based on enemy state
		Color color = Color.white;
		switch (enemyScript.enemydata.currentState) {
		case(EnemyScript.State.Patrol):
			color = Color.white;
			break;
		case(EnemyScript.State.Investigate):
			color = Color.yellow;
			break;
		case(EnemyScript.State.Alert):
			color = Color.red;
			break;
		}

		//fade in and out
		if (enemyVisibleByPlayer)
			color.a = 1;
		else
			color.a = 0;
		float fadeSpeed = regularFadeSpeed;
		if (enemyOnScreen)
			fadeSpeed = enemyOnScreenFadeSpeed;
		sprite.material.SetColor ("_Color", Color.Lerp(sprite.material.color, color, Time.deltaTime * fadeSpeed));
	}
}
