using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamage : MonoBehaviour {
	public float damage = 10f;

	void OnParticleCollision(GameObject other) {
		IHittable hittable = other.GetComponent<IHittable> ();

		if (hittable != null) {
			Debug.Log ("hitting " + hittable);
			hittable.Hit (damage);
		}
	}
}
