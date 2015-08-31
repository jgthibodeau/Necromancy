using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SaveListScript : MonoBehaviour {
	public GameObject saveFileButton;
	public Transform contentPanel;
	public MenuScript menu;

	public Button.ButtonClickedEvent method;

	void Start () {
		PopulateList ();
	}
	
	void PopulateList () {
		foreach (string saveFile in SaveLoad.GetSaves()) {
			GameObject newButton = Instantiate (saveFileButton) as GameObject;
			newButton.GetComponentInChildren<Text>().text = saveFile;
			newButton.GetComponent<Button>().onClick.AddListener(delegate{menu.Save (saveFile);});
			newButton.transform.SetParent (contentPanel);
		}
	}
}