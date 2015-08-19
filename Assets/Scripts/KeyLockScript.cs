using UnityEngine;
using System.Collections;

public class KeyLockScript : InteractableScript {
	public enum State{Locked,Unlocked};
	public State currentState = State.Locked;

	public float[] tumblerHeights;
	public float tumblerHeightTolerance;
	public float tumblerTolerance;
	public float[] currentTumblerHeights;
	public int currentTumbler = 0;
	public float lockPickX;
	private float tumblerSize;
	public float changeTumblerSpeed = 0.5f;

	public float tension;
	public float tensionTolerance;
	public bool haveTension = false;

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

	void Start(){
		lockPickSprite = lockPick.GetComponent<SpriteRenderer> ().sprite;
		currentTumblerHeights = new float[tumblerHeights.Length];
		tumblerSize = 2.5f / tumblerHeights.Length;
		SetDefaults ();
	}

	void Update(){
		if (!Unlocked ()) {
			GetInput ();

			if (cancel) {
				Deactivate (true);
				return;
			}

			//Check tension is correct
			//		Vector3 leftStick = new Vector3 (inputLeftX, 0, inputLeftY);
			//		if (leftStick.magnitude > 0) {
			//			tensionAngle = Vector3.Angle (leftStick, Vector3.forward) * Mathf.Sign (leftStick.x);
			//			tensionWrench.transform.rotation = Quaternion.Euler (90, tensionAngle, 0);
			//		}
			haveTension = (leftTrigger >= tension - tensionTolerance && leftTrigger <= tension + tensionTolerance);
			int randomAngle = 0;
			if (haveTension)
				randomAngle = Random.Range (-2, 2);
			else {
				for (int i=0; i<currentTumblerHeights.Length; i++) {
					currentTumblerHeights [i] = 0;
				}
			}
			tensionWrench.transform.rotation = Quaternion.Euler (90, 135 + 90 * leftTrigger + randomAngle, 0);

			//Set current tumbler
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

			int oldTumbler = currentTumbler;
			currentTumbler = (int)((-0.5f - lockPick.localPosition.x) / tumblerSize);
			if (currentTumbler >= tumblerHeights.Length)
				currentTumbler = tumblerHeights.Length - 1;
			// Play tumbler click if on a new tumbler
			if (oldTumbler != currentTumbler)
				audioSource.PlayOneShot (hitTumblerClip, 1f);

			//Set current tumbler height
			currentTumblerHeights [currentTumbler] = rightTrigger;
			lockPick.transform.position = new Vector3 (lockPick.transform.position.x, lockPick.transform.position.y, this.transform.position.z - 1.05f + 1.9f * rightTrigger);
			if (rightTrigger >= tumblerHeights [currentTumbler] - tumblerHeightTolerance && rightTrigger <= tumblerHeights [currentTumbler] + tumblerHeightTolerance) {
				int angle = Random.Range (-2, 2);
				lockPick.rotation = Quaternion.Euler (90, angle, 0);
				if(!pickingAudioSource.isPlaying)
					pickingAudioSource.Play ();
			} else{
				if(pickingAudioSource.isPlaying)
					pickingAudioSource.Stop ();
				lockPick.rotation = Quaternion.Euler (90, 0, 0);
			}
			//		GamePad.SetVibration(0,testA,testB);

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
				Deactivate(false);
		}
	}

	bool Unlocked(){
		for(int i=0;i<tumblerHeights.Length;i++){
			if(currentTumblerHeights[i] < tumblerHeights[i] - tumblerHeightTolerance || currentTumblerHeights[i] > tumblerHeights [i] + tumblerHeightTolerance)
				return false;
		}
		return true;
	}
	
	void FixedUpdate(){
		UpdateLocation ();
	}

	void SetDefaults(){
		currentTumbler = 0;
		haveTension = false;
		tensionWrench.transform.rotation = Quaternion.Euler (90, 135, 0);
		for (int i=0; i<currentTumblerHeights.Length; i++) {
			currentTumblerHeights[i] = 0;
		}
	}

	//Disable the lock, set all variables to default, return control to player
	void Deactivate(bool reset){
		audioSource.Stop ();
		if(reset)
			SetDefaults ();
		GameObject.Find ("Player").GetComponent<PlayerScript>().ChangeState(PlayerScript.State.Moving);
		this.gameObject.SetActiveRecursively(false);
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
