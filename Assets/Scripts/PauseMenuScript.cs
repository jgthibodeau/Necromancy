using UnityEngine;
using System.Collections;

public class PauseMenuScript : MenuScript {
	public GameObject pauseMenu;

	void Start(){
		UnPause ();
	}

	// Update is called once per frame
	void Update(){
		if (GlobalScript.GetButton ("Pause")) {
			if(GlobalScript.currentGameState == GlobalScript.GameState.InGame)
				Pause();
			else
				UnPause();
		}
	}

	public void Pause(){
		GlobalScript.currentGameState = GlobalScript.GameState.Paused;
		Time.timeScale = 0f;
		pauseMenu.SetActive (true);
	}
	public void UnPause(){
		GlobalScript.currentGameState = GlobalScript.GameState.InGame;
		Time.timeScale = 1f;
		pauseMenu.SetActive (false);
	}
}
