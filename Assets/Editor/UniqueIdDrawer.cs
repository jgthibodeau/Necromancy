using UnityEditor;
using UnityEngine;
using System;

// Place this file inside Assets/Editor
[CustomPropertyDrawer (typeof(UniqueIdentifierAttribute))]
public class UniqueIdentifierDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label) {
		// Generate a unique ID, defaults to an empty string if nothing has been serialized yet
		if (prop.stringValue == "" || GuidExists(prop.stringValue)) {
			Guid guid = Guid.NewGuid();
			prop.stringValue = guid.ToString();
		}
		
		// Place a label so it can't be edited by accident
		Rect textFieldPosition = position;
		textFieldPosition.height = 16;
		EditorGUI.PropertyField (textFieldPosition, prop);
	}
	
	void DrawLabelField (Rect position, SerializedProperty prop, GUIContent label) {
		EditorGUI.LabelField(position, label, new GUIContent (prop.stringValue));
	}

//	bool GuidExists(string guid){
//		foreach (UniqueId uid in GameObject.FindObjectsOfType<UniqueId>()) {
//			if(guid == uid.uid)
//				return true;
//		}
//		return false;
//	}

	bool GuidExists(string guid){
		short found = 0;
		foreach (UniqueId uid in GameObject.FindObjectsOfType<UniqueId>()) {
			if(guid == uid.uid)
				found++;
			if(found >= 2)
				return true;
		}
		return false;
	}
}


//using UnityEditor;
//
//[CustomEditor( typeof( UniqueId ) )]
//
//class GuidInspector : Editor
//{
//	void OnEnable()
//	{
//		UnityEngine.Debug.Log ("gui enabled");
//
//		UniqueId guid = (UniqueId)target;
//		
//		if ( guid.guid == System.Guid.Empty )
//		{
//			guid.Generate();
//			EditorUtility.SetDirty( target );
//		}
//	}
//
//	void OnChange()
//	{
//		UnityEngine.Debug.Log ("gui changed");
//	}
//	
//	public override void OnInspectorGUI()
//	{
//		UnityEngine.Debug.Log ("on inspector gui");
//
//		UniqueId guid = (UniqueId)target;
//		
//		EditorGUILayout.SelectableLabel( guid.guid.ToString() );
//	}
//}