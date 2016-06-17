using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class KeyLock3DUIScript : UI3DScript{
	private KeyLockScript keylock;
	public Transform parent;
	public Transform lockpick;
	public Transform wrench;
	public RectTransform[] rotatables;

	private Vector3 initialPosition;
	private Vector3 initialRotation;

	public void SetLock(KeyLockScript kls){
		keylock = kls;

		initialPosition = keylock.transform.position;
		initialRotation = keylock.transform.rotation.eulerAngles;

		parent.transform.position = keylock.transform.position;
		parent.transform.rotation = keylock.transform.rotation;

		wrench.transform.rotation = Quaternion.Euler (0, 0, 315 + 45 * keylock.pickHeightInput);

		lockpick.localPosition = new Vector3(0, 0.05f * keylock.wrenchInput, -0.075f * keylock.lockpickX);
	}

	protected override void Update(){
		//move items appropriately
		if (Visible ()) {
			parent.transform.position = keylock.transform.position;
			parent.transform.rotation = keylock.transform.rotation;

			//set wrench and add randomness as appropriate
			float randomAngle = 0f;
			/*if (!keylock.PreviousTumblersCorrect(keylock.currentTumblerOrderToPick)){
				randomAngle = Random.Range (-10*keylock.pickHeightInput, 10*keylock.pickHeightInput);
			}
			else */if(!keylock.CorrectHeight(keylock.currentTumblerToPick))
				randomAngle = Random.Range (-2, 2);
			wrench.transform.rotation = Quaternion.Euler (0, 0, 315 + 45 * keylock.pickHeightInput + randomAngle);

			//set lockpick and add randomness as appropriate
			//want to stop at z -.075, y .05
			lockpick.localPosition = new Vector3(0, 0.05f * keylock.wrenchInput, -0.075f * keylock.lockpickX);

			randomAngle = 0f;
			if(keylock.wiggleLockpick)
				randomAngle = Random.Range (-2, 2);
			lockpick.rotation = Quaternion.Euler (randomAngle, 0, 0);


//			foreach(Transform r in rotatables){
//				r.position = keylock.rotatable.position;
//				r.rotation = keylock.rotatable.rotation;
//			}
		}

		base.Update ();
	}
}

