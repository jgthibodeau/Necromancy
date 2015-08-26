using UnityEngine;
using System.Collections;

public class LockedScript : InteractableScript {
	public bool locked = true;
	
	public GameObject lockObject;
	public GameObject openedObject;
	public GameObject closedObject;

	public bool toggleable = true;
	
	public override void Interact(GameObject go){
		if (locked) {
			if (go.transform.tag == "Player") {
				//set currentGameState to interacting
				go.GetComponent<PlayerScript> ().ChangeState (PlayerScript.State.Interacting);
				
				//create keylock instance
				lockObject.GetComponent<ToggleableScript>().Activate();
				lockObject.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
				lockObject.transform.position = new Vector3 (lockObject.transform.position.x, go.transform.position.y + 5, lockObject.transform.position.z);
			}
		} else if (toggleable){
			if(closedObject.activeInHierarchy)
				Open ();
			else
				Close ();
		}
	}
	
	public void SetLocked(bool l){
		locked = l;
	}

	public void Open(){
		closedObject.SetActiveRecursively (false);
		openedObject.SetActiveRecursively (true);
	}
	public void Close(){
		openedObject.SetActiveRecursively (false);
		closedObject.SetActiveRecursively (true);
	}
}
