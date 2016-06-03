using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

[System.Serializable]
public class PlayerData : SaveData{
	public PlayerData () : base () {}
	public PlayerData (SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
}

public class PlayerScript : SavableScript {
	public PlayerData playerdata;
	
	// Input variables
	private Vector2 moveInput;
	private Vector2 lookInput;
	private bool interact;
	private bool cancel;
	private bool jump;

	public float speed = 10;

	float direction;
	float currentSpeed;

	public Vector3 movement;
//	public Vector3 look;

	public enum State{Moving, Interacting};
	public State currentState = State.Moving;

	// Use this for initialization
	void Start () {
		savedata = playerdata;
	}
	
	void Update () {
		if (GlobalScript.currentGameState == GlobalScript.GameState.InGame)
			InGame ();
	}

	void InGame () {
		playerdata = (PlayerData)savedata;

		//TODO ensure globalscript isnt paused for all scripts

		GetInput ();

		// Only do movement updates if in movement playerstate
		switch (currentState) {
		case State.Moving:
			Vector3 forward = Camera.main.transform.TransformDirection (Vector3.forward);
			forward.y = 0;
			forward = forward.normalized;
			Vector3 right = new Vector3 (forward.z, 0, -forward.x);
			movement = (moveInput.x * right + moveInput.y * forward).normalized;
			movement *= speed;


//			movement = new Vector3 (speed * moveInput.x, 0, speed * moveInput.y);
//			movement = Vector3.ClampMagnitude (movement, speed);

//			look = new Vector3 (lookInput.x, 0, lookInput.y);
			
			if (interact) {
				InteractorScript interactor = GetComponent<InteractorScript> ();
				if (interactor != null) {
					interactor.Interact ();
				}
			}
			break;
		case State.Interacting:
			break;
		}

		currentSpeed = new Vector2 (moveInput.x, moveInput.y).sqrMagnitude;
		StickToWorldSpace (this.transform, Camera.main.transform, ref direction, ref currentSpeed);
	}

	void GetInput(){
		interact = GlobalScript.GetButton (GlobalScript.Interact);
		cancel = GlobalScript.GetButton (GlobalScript.Cancel);
		jump = GlobalScript.GetButton (GlobalScript.Jump);

		moveInput = GlobalScript.GetAxis (GlobalScript.LeftStick);
		lookInput = GlobalScript.GetAxis (GlobalScript.RightStick);
	}
	
	void FixedUpdate()
	{
		// 5 - Move the game object
		GetComponent<Rigidbody>().velocity = movement;

		Vector3 actualDirectionOfMotion = movement.normalized;

		float angle;
//		if (look.magnitude > 0) {
//			angle = Vector3.Angle (look, Vector3.forward) * Mathf.Sign (look.x);
//			this.transform.rotation = Quaternion.Euler (0, angle, 0);
		/*} else */if (actualDirectionOfMotion.magnitude > 0) {
			angle = Vector3.Angle (actualDirectionOfMotion, Vector3.forward) * Mathf.Sign (actualDirectionOfMotion.x);
			this.transform.rotation = Quaternion.Euler (0, angle, 0);
		}

	}

	void OnDestroy()
	{
		// Game Over.
		// Add the script to the parent because the current game
		// object is likely going to be destroyed immediately.
		// transform.parent.gameObject.AddComponent<GameOverScript>();
	}

	public void ChangeState(State state){
		currentState = state;
		switch(currentState){
		case State.Moving:
			break;
		case State.Interacting:
			break;
		}
	}

	public void StickToWorldSpace(Transform root, Transform camera, ref float directionOut, ref float speedOut){
		Vector3 rootDirection = root.forward;

		Vector3 stickDirection = new Vector3(moveInput.x, 0, moveInput.y);

		speedOut = stickDirection.sqrMagnitude;

		//get camera rotation
		Vector3 CameraDirection = camera.forward;
		CameraDirection.y = 0.0f;
		Quaternion referenceShift = Quaternion.FromToRotation (Vector3.forward, CameraDirection);

		//convert joystick to worldspace coords
		Vector3 moveDirection = referenceShift * stickDirection;
		Vector3 axisSign = Vector3.Cross (moveDirection, rootDirection);

		Debug.DrawRay (new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
//		Debug.DrawRay (new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
//		Debug.DrawRay (new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);

		float angleRootToMove = Vector3.Angle (rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);

		angleRootToMove /= 180f;

		directionOut = angleRootToMove * speed;
	}
}
