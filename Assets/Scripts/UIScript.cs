using UnityEngine;
using System.Collections;

public class UIScript : MonoBehaviour {
	public bool hideOnStart;
	public bool cancelable = true;
	public bool justOpened;

	private Canvas menu;
	private CanvasGroup menuGroup;

	protected virtual void Start(){
		menu = GetComponent<Canvas> ();
		menuGroup = GetComponent<CanvasGroup> ();

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
		menuGroup.interactable = true;
		menu.enabled = true;
		justOpened = true;
	}

	public virtual void Close(){
		Hide ();
	}

	public virtual void Hide(){
		menuGroup.interactable = false;
		menu.enabled = false;
	}

	public bool Visible(){
		return menuGroup.interactable;
	}
}
