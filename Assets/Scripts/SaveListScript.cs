using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;

public class SaveListScript : MonoBehaviour {
	public GameObject saveFileButton;
	public Transform contentPanel;
	public MenuScript menu;
	public bool save;

	public Button.ButtonClickedEvent method;

	void Start () {
		PopulateList ();
	}
	
	void PopulateList () {
		foreach (Transform child in contentPanel.transform) {
			GameObject.Destroy(child.gameObject);
		}

		//TODO if menu is visible, hold onto selected index then reset to it

		foreach (FileInfo saveFile in SaveLoad.GetSaves()) {
			GameObject newButton = Instantiate (saveFileButton) as GameObject;
			newButton.GetComponentInChildren<Text>().text = saveFile.Name+"\n"+saveFile.LastWriteTime;
			string localFile = saveFile.Name;
			if(save)
				newButton.GetComponent<Button>().onClick.AddListener(delegate{menu.Save (localFile); this.PopulateList();});
			else
				newButton.GetComponent<Button>().onClick.AddListener(delegate{menu.Load (localFile);});
			newButton.transform.SetParent (contentPanel);
		}
	}
}