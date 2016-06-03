using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

[System.Serializable]
public class LockedData : SaveData{
	public bool locked = true;
	public bool opened = false;

	public LockedData () : base () {}
	public LockedData (SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
}

public class LockedScript : InteractableScript {
	public LockedData lockeddata;
	
	public GameObject lockObject;
	public GameObject openedObject;
	public GameObject closedObject;

	public bool toggleable = true;
	
	public override void Interact(GameObject go){
		if (lockeddata.locked) {
			if (go.transform.tag == "Player") {
				//set currentGameState to interacting
				go.GetComponent<PlayerScript> ().ChangeState (PlayerScript.State.Interacting);
				
				//create keylock instance
				lockObject.GetComponent<ToggleableScript>().Activate();
//				lockObject.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
//				lockObject.transform.position = new Vector3 (lockObject.transform.position.x, go.transform.position.y + 5, lockObject.transform.position.z);
			}
		} else if (toggleable){
			lockeddata.opened = !lockeddata.opened;
		}
	}

	void Start(){
		savedata = lockeddata;
	}

	void Update(){
		lockeddata = (LockedData)savedata;

		if (lockeddata.opened)
			Open ();
		else
			Close ();
	}
	
	public void SetLocked(bool l){
		lockeddata.locked = l;
	}

	public void Open(){
		if (closedObject.activeInHierarchy) {
			closedObject.SetActiveRecursively (false);
			openedObject.SetActiveRecursively (true);
		}
	}
	public void Close(){
		if (openedObject.activeInHierarchy) {
			openedObject.SetActiveRecursively (false);
			closedObject.SetActiveRecursively (true);
		}
	}
}
