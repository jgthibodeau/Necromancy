using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttackTrigger : MonoBehaviour {
	public bool isAttacking;

	public void setAttacking(int i) {
		isAttacking = (i == 1);
	}
}
