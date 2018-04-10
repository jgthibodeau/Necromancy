using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : IFirable {
	public GameObject bullet;
	public Transform bulletSpawn;
	public float fireTime;
	private float fireCounter = 0;
	public Vector3 fireRandomness = new Vector3(1f, 1f, 1f);

	public AudioSource audioSource;
	public AudioClip[] clips;
	public float minPitch, maxPitch;

	private Rigidbody rigidBody;

	void Start(){
		rigidBody = GetComponentInParent<Rigidbody> ();
	}

	// Update is called once per frame
	void Update () {
		if (fireCounter > 0) {
			fireCounter -= Time.deltaTime;
		}
	}

	public override void Fire(bool fireTriggered, bool firing, bool fireReleased) {
		if (!firing) {
			return;
		}

		if (fireCounter > 0) {
			return;
		}

		fireCounter = fireTime;
		Vector3 bulletPosition = bulletSpawn.position;
		bulletPosition += bulletSpawn.right * Random.Range (-1f, 1f) * fireRandomness.x;
		bulletPosition += bulletSpawn.up * Random.Range (-1f, 1f) * fireRandomness.y;
		bulletPosition += bulletSpawn.forward * Random.Range (0f, 1f) * fireRandomness.z;

//		Quaternion bulletRotation = bulletSpawn.rotation;
//		GameObject bulletInst = GameObject.Instantiate (bullet, bulletPosition, bulletRotation);
		Vector3 targetPosition = GetTargetPosition ();
		Vector3 randomUp = Random.Range(-1f, 1f) * bulletSpawn.right + Random.Range(-1f, 1f) * bulletSpawn.up;
		GameObject bulletInst = GameObject.Instantiate (bullet, bulletPosition, Quaternion.LookRotation(targetPosition - bulletPosition, randomUp));

		bulletInst.GetComponent<Rigidbody>().velocity = rigidBody.velocity;

		if (clips.Length > 0) {
			audioSource.pitch = Random.Range (minPitch, maxPitch);
			audioSource.PlayOneShot (clips [Random.Range (0, clips.Length - 1)]);
		}
	}

	public override void AltFire(bool altFireTriggered, bool altFiring, bool altFireReleased) {
	}

	Vector3 GetTargetPosition() {
		Vector3 aimSpot;

		float screenX = Screen.width / 2;
		float screenY = Screen.height / 2;
		Ray ray = Camera.main.ScreenPointToRay(new Vector2(screenX, screenY));
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			aimSpot = hit.point;
		} else {
			aimSpot = Camera.main.transform.position + Camera.main.transform.forward * 50;
		}

		return aimSpot;
	}
}
