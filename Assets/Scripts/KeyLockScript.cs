using UnityEngine;
using System.Collections;using System.Runtime.Serialization;

[System.Serializable]
public class KeyLockData : SaveData{
	public KeyLockScript.State currentState = KeyLockScript.State.Locked;
	public float[] correctHeights;
	public int[] tumblerOrder;	//what order this tumbler is
	
	public KeyLockData () : base () {}
	public KeyLockData (SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
}

public class KeyLockScript : ToggleableScript {
	public KeyLockData keylockdata;

	public enum State{Locked,Unlocked};
	
	public float[] currentHeights;
	public float heightTolerance;
	public int currentTumblerOrderToPick;
	public int currentTumblerToPick;
	public int currentTumblerBeingPicked;
	public float tensionStep;
	public float tumblerSize;
	public float changeTumblerSpeed;

	public float lockpickX;
	public bool wiggleLockpick = false;
	public Transform rotatable;

	public GameObject lockedObject;

	public float unlockSpeed;

	// Inputs
	private bool unlock;
	private bool cancel;
	private float pickInput;
	private float prevPickInput;
	public float pickHeightInput;
	public float wrenchInput;

	// Sounds
	public AudioClip hitTumblerClip;
	public AudioClip releasedTumblerClip;
	public AudioClip unlockedClip;
	public AudioSource audioSource;
	public AudioSource pickingAudioSource;

	public override void Start(){
		base.Start ();

		lockpickX = 0;

		rotatable = new GameObject().transform;

		currentHeights = new float[keylockdata.correctHeights.Length];
		keylockdata.tumblerOrder = new int[keylockdata.correctHeights.Length];
		for (int i=0; i<keylockdata.correctHeights.Length; i++) {
			keylockdata.correctHeights [i] = Random.Range (.15f, 1f);
			keylockdata.tumblerOrder[i] = i;
		}
		GlobalScript.ShuffleArray<int> (keylockdata.tumblerOrder);
		tumblerSize = 1f / keylockdata.correctHeights.Length;
		tensionStep = 1f / (keylockdata.correctHeights.Length+1);
		SetDefaults ();

		savedata = keylockdata;
	}

	public override void ToggledUpdate () {
		if (GlobalScript.currentGameState == GlobalScript.GameState.InGame)
			InGame ();
	}

	public override void Activate(){
//		GameObject.Find ("KeyLockUI").GetComponent<KeyLockUIScript>().SetLock (this);
//		GameObject.Find ("KeyLockUI").GetComponent<KeyLockUIScript>().Open ();
		GameObject.Find ("KeyLock3DUI").GetComponent<KeyLock3DUIScript>().SetLock (this);
		GameObject.Find ("KeyLock3DUI").GetComponent<KeyLock3DUIScript>().Open ();
		base.Activate ();
	}

	public override void Deactivate(){
		GameObject.Find ("KeyLock3DUI").GetComponent<KeyLock3DUIScript>().Close ();
		base.Deactivate ();
	}
	
	void InGame () {
		keylockdata = (KeyLockData)savedata;

		if (!Unlocked ()) {
			GetInput ();

			if (cancel) {
				Exit (true);
				return;
			}

			//Check tension is correct
			int oldTumbler = currentTumblerToPick;
			currentTumblerOrderToPick = (int)(pickHeightInput / tensionStep) - 1;
			if(currentTumblerOrderToPick > keylockdata.tumblerOrder.Length - 1)
				currentTumblerOrderToPick = keylockdata.tumblerOrder.Length - 1;
			if(currentTumblerOrderToPick == -1)
				currentTumblerToPick = -1;
			else
				currentTumblerToPick = keylockdata.tumblerOrder[currentTumblerOrderToPick];
//			if(oldTumbler != currentTumblerToPick)
//				audioSource.PlayOneShot (hitTumblerClip, 1f);

			//Set all tumblers above currentTumblerToPick to 0 height
			for (int i=currentTumblerOrderToPick+1; i<keylockdata.tumblerOrder.Length; i++)
				currentHeights [keylockdata.tumblerOrder[i]] = 0;

			//Set current tumbler
			oldTumbler = (int)(lockpickX / tumblerSize);
			if (oldTumbler >= currentHeights.Length)
				oldTumbler = currentHeights.Length - 1;

			float offset = 0f;
			if (pickInput < 0f && lockpickX > 0f) {
				offset = pickInput * changeTumblerSpeed * Time.deltaTime;
				if (lockpickX + offset < 0)
					offset = 0f - lockpickX;
			}
			if (pickInput > 0f && lockpickX < 1f) {
				offset = pickInput * changeTumblerSpeed * Time.deltaTime;
				if (lockpickX + offset > 1f)
					offset = 1f - lockpickX;
			}
			if (offset != 0f)
				lockpickX += offset;
			currentTumblerBeingPicked = (int)(lockpickX / tumblerSize);
			Debug.Log (lockpickX + " " + tumblerSize + " " + currentTumblerBeingPicked);

			if (currentTumblerBeingPicked >= currentHeights.Length)
				currentTumblerBeingPicked = currentHeights.Length - 1;
			// Play tumbler click if on a new tumbler
			if (oldTumbler != currentTumblerBeingPicked)
				audioSource.PlayOneShot (hitTumblerClip, 1f);

			//if currentTumblerBeingPicked is after currentTumblerToPick
			int pickingOrder = System.Array.IndexOf(keylockdata.tumblerOrder, currentTumblerBeingPicked);
			if (pickingOrder > currentTumblerOrderToPick) {
				currentHeights [currentTumblerBeingPicked] = wrenchInput;
				//no wiggle
				wiggleLockpick = false;
				//no sound
				if(pickingAudioSource.isPlaying)
					pickingAudioSource.Stop ();
			//if currentTumblerBeingPicked is before currentTumblerToPick
			} else if (pickingOrder < currentTumblerOrderToPick) {
				if (wrenchInput > currentHeights [currentTumblerBeingPicked])
					wrenchInput = currentHeights [currentTumblerBeingPicked];
				//no wiggle
				wiggleLockpick = false;

				//no sound
				if(pickingAudioSource.isPlaying)
					pickingAudioSource.Stop ();
			//if currentTumblerBeingPicked is currentTumblerToPick
			} else {
//				if (wrenchInput > currentHeights[currentTumblerBeingPicked]){
//					if(!pickingAudioSource.isPlaying)
//						pickingAudioSource.Play ();
//				} else {
//					if(pickingAudioSource.isPlaying)
//						pickingAudioSource.Stop ();
//				}

				bool wasCorrect = CorrectHeight(currentTumblerBeingPicked);
				if (wrenchInput > currentHeights[currentTumblerBeingPicked]){
					currentHeights[currentTumblerBeingPicked] = wrenchInput;

					//if all previous tumblers are correct
					if(PreviousTumblersCorrect(currentTumblerOrderToPick)){
						//wiggle and noise as adjust tumbler
						wiggleLockpick = true;
							
						//if just got to correct height
						if (CorrectHeight(currentTumblerBeingPicked) && !wasCorrect){
							audioSource.PlayOneShot (hitTumblerClip, 1f);
						}
					} else {
						wiggleLockpick = false;
					}
				} else {
					wiggleLockpick = false;
				}
			}

			//start unlock animation
			if (Unlocked ()) {
				lockedObject.GetComponent<LockedScript>().SetLocked(false);
				rotatable.Rotate (new Vector3(0,unlockSpeed*Time.deltaTime,0));
				audioSource.Stop ();
				audioSource.PlayOneShot (unlockedClip, 1f);
			}
		} else {
			rotatable.Rotate (new Vector3(0,unlockSpeed*Time.deltaTime,0));
			if(rotatable.eulerAngles.y >= 45)
				Exit(false);
		}
	}

	bool Unlocked(){
		for(int i=0;i<currentHeights.Length;i++){
			if(!CorrectHeight(i))
				return false;
		}
		return true;
	}

	public bool CorrectHeight(int tumbler){
		if (tumbler < 0)
			return true;
		if (tumbler >= keylockdata.correctHeights.Length)
			return false;
		return (currentHeights [tumbler] >= (keylockdata.correctHeights [tumbler] - heightTolerance) && currentHeights [tumbler] <= (keylockdata.correctHeights [tumbler] + heightTolerance));
	}

	public bool PreviousTumblersCorrect(int thisTumblerOrder){
		if (thisTumblerOrder < 0)
			return true;

		if (thisTumblerOrder >= keylockdata.tumblerOrder.Length)
			thisTumblerOrder = keylockdata.tumblerOrder.Length-1;
		
		print (thisTumblerOrder);
		for (int i=thisTumblerOrder-1; i>=0; i--){
			print (i+" "+CorrectHeight(keylockdata.tumblerOrder[i]));
			if(!CorrectHeight(keylockdata.tumblerOrder[i]))
			   return false;
		}
		return true;
	}

	void SetDefaults(){
		for (int i=0; i<currentHeights.Length; i++) {
			currentHeights[i] = 0;
		}
	}

	//Disable the lock, set all variables to default, return control to player
	void Exit(bool reset){
		audioSource.Stop ();
		if(reset)
			SetDefaults ();
		GameObject.Find ("Player").GetComponent<PlayerController>().ChangeState(PlayerController.State.Moving);

		Deactivate();
	}

	void GetInput(){
		unlock = GlobalScript.GetButton (GlobalScript.Interact);
		cancel = GlobalScript.GetButton (GlobalScript.Cancel);
		prevPickInput = pickInput;
		pickInput = GlobalScript.GetStick(GlobalScript.LeftStick).y;
		wrenchInput = GlobalScript.GetTrigger (GlobalScript.LeftTrigger);
		pickHeightInput = GlobalScript.GetTrigger (GlobalScript.RightTrigger);
	}
}
