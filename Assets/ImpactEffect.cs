using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ImpactEffect : MonoBehaviour {
	public AudioClip[] impactClips;
	public float volumeScale = 1f;
	public float minPitch;
	public float maxPitch;
	public float collisionEnterSpeed;
	public float collisionStaySpeed;
	public float maxSources;

	private List<AudioSource> sources = new List<AudioSource> ();

	void Update() {
		for (int i = sources.Count - 1; i >= 0; i--) {
			AudioSource source = sources [i];
			if (! source.isPlaying) {
				sources.Remove (source);
				GameObject.Destroy (source.gameObject);
			}
		}
	}

	void OnCollisionEnter(Collision hit) {
		if (hit.relativeVelocity.magnitude >= collisionEnterSpeed) {
			CreateSound(hit.relativeVelocity.magnitude);
		}
	}

	void OnCollisionsTAY(Collision hit) {
		if (hit.relativeVelocity.magnitude >= collisionStaySpeed) {
			CreateSound(hit.relativeVelocity.magnitude);
		}
	}

	void CreateSound(float magnitude) {
		if (sources.Count < 3) {
			float volume = magnitude / 20;
//			impactAudio.pitch = Random.Range (minPitch, maxPitch);
//			impactAudio.PlayOneShot (impactClips[Random.Range (0, impactClips.Length)], volume);

			GameObject newSource = new GameObject ();
			newSource.transform.position = this.transform.position;
			AudioSource source = newSource.AddComponent<AudioSource> ();
			source.pitch = Random.Range (minPitch, maxPitch);
			source.PlayOneShot (impactClips[Random.Range (0, impactClips.Length)], volume*volumeScale);

			sources.Add (source);
		}
	}
}
