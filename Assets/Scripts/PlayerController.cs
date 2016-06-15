using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using UnityStandardAssets.Characters.FirstPerson;

[System.Serializable]
public class PlayerData : SaveData{
	public PlayerData () : base () {}
	public PlayerData (SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
}

public class PlayerController : SavableScript {
	public PlayerData playerdata;

	// Input variables
	private bool interact;
	private bool cancel;

	public enum State{Moving, Interacting};
	public State currentState = State.Moving;

	private Transform camera;

	private FpsController fpsController;

	public AudioClip[] footStepClips;
	public AudioClip jumpClip;
	public AudioClip landClip;
	public AudioClip crouchClip;
	public AudioClip unCrouchClip;
	public AudioClip[] distractionClips;

	private DetectableSound detectableSounds;
	public float footStepVolume = 0.1f;
	public float footStepRate = 4f;
	public float footStepDistance = 30f;
	public float lastStep;

	// Use this for initialization
	void Start () {
		savedata = playerdata;
//		camera = GetComponentInChildren <Camera> ().transform;
		camera = Camera.main.transform;
		fpsController = GetComponent<FpsController>();
		detectableSounds = GetComponent<DetectableSound> ();
	}

	void Update () {
		if (GlobalScript.currentGameState == GlobalScript.GameState.InGame)
			InGame ();
		else
			fpsController.doMovement = false;
	}

	void InGame () {
		playerdata = (PlayerData)savedata;

		GetInput ();

		// Only do movement updates if in movement playerstate
		switch (currentState) {
		case State.Moving:
			fpsController.doMovement = true;
			if (interact) {
				InteractorScript interactor = GetComponent<InteractorScript> ();
				if (interactor != null) {
					interactor.Interact (camera);
				}
			}
			break;
		case State.Interacting:
			fpsController.doMovement = false;
			break;
		}

		//if grounded and moving, make step sounds and send out sound waves of appropriate strength and rate
		float speed = fpsController.Velocity.magnitude;
		if (fpsController.IsGrounded && speed > 0) {
			float period = footStepRate * fpsController.MaxSpeed / speed;
//			Debug.Log (period);
			//TODO keep track of last step
			if (Time.time - lastStep > period) {
				lastStep = Time.time;
//				detectableSounds.frequency = footStepRate * frequency;
//				detectableSounds.maxDistance = footStepDistance * period;
				int index = Random.Range (0, footStepClips.Length);
				detectableSounds.Play(footStepClips[index], footStepVolume, footStepVolume, 10, footStepDistance / period, 30f);
			}
		}

		//if jumped
	}

	void GetInput(){
		interact = GlobalScript.GetButton (GlobalScript.Interact);
		cancel = GlobalScript.GetButton (GlobalScript.Cancel);
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
}
