using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DetectableSound : MonoBehaviour {
//	public float speed;
//	public AudioClip clip;
//	public float volume = 0.01f;
//	public float minDistance = 10f;
//	public float maxDistance = 30f;
//	public float frequency = 0;
//	public float timeSinceLast = 0;

	// Update is called once per frame
	void Update () {
//		if (frequency > 0) {
//			timeSinceLast += Time.deltaTime;
//
//			if (timeSinceLast > 1/frequency) {
//				CreateWave ();
//				timeSinceLast = 0;
//			}
//		} else
//			timeSinceLast = 0;
	}

	public void Play(AudioClip clip, float volume, float minDistance, float maxDistance, float speed){
		GameObject wave =  new GameObject ();
		wave.name = "Sound Wave";
		wave.transform.position = transform.position;
		wave.layer = LayerMask.NameToLayer("Ignore Raycast");

		SphereCollider collider = wave.AddComponent<SphereCollider> ();
		collider.isTrigger = true;

		if (clip != null) {
			AudioSource audioSource = wave.AddComponent <AudioSource> ();
			audioSource.clip = clip;
			audioSource.volume = volume;
			audioSource.spatialBlend = 1;
			audioSource.dopplerLevel = 5;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.minDistance = minDistance;
			audioSource.maxDistance = maxDistance;

			audioSource.Play ();
		}

		SoundWave waveScript = wave.AddComponent<SoundWave> ();
		waveScript.maxMagnitude = maxDistance;
		waveScript.speed = speed;
		waveScript.volume = volume;
	}

//	public void CreateWave(){
//		GameObject wave =  new GameObject ();
//		wave.name = "Sound Wave";
//		wave.transform.position = transform.position;
//		wave.layer = LayerMask.NameToLayer("Ignore Raycast");
//
//		SphereCollider collider = wave.AddComponent<SphereCollider> ();
//		collider.isTrigger = true;
//
//		if (clip != null) {
//			AudioSource audioSource = wave.AddComponent <AudioSource> ();
//			audioSource.clip = clip;
//			audioSource.volume = volume;
//			audioSource.spatialBlend = 1;
//			audioSource.dopplerLevel = 5;
//			audioSource.rolloffMode = AudioRolloffMode.Linear;
//			audioSource.minDistance = minDistance;
//			audioSource.maxDistance = maxDistance;
//
//			audioSource.Play ();
//		}
//
//		SoundWave waveScript = wave.AddComponent<SoundWave> ();
//		waveScript.maxMagnitude = maxDistance;
//		waveScript.speed = speed;
//		waveScript.volume = volume;
//	}
}
