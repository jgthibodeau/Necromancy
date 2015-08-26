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

	public Transform lockPick;
	private Sprite lockPickSprite;
	public Transform tensionWrench;
	public Transform rotatable;

	public GameObject lockedObject;

	public float unlockSpeed;

	// Inputs
	private bool unlock;
	private bool cancel;
	private Vector2 inputLeft;
	private Vector2 inputRight;
	private Vector2 prevInputLeft;
	private Vector2 prevInputRight;
	private float leftTrigger;
	private float rightTrigger;

	// Sounds
	public AudioClip hitTumblerClip;
	public AudioClip releasedTumblerClip;
	public AudioClip unlockedClip;
	public AudioSource audioSource;
	public AudioSource pickingAudioSource;

	public override void Start(){
		base.Start ();

		lockPickSprite = lockPick.GetComponent<SpriteRenderer> ().sprite;

		currentHeights = new float[keylockdata.correctHeights.Length];
		keylockdata.tumblerOrder = new int[keylockdata.correctHeights.Length];
		for (int i=0; i<keylockdata.correctHeights.Length; i++) {
			keylockdata.correctHeights [i] = Random.Range (.15f, 1f);
			keylockdata.tumblerOrder[i] = i;
		}
		GlobalScript.ShuffleArray<int> (keylockdata.tumblerOrder);
		tumblerSize = 2.5f / keylockdata.correctHeights.Length;
		tensionStep = 1f / (keylockdata.correctHeights.Length+1);
		SetDefaults ();

		savedata = (SaveData)keylockdata;
	}

	public override void ToggledUpdate () {
		if (GlobalScript.currentGameState == GlobalScript.GameState.InGame)
			InGame ();
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
			currentTumblerOrderToPick = (int)(leftTrigger / tensionStep) - 1;
			if(currentTumblerOrderToPick > keylockdata.tumblerOrder.Length - 1)
				currentTumblerOrderToPick = keylockdata.tumblerOrder.Length - 1;
			if(currentTumblerOrderToPick == -1)
				currentTumblerToPick = -1;
			else
				currentTumblerToPick = keylockdata.tumblerOrder[currentTumblerOrderToPick];
//			if(oldTumbler != currentTumblerToPick)
//				audioSource.PlayOneShot (hitTumblerClip, 1f);

			float randomAngle = 0f;
			if (!PreviousTumblersCorrect(currentTumblerOrderToPick)){
				randomAngle = Random.Range (-15*leftTrigger, 15*leftTrigger);
				print (randomAngle);
			}
			else if(!CorrectHeight(currentTumblerToPick))
				randomAngle = Random.Range (-2, 2);
			tensionWrench.transform.rotation = Quaternion.Euler (90, 135 + 90 * leftTrigger + randomAngle, 0);

			//Set all tumblers above currentTumblerToPick to 0 height
			for (int i=currentTumblerOrderToPick+1; i<keylockdata.tumblerOrder.Length; i++)
				currentHeights [keylockdata.tumblerOrder[i]] = 0;

			//Set current tumbler
			oldTumbler = (int)((-0.5f - lockPick.localPosition.x) / tumblerSize);
			if (oldTumbler >= currentHeights.Length)
				oldTumbler = currentHeights.Length - 1;

			float offset = 0f;
			if (inputRight.x < 0f && lockPick.transform.localPosition.x > -3f) {
				offset = inputRight.x * changeTumblerSpeed * Time.deltaTime;
				if (lockPick.transform.localPosition.x + offset < -3f)
					offset = -3f - lockPick.transform.localPosition.x;
			}
			if (inputRight.x > 0f && lockPick.transform.localPosition.x < -0.5f) {
				offset = inputRight.x * changeTumblerSpeed * Time.deltaTime;
				if (lockPick.transform.localPosition.x + offset > -0.5f)
					offset = -0.5f - lockPick.transform.localPosition.x;
			}
			if (offset != 0f)
				lockPick.transform.position = lockPick.transform.position + new Vector3 (offset, 0, 0);
			currentTumblerBeingPicked = (int)((-0.5f - lockPick.localPosition.x) / tumblerSize);

			if (currentTumblerBeingPicked >= currentHeights.Length)
				currentTumblerBeingPicked = currentHeights.Length - 1;
			// Play tumbler click if on a new tumbler
			if (oldTumbler != currentTumblerBeingPicked)
				audioSource.PlayOneShot (hitTumblerClip, 1f);

			//if currentTumblerBeingPicked is after currentTumblerToPick
			int pickingOrder = System.Array.IndexOf(keylockdata.tumblerOrder, currentTumblerBeingPicked);
			if (pickingOrder > currentTumblerOrderToPick) {
				currentHeights [currentTumblerBeingPicked] = rightTrigger;
				//no wiggle
				lockPick.rotation = Quaternion.Euler (90, 0, 0);
				//no sound
				if(pickingAudioSource.isPlaying)
					pickingAudioSource.Stop ();
			//if currentTumblerBeingPicked is before currentTumblerToPick
			} else if (pickingOrder < currentTumblerOrderToPick) {
				if (rightTrigger > currentHeights [currentTumblerBeingPicked])
					rightTrigger = currentHeights [currentTumblerBeingPicked];
				//no wiggle
				lockPick.rotation = Quaternion.Euler (90, 0, 0);
				//no sound
				if(pickingAudioSource.isPlaying)
					pickingAudioSource.Stop ();
			//if currentTumblerBeingPicked is currentTumblerToPick
			} else {
//				if (rightTrigger > currentHeights[currentTumblerBeingPicked]){
//					if(!pickingAudioSource.isPlaying)
//						pickingAudioSource.Play ();
//				} else {
//					if(pickingAudioSource.isPlaying)
//						pickingAudioSource.Stop ();
//				}

				bool wasCorrect = CorrectHeight(currentTumblerBeingPicked);
				if (rightTrigger > currentHeights[currentTumblerBeingPicked]){
					currentHeights[currentTumblerBeingPicked] = rightTrigger;

					//if all previous tumblers are correct
					if(PreviousTumblersCorrect(currentTumblerOrderToPick)){
						//wiggle and noise as adjust tumbler
						float angle = Random.Range (-2, 2);
						lockPick.rotation = Quaternion.Euler (90, angle, 0);
							
						//if just got to correct height
						if (CorrectHeight(currentTumblerBeingPicked) && !wasCorrect){
							audioSource.PlayOneShot (hitTumblerClip, 1f);
						}
					} else {
						lockPick.rotation = Quaternion.Euler (90, 0, 0);
					}
				} else {
					lockPick.rotation = Quaternion.Euler (90, 0, 0);
				}
			}

			lockPick.transform.position = new Vector3 (lockPick.transform.position.x, lockPick.transform.position.y, this.transform.position.z - 1.05f + 1.9f * rightTrigger);

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

	bool CorrectHeight(int tumbler){
		if (tumbler < 0)
			return true;
		if (tumbler >= keylockdata.correctHeights.Length)
			return false;
		return (currentHeights [tumbler] >= (keylockdata.correctHeights [tumbler] - heightTolerance) && currentHeights [tumbler] <= (keylockdata.correctHeights [tumbler] + heightTolerance));
	}

	bool PreviousTumblersCorrect(int thisTumblerOrder){
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
	
	void FixedUpdate(){
		UpdateLocation ();
	}

	void SetDefaults(){
		tensionWrench.transform.rotation = Quaternion.Euler (90, 135, 0);
		for (int i=0; i<currentHeights.Length; i++) {
			currentHeights[i] = 0;
		}
	}

	//Disable the lock, set all variables to default, return control to player
	void Exit(bool reset){
		audioSource.Stop ();
		if(reset)
			SetDefaults ();
		GameObject.Find ("Player").GetComponent<PlayerScript>().ChangeState(PlayerScript.State.Moving);

		Deactivate();
	}

	void GetInput(){
		unlock = GlobalScript.GetButton (GlobalScript.Interact);
		cancel = GlobalScript.GetButton (GlobalScript.Cancel);
		prevInputLeft = inputLeft;
		inputLeft = GlobalScript.GetAxis(GlobalScript.LeftStick);
		prevInputRight = inputRight;
		inputRight = GlobalScript.GetAxis(GlobalScript.RightStick);
		leftTrigger = GlobalScript.GetTrigger (GlobalScript.LeftTrigger);
		rightTrigger = GlobalScript.GetTrigger (GlobalScript.RightTrigger);
	}

	//Ensure the lock is always centered on the screen
	void UpdateLocation(){
		float oldY = this.transform.position.y;
		this.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
		this.transform.position = new Vector3 (this.transform.position.x, oldY, this.transform.position.z);
	}
}
