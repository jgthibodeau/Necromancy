using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class DialLockUIScript : MenuScript{
	private DialLockScript diallock;
	public RectTransform dial;
	public RectTransform handle;
	public RectTransform[] rotatables;

	public void SetLock(DialLockScript dls){
		diallock = dls;
	}

	void Update(){
		//move items appropriately
		if (Visible ()) {
			//set dial angle
			dial.rotation = Quaternion.Euler(0,0,diallock.currentAngle);
		}

		base.Update ();
	}
}

