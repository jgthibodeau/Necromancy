﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour {
	public Text alertText;
	public float alertTime = 3;
	private float remainingAlertTime = -1;

	public bool hideOnStart;
	public bool justOpened;

	private Canvas menu;
	private CanvasGroup menuGroup;

	public MenuScript previousMenu;

	private EventSystem es;

	public void SwitchTo(GameObject other){
		MenuScript newMenu = other.GetComponent<MenuScript> ();
		newMenu.previousMenu = this;
		SwitchToMenu (newMenu);
	}

	public void SwitchToPrevious(){
		SwitchToMenu (previousMenu);
	}

	public void SwitchToMenu(MenuScript other){
		Hide ();
		other.Open ();
	}

	public virtual void Open(){
		menuGroup.interactable = true;
		menu.enabled = true;
		justOpened = true;

//		es.SetSelectGameObject(go, null);
		es.SetSelectedGameObject (transform.GetChild(1).gameObject);
	}

	public virtual void Close(){
		Hide ();
	}

	public virtual void Hide(){
		menuGroup.interactable = false;
		menu.enabled = false;
	}

	public bool Visible(){
		return menu.enabled;
	}

	public void Save(){
		Alert ("Saving ...");
		SaveLoad.SaveAll ();
		Alert ("Saving Complete");
	}

	public void Save(string file){
		Alert ("Saving ...");
		SaveLoad.SaveAll (file);
		Alert ("Saving Complete");
	}

	public List<string> GetSaves(){
		return SaveLoad.GetSaves ();
	}

	public void Load(){
		SaveLoad.LoadAll ();
		Time.timeScale = 1f;
	}

	public void SaveAndQuit(){
		Save ();
		Quit ();
	}
	public void Quit(){
		Application.Quit();
	}
	public void LoadLevel(string level){
		GlobalScript.LoadLevel (level);
	}

	public void Alert(string text){
		alertText.text = text;
		remainingAlertTime = alertTime;
	}

	protected virtual void Start(){
		es = EventSystem.current;

		menu = GetComponent<Canvas> ();
		menuGroup = GetComponent<CanvasGroup> ();

		if (hideOnStart)
			Close ();

		alertText.text = "";
	}

	protected virtual void Update(){
		if (remainingAlertTime > 0) {
			remainingAlertTime -= Time.unscaledDeltaTime;
			if (remainingAlertTime <= 0)
				alertText.text = "";
		}

		if (Visible () && !justOpened) {
			if (GlobalScript.GetButton ("Cancel")) {
				if (previousMenu != null)
					SwitchToPrevious ();
				else
					Close ();
			}
		} else
			justOpened = false;
	}
}
