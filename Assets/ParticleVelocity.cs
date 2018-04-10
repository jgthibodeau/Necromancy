using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleVelocity : MonoBehaviour {
	public GameObject spawnOnCollision;

	private ParticleSystem ps;
	private ParticleSystem.MainModule mm;
	private ParticleSystem.MinMaxCurve particleSpeed;
	private float originalSpeed;
	private Rigidbody rb;

	List<ParticleCollisionEvent> collisionEvents;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		mm = ps.main;
		particleSpeed = mm.startSpeed;
		originalSpeed = particleSpeed.constant;

		rb = GetComponentInParent<Rigidbody> ();

		collisionEvents = new List<ParticleCollisionEvent> ();
	}

	void Update () {
		float forwardVelocity = Vector3.Dot (rb.velocity, transform.forward);
		particleSpeed.constant = originalSpeed + forwardVelocity;
		mm.startSpeed = particleSpeed;
		//		ps.main = mm;

		//		Debug.Log (ps+" particle speed: " + particleSpeed.constant);
	}

	void OnParticleCollision(GameObject other) {


//		int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents (ps, other, collisionEvents);
////		Debug.Log (numCollisionEvents);
//
//		int i = 0;
//		while (i < numCollisionEvents) {
//		//			if (i % 20 == 0) {
//			ParticleCollisionEvent pevent = collisionEvents [i];
////		ParticleCollisionEvent pevent = collisionEvents [Random.Range(0, numCollisionEvents-1)];
//			GameObject.Instantiate (spawnOnCollision, pevent.intersection, Quaternion.identity);
//			//			}
//			i++;
//		}

	}
}
