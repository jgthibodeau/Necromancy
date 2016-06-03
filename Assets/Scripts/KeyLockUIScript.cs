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
			if (!keylock.PreviousTumblersCorrect(keylock.currentTumblerOrderToPick)){
				randomAngle = Random.Range (-15*keylock.leftTrigger, 15*keylock.leftTrigger);
			}
			else if(!keylock.CorrectHeight(keylock.currentTumblerToPick))
				randomAngle = Random.Range (-2, 2);
			wrench.transform.rotation = Quaternion.Euler (0, 0, 225 - 90 * keylock.leftTrigger + randomAngle);

			//set lockpick and add randomness as appropriate
			//want to stop at 20
			lockpick.localPosition = new Vector3(75 + (keylock.lockpickX + 0.5f) * 16f, keylock.rightTrigger * 38 - 21, 0);
			Debug.Log(keylock.lockpickX+" "+keylock.rightTrigger+" "+lockpick.localPosition);

			randomAngle = 0f;
			if(keylock.wiggleLockpick)
				randomAngle = Random.Range (-2, 2);
			lockpick.rotation = Quaternion.Euler (0, 0, randomAngle);


			foreach(Transform r in rotatables){
				r.position = keylock.rotatable.position;
				r.rotation = keylock.rotatable.rotation;
			}
		}

		base.Update ();
	}
}

