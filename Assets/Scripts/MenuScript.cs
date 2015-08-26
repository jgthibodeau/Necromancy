using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public void Load(){
		//TODO
		foreach (SavableScript obj in GameObject.FindObjectsOfType<SavableScript> ()) {
			obj.Load ();
		}
	}
	public void Save(){
		//TODO
		foreach (SavableScript obj in GameObject.FindObjectsOfType<SavableScript> ()) {
			obj.Save ();
		}
	}
	public void SaveAndQuit(){
		Save ();
		Quit ();
	}
	public void Quit(){
		Application.Quit();
	}
	public void LoadLevel(string level){
		Time.timeScale = 1f;
		Application.LoadLevel (level);
	}
}
