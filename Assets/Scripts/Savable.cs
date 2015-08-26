using UnityEngine;

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

public class SavableScript : MonoBehaviour {
	public SaveData savedata;
	
	public virtual void Save(){
		savedata.position = transform.position;
		savedata.rotation = transform.rotation.eulerAngles;
		//TODO save all other savables
		SaveLoad.Save(savedata);
	}
	
	public virtual SaveData Load(){
		transform.position = savedata.position;
		Vector3 rotation = savedata.rotation;
		transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
		//TODO load all other loadables
		return SaveLoad.Load();
	}
}

// === This is the info container class ===
[Serializable ()]
public class SaveData : ISerializable {
//	TODO public Transform transform;

	public Vector3 position;
	public Vector3 rotation;

	// The default constructor. Included for when we call it during Save() and Load()
	public SaveData () {}
	
	// This constructor is called automatically by the parent class, ISerializable
	// We get to custom-implement the serialization process here
	public SaveData (SerializationInfo info, StreamingContext ctxt)
	{
		foreach(FieldInfo field in this.GetType().GetFields()){
//			UnityEngine.Debug.Log(field.Name+" "+field.FieldType);
			if(field.FieldType == typeof(Vector3))
				field.SetValue(this, LoadVector3(info, field.Name));
			else
				field.SetValue(this, info.GetValue(field.Name, field.FieldType));
		}
	}
	
	// Required by the ISerializable class to be properly serialized. This is called automatically
	public void GetObjectData (SerializationInfo info, StreamingContext ctxt)
	{
		foreach(FieldInfo field in this.GetType().GetFields()){
			if(field.FieldType == typeof(Vector3))
				SaveVector3(info, (Vector3)field.GetValue(this), field.Name);
			else
				info.AddValue(field.Name, field.GetValue(this));
		}
	}

	public void SaveVector3(SerializationInfo info, Vector3 vector, String name){
		info.AddValue(name+"x", vector.x);
		info.AddValue(name+"y", vector.y);
		info.AddValue(name+"z", vector.z);
	}
	public Vector3 LoadVector3(SerializationInfo info, String name){
		float x = (float)info.GetValue(name+"x", typeof(float));
		float y = (float)info.GetValue(name+"y", typeof(float));
		float z = (float)info.GetValue(name+"z", typeof(float));
		return new Vector3 (x, y, z);
	}
}

// === This is the class that will be accessed from scripts ===
public class SaveLoad {
	
	public static string currentFilePath = "SaveData.cjc";    // Edit this for different save files
	
	// Call this to write data
	public static void Save (SaveData data)  // Overloaded
	{
		Save (currentFilePath, data);
	}
	public static void Save (string filePath, SaveData data)
	{
		Stream stream = File.Open(filePath, FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder(); 
		bformatter.Serialize(stream, data);
		stream.Close();
	}
	
	// Call this to load from a file into "data"
	public static SaveData Load ()  { return Load(currentFilePath);  }   // Overloaded
	public static SaveData Load (string filePath) 
	{
		SaveData data = new SaveData ();
		Stream stream = File.Open(filePath, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		data = (SaveData)bformatter.Deserialize(stream);
		stream.Close();

		return data;
	}
}

// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
// Do not change this
public sealed class VersionDeserializationBinder : SerializationBinder 
{ 
	public override Type BindToType( string assemblyName, string typeName )
	{ 
		if ( !string.IsNullOrEmpty( assemblyName ) && !string.IsNullOrEmpty( typeName ) ) 
		{ 
			Type typeToDeserialize = null; 
			
			assemblyName = Assembly.GetExecutingAssembly().FullName; 
			
			// The following line of code returns the type. 
			typeToDeserialize = Type.GetType( String.Format( "{0}, {1}", typeName, assemblyName ) ); 
			
			return typeToDeserialize; 
		} 
		
		return null; 
	} 
}
