using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour {
	public float lifeTimeInSeconds;
	public float startTime;

	private ParticleSystem ps;
	public float stopParticlesTimeInSeconds;

	private AudioSource audioSource;
	public float volumeDropoffRate = 1f;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		audioSource = GetComponent<AudioSource> ();
		startTime = Time.time;
		MyGameManager.instance.AddInstace (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (ps != null && Time.time >= startTime + stopParticlesTimeInSeconds) {
			if (ps.isPlaying) {
				ps.Stop ();
			}
			if (audioSource != null && audioSource.volume > 0) {
				audioSource.volume -= Time.deltaTime * volumeDropoffRate;
			}
		}

		if (Time.time >= startTime + lifeTimeInSeconds) {
			Die ();
		}
	}

	public void Die () {
		MyGameManager.instance.RemoveInstance (this.gameObject);
	}
}
