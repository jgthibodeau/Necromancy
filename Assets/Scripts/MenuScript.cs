using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public void Load(){
		//TODO
		SaveLoad.Load ();
	}
	public void Save(){
		//TODO
		SaveLoad.Save ();
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
