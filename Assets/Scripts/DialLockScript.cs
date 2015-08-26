using UnityEngine;
using System.Collections;

public class DialLockScript : MonoBehaviour {
	public enum State{Locked,Unlocked};
	public State currentState = State.Locked;

	public float[] tumblerAngles;
	public float[] currentTumblerAngles;
	public bool[] correctTumblers;
	public int currentTumbler;
	public float currentAngle;
	private float prevAngle;
	public float tumblerAngleTolerance;
	public Vector2 listenPoint;
	private int resetAngle;
	private bool hasBeenReset;
	private enum DIRECTION{
		CLOCKWISE,
		COUNTERCLOCKWISE
	}
	private DIRECTION prevDirection = DIRECTION.CLOCKWISE;
	private DIRECTION currentDirection = DIRECTION.CLOCKWISE;

	public float rotateSpeed = 0.5f;
	public float unlockSpeed;
	
	public Transform dial;
	public Transform handle;
	public GameObject lockedObject;
	
	// Inputs
	private bool unlock;
	private bool cancel;
	private Vector2 prevInputLeft;
	private Vector2 inputLeft;
	private Vector2 prevInputRight;
	private Vector2 inputRight;
	private float leftTrigger;
	private float rightTrigger;
	
	// Sounds
	public AudioClip newTumblerClip;
	public AudioClip correctAngleClip;
	public AudioClip unlockedClip;
	public AudioSource audioSource;
	public AudioSource rotatingAudioSource;
	
	void Start(){
		currentTumblerAngles = new float[tumblerAngles.Length];
		correctTumblers = new bool[tumblerAngles.Length];

		//calculate angles from numbers
		for (int i=0; i<tumblerAngles.Length; i++) {
			float angle = (360f*(30f-tumblerAngles[i]))/30f;
			//if i is even, turning right and this angle must be greater than the last
			if(i%2==0){
				//if angle greater than last
				if(i == 0 || angle > tumblerAngles[i-1])
					tumblerAngles[i] = angle;
				//else, add 360
				else
					tumblerAngles[i] = angle + 360;
			}
			//if i is odd, turning left and this angle must be less than the last
			else{
				//if angle less than last
				if(i == 0 || angle < tumblerAngles[i-1])
					tumblerAngles[i] = angle;
				//else, subtract 360
				else
					tumblerAngles[i] = angle - 360;
			}
		}

		resetAngle = 360 * tumblerAngles.Length;
		currentTumbler = 0;
	}
	
	void Update () {
		if (GlobalScript.currentGameState == GlobalScript.GameState.InGame)
			InGame ();
	}
	
	void InGame () {
		switch(currentState){
		case State.Locked:
			GetInput ();
			
			if (cancel) {
				Deactivate ();
				return;
			}

			// TODO update listening point


			// update current angle
			prevAngle = currentAngle;
			prevDirection = currentDirection;
			Vector3 prevInputVector = new Vector3(prevInputLeft.x, 0, prevInputLeft.y);
			Vector3 inputVector = new Vector3(inputLeft.x, 0, inputLeft.y);
			if(prevInputVector.magnitude > .9f && inputVector.magnitude > .9f && !prevInputVector.Equals (inputVector)){
				float angle = Vector3.Angle (prevInputVector, inputVector);
				float sign = Mathf.Sign(Vector3.Cross(prevInputVector, inputVector).y);
				currentAngle += angle*sign*rotateSpeed;

				if(sign > 0)
					currentDirection = DIRECTION.CLOCKWISE;
				else
					currentDirection = DIRECTION.COUNTERCLOCKWISE;

				if(!rotatingAudioSource.isPlaying)
					rotatingAudioSource.Play ();
			}
			else if(rotatingAudioSource.isPlaying)
				rotatingAudioSource.Stop();

			dial.transform.rotation = Quaternion.Euler(0,currentAngle,0);

			//To get number of tumblers, lower pitch click every 360 degrees turned until picked up all tumblers
			if((int)prevAngle / 360 < (int)currentAngle / 360 && !hasBeenReset){
				//TODO play sound
				print ("click");
			}

			//if rotated right enough times
			if(currentAngle >= resetAngle){
				for(int i=0;i<currentTumblerAngles.Length;i++)
					currentTumblerAngles[i]=0;
				currentTumbler = 0;
				hasBeenReset = true;
			}
			if(hasBeenReset)
				currentAngle = currentAngle % 360;

			//if direction changed, increment tumbler
			if(prevDirection != currentDirection){
				hasBeenReset = false;
				if(currentTumbler < tumblerAngles.Length - 1)
					currentTumbler ++;
				else{
					for(int i=0;i<currentTumblerAngles.Length;i++)
						currentTumblerAngles[i]=0;
				}
			}

			//set current tumbler correctly
			currentTumblerAngles[currentTumbler] = currentAngle;

			//Play sound if tumbler set correctly
			bool oldTumbler = correctTumblers[currentTumbler];
			if(CheckTumbler(currentTumbler) && !oldTumbler)
				audioSource.PlayOneShot(correctAngleClip, 1f);

			//start unlock animation
			if (Unlocked () && rightTrigger > 0) {
				currentState = State.Unlocked;
				lockedObject.GetComponent<LockedScript>().SetLocked(false);
				handle.Rotate (new Vector3(0,0,-unlockSpeed*Time.deltaTime));
				rotatingAudioSource.Stop ();
				audioSource.PlayOneShot (unlockedClip, 1f);
			}
			break;
		case State.Unlocked:
			handle.Rotate (new Vector3(0,0,-unlockSpeed*Time.deltaTime));
			if(handle.rotation.eulerAngles.y >= 45)
				Deactivate();
			break;
		}
	}

	bool CheckTumbler(int i){
		if (currentTumblerAngles [i] >= tumblerAngles [i] - tumblerAngleTolerance && currentTumblerAngles [i] <= tumblerAngles [i] + tumblerAngleTolerance)
			correctTumblers [i] = true;
		else
			correctTumblers [i] = false;
		return correctTumblers [i];
	}
	
	bool Unlocked(){
		for(int i=0;i<correctTumblers.Length;i++){
			if(!correctTumblers[i])
				return false;
		}
		return true;
	}
	
	void FixedUpdate(){
		UpdateLocation ();
	}
	
	//Disable the lock, set all variables to default, return control to player
	void Deactivate(){
		rotatingAudioSource.Stop ();
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
