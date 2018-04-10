using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Syphon : MonoBehaviour {
	Health health;
	public float drainRate;
	public float healRate;
	public float minHealthToHeal;

	public BLINDED_AM_ME.Path_Comp bloodPath;
	private ParticleSystem bloodParticles;
	public Transform syphonStart;
	public float range = 25f;

	Health target;

	// Use this for initialization
	void Start () {
		health = GetComponent<Health> ();
		bloodParticles = bloodPath.GetComponent<ParticleSystem> ();
		bloodParticles.Stop ();
	}
	
	// Update is called once per frame
	void Update () {
		//if button pressed, aquire a target
		if (Input.GetButtonDown ("Syphon")) {
			GetTarget ();
		}

		//if button held, do syphon
		if (Input.GetButton ("Syphon")) {
			if (CheckTarget ()) {
				HandleSyphon ();
				if (!bloodParticles.isPlaying) {
					bloodParticles.Play ();
//					Debug.Log ("Blood particles on");
				}
			} else {
				target = null;
			}
		} else {
			target = null;
		}

		if (target == null) {
			if (bloodParticles.isPlaying) {
				bloodParticles.Stop ();
//				Debug.Log ("Blood particles off");
			}
		}
	}

	void GetTarget() {
		target = null;

		RaycastHit hit;

		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range)) {
			Health h = hit.transform.GetComponent<Health> ();
			if (h != null) {
				target = h;
			}
		}
	}

	bool CheckTarget() {
		return (target != null) && (Vector3.Distance(transform.position, target.transform.position) <= range);
	}

	void HandleSyphon() {
		bool doneSyphoning = false;
		switch (target.state) {
		case Health.State.Alive:
			bloodPath.children = new Transform[] { target.transform, syphonStart };

			//drain health from enemy to player
			float drainAmount = drainRate * Time.deltaTime;
			drainAmount = target.Hit (drainAmount);
			health.Heal (drainAmount);

//			Debug.Log ("Syphon drain " + drainAmount);
			if (target.state == Health.State.Dead || drainAmount == 0f) {
//				Debug.Log ("Done draining");
				doneSyphoning = true;
			}
			break;
		case Health.State.Dead:
		case Health.State.Reanimated:
			bloodPath.children = new Transform[] { syphonStart, target.transform };

			//reanimate corpse
			//or
			//drain health from player to zombie
			float healAmount = healRate * Time.deltaTime;
			if (health.currentHealth - healAmount < minHealthToHeal) {
				healAmount = health.currentHealth - minHealthToHeal;
			}
			healAmount = target.Heal (healAmount);
			health.Hit (healAmount);

//			Debug.Log ("Syphon heal " + healAmount);
			if (healAmount == 0f) {
//				Debug.Log ("Done healing");
				doneSyphoning = true;
			}
			break;
		}

		if (doneSyphoning) {
			target = null;
		}
	}
}
