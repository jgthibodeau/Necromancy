//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//
//[CustomEditor(typeof(Light))]
//public class LightEditor : Editor {
//
//	private Light light;
//
//	void OnEnable()
//	{
//		light = target as Light;
//	}
//
//	public override void OnInspectorGUI()
//	{
//		//base.OnInspectorGUI();
//
//		GUIEditorBuilder.createLabel("__________________________");
//		light.color = GUIEditorBuilder.createColorField("Color:", light.color);
//	}
//}