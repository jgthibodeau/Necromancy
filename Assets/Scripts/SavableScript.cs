using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

public class SavableScript : MonoBehaviour {
	public static void Save(SerializationInfo info, StreamingContext ctxt){
		FieldInfo[] fields = this.GetType().GetFields();
		foreach (FieldInfo f in fields) {
			FieldInfo fi = this.GetType().GetField(f.Name);
			if (fi == null)
				Debug.Log("Field is null! Property name: " + f.Name + " --- Type: " + f.FieldType);
			fi.SetValue(this, info.GetValue(f.Name, f.FieldType));
		}

//		formatter.Serialize (fileStream, obj);
	}
	
	public static void Load(SerializationInfo info, StreamingContext ctxt){
		FieldInfo[] fields = this.GetType().GetFields();
		foreach (FieldInfo f in fields){
			info.AddValue(f.Name, f.GetValue(this));
		}
//		Object obj = formatter.Deserialize(fileStream);     
	}
}
