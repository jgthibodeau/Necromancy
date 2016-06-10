using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundData{
	public Vector3 origin;
	public float volume;
	public int priority; //TODO get this from somewhere
}

public class SoundDetector : MonoBehaviour {
	public List<SoundData> sounds = new List<SoundData> ();
	public LayerMask Mask;
	public float dampingFactor = 0.5f;
	public float minVolume = 0.001f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ReceiveSound(SoundWave wave){
		Vector3 direction = wave.transform.position - transform.position;
		Debug.DrawRay (transform.position, direction, Color.gray);

		float volume = wave.CurrentVolume;
		foreach (RaycastHit hit in Physics.RaycastAll (transform.position, direction, direction.magnitude, Mask)) {
			volume *= dampingFactor;
		}

		if (volume > minVolume) {
			SoundData sd = new SoundData ();
			sd.origin = wave.transform.position;
			sd.volume = volume;
			sounds.Add (sd);
		}
	}

	public void RemoveSound(SoundData sd){
		sounds.Remove (sd);
	}
}
