using UnityEngine;
using System.Collections;

public class Sprite2DScript : MonoBehaviour {
	private Animator animator;

	void Start(){
		animator = GetComponent<Animator>();

		this.transform.rotation = Quaternion.Euler(90, 0, 0);
	}

	void Update(){
		this.transform.rotation = Quaternion.Euler(90, 0, 0);

		float realAngle = transform.parent.rotation.eulerAngles.y;

		if (realAngle < 0)
			realAngle += 360;

		// Up = 2
		// Down = 0
		// Left = 1
		// Right = 3

		// >315 and <45 = away
		// <315 and >225 = left
		// <225 and >135 = towards
		// <135 and >45 = right

		if(animator != null){
			bool idle = false;
			if (realAngle > 315 || realAngle < 45)
				animator.SetInteger ("Direction", 2);
			else if (realAngle > 225)
				animator.SetInteger ("Direction", 1);
			else if (realAngle > 135)
				animator.SetInteger ("Direction", 0);
			else
				animator.SetInteger ("Direction", 3);

			//		animator.SetBool ("Idle", idle);
		}
	}
}