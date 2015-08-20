using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public static class SaveLoad  {
	public static void Save() {
		//TODO
		//save all savables
		//save current level
//		GameObject.FindObjectOfType<PlayerScript> ().Save ();
		foreach(SavableScript obj in GameObject.FindObjectsOfType<SavableScript> ())
			obj.Save();
	}
	
	public static void Load() {
		//TODO
		//load saved level
		//load all loadables
//		GameObject.FindObjectOfType<PlayerScript> ().Load ();
		foreach(SavableScript obj in GameObject.FindObjectsOfType<SavableScript> ())
			obj.Load();
	}
}
