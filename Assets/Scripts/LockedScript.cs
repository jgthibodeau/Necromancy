using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

[System.Serializable]
public class LockedData : SaveData{
	public bool locked = true;
	public bool opened = false;
	public bool initiallyOpen = false;

	public LockedData () : base () {}
	public LockedData (SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
}

public class LockedScript : InteractableScript {
	public LockedData lockeddata;
	
	public GameObject lockObject;
	public GameObject openedObject;
	public GameObject closedObject;
	public GameObject animatedObject;

	public bool toggleable = true;
	private bool useAnimations;
	
	public override void Interact(GameObject go){
		if (lockeddata.locked) {
			if (go.transform.tag == "Player") {
				//set currentGameState to interacting
				go.GetComponent<PlayerController> ().ChangeState (PlayerController.State.Interacting);
				
				//create keylock instance
				lockObject.GetComponent<ToggleableScript>().Activate();
			}
		} else if (toggleable){
			lockeddata.opened = !lockeddata.opened;

			if (lockeddata.opened)
				Open ();
			else
				Close ();
		}
	}

	void Start(){
		savedata = lockeddata;
		useAnimations = (animatedObject != null);
	}

//	void Update(){
//		lockeddata = (LockedData)savedata;
//
//		if (lockeddata.opened)
//			Open ();
//		else
//			Close ();
//	}
	
	public void SetLocked(bool l){
		lockeddata.locked = l;
	}

	public void Open(){
		if (useAnimations) {
			animatedObject.GetComponent<Animator> ().SetBool ("open", true);
		} else {
			if (closedObject.activeInHierarchy) {
				closedObject.SetActiveRecursively (false);
				openedObject.SetActiveRecursively (true);
			}
		}
	}
	public void Close(){
		if (useAnimations) {
			animatedObject.GetComponent<Animator> ().SetBool ("open", false);
		} else {
			if (openedObject.activeInHierarchy) {
				openedObject.SetActiveRecursively (false);
				closedObject.SetActiveRecursively (true);
			}
		}
	}
}
