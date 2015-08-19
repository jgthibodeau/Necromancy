using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

public class SaveLoad : MonoBehaviour {
	public static void SaveFile(){
		Stream fileStream = File.Open ("Save.sv", FileMode.Create);
		BinaryFormatter formatter = new BinaryFormatter();

		SavableScript[] objects = (GameObject[])GameObject.FindObjectsOfType<SavableScript> ();
		foreach (SavableScript obj in objects) {
			print(obj.Save());
//			formatter.Serialize (fileStream, obj);
		}
		fileStream.Close();
//		try {
//			Debug.Log("Writing Stream to Disk.", SaveLoad);
//			var fileStream:Stream = File.Open(filename, FileMode.Create);
//			var formatter:BinaryFormatter = new BinaryFormatter();
//			formatter.Serialize(fileStream, obj);
//			fileStream.Close();
//		} catch(e:Exception) {
//			Debug.LogWarning("Save.SaveFile(): Failed to serialize object to a file " + filename + " (Reason: " + e.ToString() + ")");
//		}
	}
	
	public static void LoadFile(){
		Stream fileStream = File.Open("Save.sv", FileMode.Open, FileAccess.Read);
		BinaryFormatter formatter = new BinaryFormatter ();


//		try {
//			Debug.Log("Reading Stream from Disk.", SaveLoad);
//			var fileStream:Stream = File.Open(filename, FileMode.Open, FileAccess.Read);
//			var formatter:BinaryFormatter = new BinaryFormatter();
//			var obj:Object= formatter.Deserialize(fileStream);
//			fileStream.Close();
//			return obj;
//		} catch(e:Exception) {
//			Debug.LogWarning("SaveLoad.LoadFile(): Failed to deserialize a file " + filename + " (Reason: " + e.ToString() + ")");
//			return null;
//		}       
	}
}
