using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : IFirable {

	public bool circleOn = false;
	public bool explodeCircleOn = false;

	public bool circleTriggered = false;
	public bool explodeCircleTriggered = false;

	public GameObject circle;
	public GameObject circleGraphics;
	public GameObject explodeCircleGraphics;
	public SphereCollider collider;

	public float resurrectHealth = 50f;

	public GameObject explosion;

	public LayerMask groundLayer;
	
	// Update is called once per frame
	void Update () {
		if (circleOn || explodeCircleOn) {
			StartCircle ();
		} else {
			StopCircle ();
		}
	}

	public override void Fire(bool fireTriggered, bool firing, bool fireReleased) {
		circleOn = firing;
		circleTriggered = fireReleased;
	}

	public override void AltFire(bool altFireTriggered, bool altFiring, bool altFireReleased) {
		explodeCircleOn = altFiring;
		explodeCircleTriggered = altFireReleased;
	}

	public void StartCircle () {
		circleGraphics.SetActive(circleOn);
		explodeCircleGraphics.SetActive(explodeCircleOn);

		//place circle graphics in the world
		Vector3 circlePosition = GetTargetPosition();
		if (circlePosition != Vector3.zero) {
			circle.transform.position = circlePosition;
		}
//		circleGraphics.transform.LookAt (Camera.main.transform, Vector3.up);
//		Quaternion lookRotation = Quaternion.LookRotation (Camera.main.transform.forward, Vector3.up);
//		circleGraphics.transform.rotation = lookRotation;
		Vector3 lookRotation = Vector3.zero;
		lookRotation.y = Camera.main.transform.eulerAngles.y;
		circle.transform.eulerAngles = lookRotation;
	}

	public void StopCircle () {
		circleGraphics.SetActive(false);
		explodeCircleGraphics.SetActive(false);

		//if active, trigger attack
		if (circleTriggered) {
			//resurrect corpses within the trigger
			Vector3 center = collider.transform.position + collider.center;
			float radius = collider.radius;

			Collider[] allOverlappingColliders = Physics.OverlapSphere(center, radius);

			List<Health> healths = new List<Health> ();

			foreach (Collider c in allOverlappingColliders) {
				Health h = c.GetComponentInParent<Health> ();
				if (h != null && h.IsDead () && !healths.Contains (h)) {
					healths.Add (h);
					h.Heal (resurrectHealth);
				}
			}
		}
		if (explodeCircleTriggered) {
			//explode corpses within the trigger
			Vector3 center = collider.transform.position + collider.center;
			float radius = collider.radius;

			Collider[] allOverlappingColliders = Physics.OverlapSphere(center, radius);

			List<Health> healths = new List<Health> ();

			foreach (Collider c in allOverlappingColliders) {
				Health h = c.GetComponentInParent<Health> ();
				if (h != null && h.IsDead () && !healths.Contains (h)) {
					healths.Add (h);

					Vector3 position = h.transform.position;
					position.y = center.y + 0.065f;
					GameObject.Instantiate (explosion, position, explosion.transform.rotation);
				}
			}

			foreach (Health h in healths) {
				GameObject.Destroy(h.transform.root.gameObject);
			}
		}
	}

	Vector3 GetTargetPosition() {
		Vector3 aimSpot = Vector3.zero;

		float screenX = Screen.width / 2;
		float screenY = Screen.height / 2;
		Ray ray = Camera.main.ScreenPointToRay(new Vector2(screenX, screenY));
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			aimSpot = hit.point;

			aimSpot.y += 0.1f;
			if (Physics.Raycast (aimSpot, Vector3.down, out hit, Mathf.Infinity, groundLayer)) {
				aimSpot = hit.point;
			}
		}

		return aimSpot;
	}
}
