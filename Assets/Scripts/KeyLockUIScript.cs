using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class KeyLockUIScript : MenuScript{
	private KeyLockScript keylock;
	public RectTransform lockpick;
	public RectTransform wrench;
	public RectTransform[] rotatables;

	public void SetLock(KeyLockScript kls){
		keylock = kls;
	}

	void Update(){
		//move items appropriately
		if (Visible ()) {
			//set wrench and add randomness as appropriate
			float randomAngle = 0f;
			/*if (!keylock.PreviousTumblersCorrect(keylock.currentTumblerOrderToPick)){
				randomAngle = Random.Range (-10*keylock.pickHeightInput, 10*keylock.pickHeightInput);
			}
			else */if(!keylock.CorrectHeight(keylock.currentTumblerToPick))
				randomAngle = Random.Range (-2, 2);
			wrench.transform.rotation = Quaternion.Euler (0, 0, 225 - 45 * keylock.pickHeightInput + randomAngle);

			//set lockpick and add randomness as appropriate
			//want to stop at 20
			lockpick.localPosition = new Vector3(-50 + (keylock.lockpickX + 0.5f) * 16f, keylock.wrenchInput * 38 - 21, 0);
			Debug.Log(keylock.lockpickX+" "+keylock.wrenchInput+" "+lockpick.localPosition);

			randomAngle = 0f;
			if(keylock.wiggleLockpick)
				randomAngle = Random.Range (-2, 2);
			lockpick.rotation = Quaternion.Euler (0, 180, randomAngle);


			foreach(Transform r in rotatables){
				r.position = keylock.rotatable.position;
				r.rotation = keylock.rotatable.rotation;
			}
		}

		base.Update ();
	}
}

