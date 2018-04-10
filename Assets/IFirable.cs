using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class IFirable : MonoBehaviour {
	public GameObject graphics;
	public bool isActive;

	public bool fireJustTriggered;
	public bool fireTriggered;
	public bool fireReleased;
	public bool firing;

	public bool altFireJustTriggered;
	public bool altFireTriggered;
	public bool altFireReleased;
	public bool altFiring;

	public bool canFireBothSimultaneously = false;
	public bool primaryTakesPrecedence = true;

	public float fireManaCost;
	public float altFireManaCost;

	void LateUpdate() {
		Fire (fireJustTriggered, firing, fireReleased);
		if (fireJustTriggered) {
			fireJustTriggered = false;
		} else if (fireTriggered) {
			fireTriggered = false;
		} else if (firing) {
			if (CanTrigger ()) {
				fireReleased = true;
			}
			firing = false;
		} else if (fireReleased) {
			fireReleased = false;
		}

		AltFire (altFireJustTriggered, altFiring, altFireReleased);
		if (altFireJustTriggered) {
			altFireJustTriggered = false;
		} else if (altFireTriggered) {
			altFireTriggered = false;
		} else if (altFiring) {
			if (CanAltTrigger ()) {
				altFireReleased = true;
			}
			altFiring = false;
		} else if (altFireReleased) {
			altFireReleased = false;
		}
	}

	private bool CanTrigger() {
		return isActive && (canFireBothSimultaneously || primaryTakesPrecedence || (!primaryTakesPrecedence && !altFiring));
	}

	private bool CanAltTrigger() {
		return isActive && (canFireBothSimultaneously || !primaryTakesPrecedence || (primaryTakesPrecedence && !firing));
	}

	public void Trigger() {
		if (CanTrigger ()) {
			if (!firing) {
				fireJustTriggered = true;
			}
			fireTriggered = true;
			firing = true;
		} else {
			fireJustTriggered = false;
			fireTriggered = false;
			fireReleased = false;
			firing = false;
		}
	}

	public void AltTrigger() {
		if (CanAltTrigger ()) {
			if (!altFiring) {
				altFireJustTriggered = true;
			}
			altFireTriggered = true;
			altFiring = true;
		} else {
			altFireJustTriggered = false;
			altFireTriggered = false;
			altFireReleased = false;
			altFiring = false;
		}
	}

	public abstract void Fire(bool fireTriggered, bool firing, bool fireReleased);
	public abstract void AltFire(bool altFireTriggered, bool altFiring, bool altFireReleased);

	public void Select() {
		graphics.SetActive (true);
		isActive = true;
	}

	public void DeSelect() {
		graphics.SetActive (false);
		isActive = false;
	}
}
