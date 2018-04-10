using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSyphon : IFirable {
	Health health;
	public float drainRate;
	public float healRate;
	public float minHealthToHeal;

	public BLINDED_AM_ME.Path_Comp drainBloodPath;
	private ParticleSystem drainBloodParticles;

	public BLINDED_AM_ME.Path_Comp healBloodPath;
	private ParticleSystem healBloodParticles;

	public Transform drainStart;
	public Transform healStart;
	public float range = 25f;

	public Health drainTarget;
	public Health healTarget;

	public LayerMask playerLayer;

	// Use this for initialization
	void Start () {
		health = GetComponentInParent<Health> ();

		drainBloodParticles = drainBloodPath.GetComponent<ParticleSystem> ();
		drainBloodParticles.Stop ();

		healBloodParticles = healBloodPath.GetComponent<ParticleSystem> ();
		healBloodParticles.Stop ();
	}

	void Update() {
		if (drainTarget != null) {
			if (!drainBloodParticles.isPlaying) {
				drainBloodParticles.Play ();
			}
		} else {
			if (drainBloodParticles.isPlaying) {
				drainBloodParticles.Stop ();
			}
		}

		if (healTarget != null) {
			if (!healBloodParticles.isPlaying) {
				healBloodParticles.Play ();
			}
		} else {
			if (healBloodParticles.isPlaying) {
				healBloodParticles.Stop ();
			}
		}
	}

	public override void Fire(bool fireTriggered, bool firing, bool fireReleased) {
		if (fireTriggered) {
			drainTarget = GetTarget ();
		}

		if (firing) {
			if (CheckTarget (drainTarget)) {
				Drain ();
			} else {
				drainTarget = null;
			}
		} else {
			drainTarget = null;
		}
	}

	public override void AltFire(bool altFireTriggered, bool altFiring, bool altFireReleased) {
		if (altFireTriggered) {
			healTarget = GetTarget ();
		}

		if (altFiring) {
			if (CheckTarget (healTarget)) {
				Heal ();
			} else {
				healTarget = null;
			}
		} else {
			healTarget = null;
		}
	}

	Health GetTarget() {
		Health target = null;

		RaycastHit hit;

		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, ~playerLayer)) {
			Health h = hit.transform.GetComponentInParent<Health> ();
			if (h != null) {
				target = h;
			}
		}

		return target;
	}

	bool CheckTarget(Health target) {
		if (target != null) {
			Debug.Log (Vector3.Distance(transform.position, target.transform.position) <= range);
//			Debug.Break ();
			return (Vector3.Distance (transform.position, target.transform.position) <= range);
		}
		return false;
	}

	void Drain() {
		bool doneSyphoning = false;

		drainBloodPath.children = new Transform[] { drainTarget.transform, drainStart };

		//drain health from enemy to player
		float drainAmount = drainRate * Time.deltaTime;
		drainAmount = drainTarget.Hit (drainAmount);
		health.Heal (drainAmount);

		if (drainTarget.state == Health.State.Dead || drainAmount == 0f) {
			doneSyphoning = true;
		}

		if (doneSyphoning) {
			drainTarget = null;
		}
	}

	void Heal() {
		bool doneSyphoning = false;

		healBloodPath.children = new Transform[] { healStart, healTarget.transform };

		//reanimate corpse
		//or
		//drain health from player to zombie
		float healAmount = healRate * Time.deltaTime;
		if (health.currentHealth - healAmount < minHealthToHeal) {
			healAmount = health.currentHealth - minHealthToHeal;
		}
		healAmount = healTarget.Heal (healAmount);
		health.Hit (healAmount);

		if (healAmount == 0f) {
			doneSyphoning = true;
		}

		if (doneSyphoning) {
			healTarget = null;
		}
	}
}
