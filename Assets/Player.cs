using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour {
	public Image healthBar;
	private Health health;

	public IFirable[] firables;
	public int currentFirable = 0;
	public float minScrollToSwap = 0.1f;
	public float weaponSelectDelay;
	public float maxWeaponSelectDelay = 0.5f;

	void Start() {
		health = GetComponent<Health> ();

		foreach (IFirable firable in firables) {
			firable.DeSelect ();
		}
		firables [0].Select ();
	}

	void Update() {
		float weaponSelect = Input.GetAxis ("Weapon Select");
		if (weaponSelectDelay <= 0) {
			int nextFirable = currentFirable;
			if (weaponSelect > minScrollToSwap) {
				nextFirable++;
				if (nextFirable >= firables.Length) {
					nextFirable = 0;
				}
			} else if (weaponSelect < -minScrollToSwap) {
				nextFirable--;
				if (nextFirable < 0) {
					nextFirable = firables.Length - 1;
				}
			}

			if (nextFirable != currentFirable) {
				firables [currentFirable].DeSelect ();
				firables [nextFirable].Select ();
				currentFirable = nextFirable;

				weaponSelectDelay = maxWeaponSelectDelay;
			}
		} else {
			weaponSelectDelay -= Time.deltaTime;
		}

		if (Input.GetButton ("Fire")) {
			firables [currentFirable].Trigger ();
		}

		if (Input.GetButton ("Alt Fire")) {
			firables [currentFirable].AltTrigger ();
		}
	}
}
