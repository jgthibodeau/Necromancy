using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public void Save(){
		SaveLoad.SaveAll ();
	}

	public void Load(){
		SaveLoad.LoadAll ();
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
}
