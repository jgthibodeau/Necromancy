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

	private RigidbodyFirstPersonController firstPersonController;

	// Use this for initialization
	void Start () {
		savedata = playerdata;
		camera = GetComponentInChildren <Camera> ().transform;
		firstPersonController = GetComponent<RigidbodyFirstPersonController>();
	}

	void Update () {
		if (GlobalScript.currentGameState == GlobalScript.GameState.InGame)
			InGame ();
		else
			firstPersonController.doMovement = false;
	}

	void InGame () {
		playerdata = (PlayerData)savedata;

		//TODO ensure globalscript isnt paused for all scripts

		GetInput ();

		// Only do movement updates if in movement playerstate
		switch (currentState) {
		case State.Moving:
			firstPersonController.doMovement = true;
			if (interact) {
				InteractorScript interactor = GetComponent<InteractorScript> ();
				if (interactor != null) {
					interactor.Interact (camera);
				}
			}
			break;
		case State.Interacting:
			firstPersonController.doMovement = false;
			break;
		}
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
