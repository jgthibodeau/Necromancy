using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	public Health health;
	public Image image;
		
	// Update is called once per frame
	void Update () {
		image.fillAmount = health.currentHealth / health.maxHealth;
	}
}
