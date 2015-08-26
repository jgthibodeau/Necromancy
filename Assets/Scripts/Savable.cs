using UnityEngine;    // For Debug.Log, etc.

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

public class SavableScript : MonoBehaviour {
	public SaveData savedata;
	
	public void Save(){
		SaveLoad.Save(savedata);
	}
	
	public void Load(){
		SaveLoad.Save(savedata);
	}
}

// === This is the info container class ===
[Serializable ()]
public class SaveData : ISerializable {
	// The default constructor. Included for when we call it during Save() and Load()
	public SaveData () {}
	
	// This constructor is called automatically by the parent class, ISerializable
	// We get to custom-implement the serialization process here
	public SaveData (SerializationInfo info, StreamingContext ctxt)
	{
//		levelReached = (int)info.GetValue("levelReached", typeof(int));
		foreach(PropertyInfo prop in this.GetType().GetProperties()){
			prop.SetValue(this, info.GetValue(prop.Name, prop.GetType()), null);
		}
	}
	
	// Required by the ISerializable class to be properly serialized. This is called automatically
	public void GetObjectData (SerializationInfo info, StreamingContext ctxt)
	{
//		info.AddValue("levelReached", levelReached);
		foreach(PropertyInfo prop in this.GetType().GetProperties()){
			info.AddValue(prop.Name, prop.GetValue(this,null));
		}
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