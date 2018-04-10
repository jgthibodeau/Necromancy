using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
	public GameObject spreadableFire;
	public Kill killScript;

	float remainingLife;

	[Range(0,1)]
	public float spreadChance;
	public float spreadRadius;

	public float minPitch, maxPitch;

	void Start () {
		killScript = GetComponent<Kill> ();
		remainingLife = killScript.stopParticlesTimeInSeconds;
		StartCoroutine (Spread ());

		AudioSource audioSource = GetComponent<AudioSource> ();
		audioSource.pitch = Random.Range (minPitch, maxPitch);
		audioSource.time = Random.Range (0, audioSource.clip.length);
	}

	private IEnumerator Spread() {
		while (remainingLife > 0) {
			float waitTime = Random.Range (0.25f, 0.5f);
			remainingLife -= waitTime;
			yield return new WaitForSeconds (waitTime);
			float spread = Random.value;
			if (spread < spreadChance) {
				Debug.Log ("Fire spreading");
				Vector2 direction = Random.insideUnitCircle.normalized * spreadRadius;
				Vector3 newPosition = new Vector3 (transform.position.x + direction.x, transform.position.y, transform.position.z + direction.y);
				GameObject.Instantiate (spreadableFire, newPosition, Quaternion.identity);
			}
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Fire") {
			collision.gameObject.GetComponent<Kill> ().Die ();
		}
	}
}
