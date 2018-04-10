using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceThrower : IFirable {
	public bool flameOn;

	public ParticleSystem particles;
	public enum State
	{
		Starting, Playing, Stopping, Stopped
	}
	public State state = State.Stopped;
	public AudioSource audioSource;
	public AudioClip startClip;
	public AudioClip continueClip;
	public AudioClip endClip;

	public float rampUpSpeed = 2;
	public float rampDownSpeed = 2f;

	private ParticleSystem.EmissionModule em;
	private ParticleSystem.MinMaxCurve rateOverTime;
	public float originalRate;
	public float rateMultiplier;

	void Start() {
//		particles = GetComponent<ParticleSystem> ();
		em = particles.emission;
		rateOverTime = em.rateOverTime;
		originalRate = rateOverTime.constant;

		rateMultiplier = 0;

//		audioSource.loop = true;
//		audioSource.clip = continueClip;
//		audioSource.Play ();
//
//		audioSource.volume = rateMultiplier;
	}

	void Update() {
		if (flameOn) {
			StartFlame ();
		} else {
			StopFlame ();
		}
		rateOverTime.constant = originalRate * rateMultiplier;
		em.rateOverTime = rateOverTime;
//		audioSource.volume = rateMultiplier;
	}

	public override void Fire(bool fireTriggered, bool firing, bool fireReleased) {
		flameOn = firing;
	}

	public override void AltFire(bool altFireTriggered, bool altFiring, bool altFireReleased) {
	}

	public void StartFlame () {
		if (rateMultiplier == 0) {
			particles.Play ();
//			audioSource.time = 0;
		}
		rateMultiplier = Mathf.Clamp01 (rateMultiplier + Time.deltaTime * rampUpSpeed);
	}

	public void StopFlame () {
		rateMultiplier = Mathf.Clamp01 (rateMultiplier - Time.deltaTime * rampDownSpeed);

		if (rateMultiplier == 0) {
			if (particles.isPlaying) {
				particles.Stop ();
			}
		}
	}
}
