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
			Button newButton = Instantiate (saveFileButton).GetComponent<Button>();
			newButton.GetComponentInChildren<Text>().text = saveFile;
			string localFile = saveFile;
			newButton.onClick.AddListener(delegate{menu.Save (localFile);});
			newButton.transform.SetParent (contentPanel);
		}
	}
}