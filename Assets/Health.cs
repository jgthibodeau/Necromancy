using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IHittable {
	public float maxHealth = 100;
	public float currentHealth;
	public float minHealthToReanimate;

	public enum State {
		Alive, Dead, Reanimated
	}
	public State state;

//	private Respawnable respawnable;

	void Start() {
//		respawnable = GetComponent<Respawnable> ();
	}

	public float Hit(float damage) {
		if (currentHealth < damage) {
			damage = currentHealth;
		}

		currentHealth -= damage;
		if (currentHealth <= 0) {
			//shouldnt need this sanity check
			currentHealth = 0;
			state = State.Dead;
		}

		return damage;
	}

	public float Heal(float amount) {
		if (currentHealth + amount > maxHealth) {
			amount = maxHealth - currentHealth;
		}

		currentHealth += amount;
		//shouldnt need this sanity check
		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		}

		if (currentHealth > minHealthToReanimate && state == State.Dead) {
			state = State.Reanimated;
		}

		return amount;
	}

	public bool IsDead() {
//		return currentHealth <= 0;
		return state == State.Dead;
	}

	public void Reset() {
		currentHealth = maxHealth;
	}
}
