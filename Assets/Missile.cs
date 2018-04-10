using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (Kill))]
public class Missile : MonoBehaviour {
	public int damage = 10;

	public LayerMask instantKillLayerMask;
	public float stickDepth = 1f;
	public float initialForce = 10f;
	public bool accelerate = false;
	public int acceleration = 10;
	public int accelerationTime = 10;
	private float startTime;

	public GameObject explosionPrefab;
	public GameObject explosion;
	public bool createExplosionParticles = true;
	public float explosionRadius = 1f;
	public float explosionForceScale = 1f;
	public ForceMode explosionForceMode = ForceMode.Impulse;

	private NetworkTransform networkTransform;
	private Rigidbody rigidBody;
	private Kill kill;

	private bool collided = false;

	void Start() {
		startTime = Time.time;
		rigidBody = GetComponent<Rigidbody> ();
		rigidBody.AddForce (transform.forward * initialForce);

		kill = GetComponent<Kill> ();
	}

	void Update() {
		if (accelerate) {
			if (Time.time - startTime < accelerationTime) {
				rigidBody.AddForce (transform.forward * acceleration);
			}
		}

//		if (!collided) {
//			transform.LookAt (transform.position + rigidBody.velocity);
//		}
	}

	void OnCollisionEnter(Collision collision) {
		//destroy if collided with fire
//		if (Util.InLayerMask (collision.gameObject.layer, instantKillLayerMask)) {
//			GetComponent<Kill> ().Die ();
//		}

//		collision.rigidbody.AddForce (collision.impulse, ForceMode.Impulse);
		if (collided) {
			return;
		}
		collided = true;
		GameObject hit = collision.gameObject;
		IHittable hittable = hit.GetComponent<IHittable> ();

		if (hittable != null) {
			Debug.Log ("hitting " + hittable);
			hittable.Hit (damage);
			//explode
			kill.Die();
		}

		Halt (collision);

//		Debug.Log ("calling explosion " + collision + " " + collision.contacts.Length);
//		CmdExplode (collision.contacts[0].point);
//		CmdExplode (transform.position, hit);
	}

	private void Halt(Collision collision) {
		Debug.Log ("halting");

		//adjust to be within collider a little bit more
		transform.position += transform.forward * stickDepth;

		//remove collider
		GetComponentInChildren<Collider> ().enabled = false;
		rigidBody.velocity = Vector2.zero;
		rigidBody.constraints = RigidbodyConstraints.FreezeAll;
		rigidBody.isKinematic = true;

		//parent to collision
//		Vector3 scale = transform.lossyScale;
//		Vector3 position = transform.position;
//		Quaternion rotation = transform.rotation;

//		transform.SetParent(collision.transform, false);

//		transform.sca = scale;
//		transform.position = position;
//		transform.rotation = rotation;

		//turn off this script
//		this.enabled = false;

		//remove the trail renderer
//		StartCoroutine (SlowTrailDisable (GetComponent<TrailRenderer> ()));
	}


	IEnumerator SlowTrailDisable (TrailRenderer trail) {
		float rate = trail.time / 15f;
		while (trail.time > 0) {
			trail.time -= rate;
			yield return 0;
		}
	}

//	[Command]
	private void CmdExplode(Vector3 collisionPoint, GameObject directHit) {
//		Debug.Log ("creating explosion");
		GameObject explosionInst = GameObject.Instantiate (explosionPrefab, collisionPoint, transform.rotation);
		Explosion explosionInstExplosion = explosionInst.GetComponent<Explosion> ();
		explosionInstExplosion.damage = damage;
		explosionInstExplosion.ignoreForDamage.Add (directHit);
		explosionInstExplosion.radius = explosionRadius;
		NetworkServer.Spawn (explosionInst);
//		Debug.Log ("spawned explosion");

		Destroy (gameObject);
	}
}
