using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Runtime.Serialization;
using System.Reflection;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;


public class SavableScript : MonoBehaviour {
	public void SaveVar(){
		XmlSerializer serializer = new XmlSerializer(typeof(float));
		using (StringWriter writer = new StringWriter()) {
			serializer.Serialize(writer, this.transform.position.x);
			PlayerPrefs.SetString(this.name + "positionx", writer.ToString());
		}
	}
	public void LoadVar(){
	}

	public void Save(){
		print ("saving "+this.name);
		XmlSerializer serializer;
		FieldInfo[] fields = this.GetType().GetFields();
		foreach (FieldInfo f in fields) {
			bool serializable = f.FieldType.IsSerializable && f.FieldType != typeof(Transform[]) && f.FieldType != typeof(GameObject[]);
			//TODO make this properly detect Transform[]
			print (this.name+" "+f.Name+" "+f.FieldType+" "+f.GetValue(this)+" "+serializable);
			if(serializable){
				serializer = new XmlSerializer(f.FieldType);
				using (StringWriter writer = new StringWriter()) {
					serializer.Serialize(writer, f.GetValue(this));
					PlayerPrefs.SetString(this.name + f.Name, writer.ToString());
				}
			}
		}

		serializer = new XmlSerializer(typeof(float));
		using (StringWriter writer = new StringWriter()) {
			serializer.Serialize(writer, this.transform.position.x);
			PlayerPrefs.SetString(this.name + "positionx", writer.ToString());
		}
		serializer = new XmlSerializer(typeof(float));
		using (StringWriter writer = new StringWriter()) {
			serializer.Serialize(writer, this.transform.position.y);
			PlayerPrefs.SetString(this.name + "positiony", writer.ToString());
		}
		serializer = new XmlSerializer(typeof(float));
		using (StringWriter writer = new StringWriter()) {
			serializer.Serialize(writer, this.transform.position.z);
			PlayerPrefs.SetString(this.name + "positionz", writer.ToString());
		}

		serializer = new XmlSerializer(typeof(float));
		using (StringWriter writer = new StringWriter()) {
			serializer.Serialize(writer, this.transform.rotation.x);
			PlayerPrefs.SetString(this.name + "rotationx", writer.ToString());
		}
		serializer = new XmlSerializer(typeof(float));
		using (StringWriter writer = new StringWriter()) {
			serializer.Serialize(writer, this.transform.rotation.y);
			PlayerPrefs.SetString(this.name + "rotationy", writer.ToString());
		}
		serializer = new XmlSerializer(typeof(float));
		using (StringWriter writer = new StringWriter()) {
			serializer.Serialize(writer, this.transform.rotation.z);
			PlayerPrefs.SetString(this.name + "rotationz", writer.ToString());
		}
		serializer = new XmlSerializer(typeof(float));
		using (StringWriter writer = new StringWriter()) {
			serializer.Serialize(writer, this.transform.rotation.w);
			PlayerPrefs.SetString(this.name + "rotationw", writer.ToString());
		}
		
//		serializer = new XmlSerializer(typeof(float));
//		using (StringWriter writer = new StringWriter()) {
//			serializer.Serialize(writer, this.transform.localScale.x);
//			PlayerPrefs.SetString(this.name + "localScalex", writer.ToString());
//		}
//		serializer = new XmlSerializer(typeof(float));
//		using (StringWriter writer = new StringWriter()) {
//			serializer.Serialize(writer, this.transform.localScale.y);
//			PlayerPrefs.SetString(this.name + "localScaley", writer.ToString());
//		}
//		serializer = new XmlSerializer(typeof(float));
//		using (StringWriter writer = new StringWriter()) {
//			serializer.Serialize(writer, this.transform.localScale.z);
//			PlayerPrefs.SetString(this.name + "localScalez", writer.ToString());
//		}
	}
	
	public void Load(){
		print ("loading "+this.name);
		XmlSerializer serializer;
		FieldInfo[] fields = this.GetType().GetFields();
		foreach (FieldInfo f in fields){
			bool serializable = f.FieldType.IsSerializable && f.FieldType != typeof(Transform[]) && f.FieldType != typeof(GameObject[]);
			if(serializable){
				serializer = new XmlSerializer(f.FieldType);
				using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + f.Name))) {
					if (PlayerPrefs.HasKey(this.name + f.Name))
						f.SetValue(this, serializer.Deserialize(reader));
				}
			}
		}

		this.transform.position = Vector3.zero;
		serializer = new XmlSerializer(typeof(float));
		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "positionx"))) {
			if (PlayerPrefs.HasKey(this.name + "positionx"))
				this.transform.position += new Vector3((float)serializer.Deserialize(reader),0,0);
		}  
		serializer = new XmlSerializer(typeof(float));
		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "positiony"))) {
			if (PlayerPrefs.HasKey(this.name + "positionz"))
				this.transform.position += new Vector3(0,(float)serializer.Deserialize(reader),0);
		}  
		serializer = new XmlSerializer(typeof(float));
		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "positionz"))) {
			if (PlayerPrefs.HasKey(this.name + "positionz"))
				this.transform.position += new Vector3(0,0,(float)serializer.Deserialize(reader));
		}  
				
		Quaternion rotation = new Quaternion();
		serializer = new XmlSerializer(typeof(float));
		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "rotationx"))) {
			if (PlayerPrefs.HasKey(this.name + "rotationx"))
				rotation.x = (float)serializer.Deserialize(reader);
		}  
		serializer = new XmlSerializer(typeof(float));
		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "rotationy"))) {
			if (PlayerPrefs.HasKey(this.name + "rotationy"))
				rotation.y = (float)serializer.Deserialize(reader);
		}  
		serializer = new XmlSerializer(typeof(float));
		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "rotationz"))) {
			if (PlayerPrefs.HasKey(this.name + "rotationz"))
				rotation.z = (float)serializer.Deserialize(reader);
		}  
		serializer = new XmlSerializer(typeof(float));
		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "rotationw"))) {
			if (PlayerPrefs.HasKey(this.name + "rotationw"))
				rotation.w = (float)serializer.Deserialize(reader);
		}
		this.transform.rotation = rotation;

//		this.transform.localScale = Vector3.zero;
//		serializer = new XmlSerializer(typeof(float));
//		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "localScalex"))) {
//			if (PlayerPrefs.HasKey(this.name + "localScalex"))
//				this.transform.position += new Vector3((float)serializer.Deserialize(reader),0,0);
//		}  
//		serializer = new XmlSerializer(typeof(float));
//		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "localScaley"))) {
//			if (PlayerPrefs.HasKey(this.name + "localScalez"))
//				this.transform.position += new Vector3(0,(float)serializer.Deserialize(reader),0);
//		}  
//		serializer = new XmlSerializer(typeof(float));
//		using (StringReader reader = new StringReader(PlayerPrefs.GetString(this.name + "localScalez"))) {
//			if (PlayerPrefs.HasKey(this.name + "localScalez"))
//				this.transform.position += new Vector3(0,0,(float)serializer.Deserialize(reader));
//		}  
	}
}
