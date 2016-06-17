using UnityEngine;
using System.Collections;

public class UI3DScript : MonoBehaviour {
	public bool hideOnStart;
	public bool cancelable = true;
	public bool justOpened;
	private bool visible;

	protected virtual void Start(){
		visible = true;
		if (hideOnStart)
			Close ();
	}

	protected virtual void Update(){
		if (Visible () && !justOpened && cancelable) {
			if (GlobalScript.GetButton ("Cancel")) {
				Close ();
			}
		} else
			justOpened = false;
	}


	public virtual void Open(){
		visible = true;

		foreach (Transform child in transform) {
			child.gameObject.SetActive (true);
		}

		justOpened = true;
	}

	public virtual void Close(){
		Hide ();
	}

	public virtual void Hide(){
		visible = false;

		foreach (Transform child in transform) {
			child.gameObject.SetActive (false);
		}
	}

	public bool Visible(){
		return visible;
	}
}
