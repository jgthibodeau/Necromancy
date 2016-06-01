using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;

public class SaveLoadMenuScript : MenuScript {
	public GameObject saveFileButton;
	public Transform contentPanel;
	public bool save;

	public override void Open(){
		PopulateList ();
		base.Open ();
	}

	public override void Close(){
		base.Close ();
		//set alert text to say it has saved or loaded
	}
	
	void PopulateList () {
		foreach (Transform child in contentPanel.transform) {
			GameObject.Destroy(child.gameObject);
		}

		//TODO if menu is visible, hold onto selected index then reset to it

		foreach (FileInfo saveFile in SaveLoad.GetSaves()) {
			GameObject newButton = Instantiate (saveFileButton) as GameObject;
			string filename = saveFile.Name.Replace (".bly", "");
			newButton.GetComponentInChildren<Text>().text = filename+"\n"+saveFile.LastWriteTime;
			if(save)
				newButton.GetComponent<Button>().onClick.AddListener(delegate{Save (saveFile.Name); SwitchToPrevious();});
			else
				newButton.GetComponent<Button>().onClick.AddListener(delegate{Load (saveFile.Name); SwitchToPrevious();});
			newButton.transform.SetParent (contentPanel);

			GameObject es = GameObject.Find ("EventSystem");
			es.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (null);
		}
	}
}