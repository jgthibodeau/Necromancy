using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public void Load(){
		Time.timeScale = 1f;
		//TODO
	}
	public void Save(){
		//TODO
		SaveLoadScript.SaveFile ();
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
