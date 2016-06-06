using UnityEngine;
using System.Collections;

public class Sprite3DScript : MonoBehaviour {
	private Animator animator;

	void Start(){
		animator = GetComponent<Animator>();
	}

	void Update(){
		Vector3 targetPosition = Camera.main.transform.position;
		targetPosition.y = transform.position.y;
		transform.LookAt (targetPosition);

		float thisAngle = transform.parent.rotation.eulerAngles.y;
		float camAngle = Camera.main.transform.rotation.eulerAngles.y;

		float realAngle = thisAngle - camAngle;
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
				animator.SetInteger ("Direction", 3);
			else if (realAngle > 135)
				animator.SetInteger ("Direction", 0);
			else
				animator.SetInteger ("Direction", 1);

	//		animator.SetBool ("Idle", idle);
		}
	}
}