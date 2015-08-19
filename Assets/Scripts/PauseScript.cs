using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {
	public GameObject pauseMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Update(){
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
