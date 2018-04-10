using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour {
	public static MyGameManager instance = null;

	public float oceanLevel;
	public Transform ocean;
	public LayerMask oceanLayer;
	public float oceanCheckDistance;

	public int maxInstances = 100;
	public List<GameObject> instances;

	void Awake(){
		Cursor.lockState = CursorLockMode.Locked;

		if (instance == null) {
			instance = this;
			instances = new List<GameObject> ();
		} else if (instance != this) {
			Destroy (gameObject);
		}

		oceanLevel = ocean != null ? ocean.position.y : oceanLevel;
	}

	public void AddInstace(GameObject instanceObj) {
		int numberInstances = instances.Count;
		if (numberInstances > maxInstances) {
			int index = Random.Range (0, numberInstances);
			GameObject removedInstance = instances [index];
			instances.RemoveAt (index);
			GameObject.Destroy (removedInstance);
		}
		instances.Add (instanceObj);
	}

	public void RemoveInstance(GameObject instanceObj) {
		if (instances.Contains (instanceObj)) {
			instances.Remove (instanceObj);
		}
		GameObject.Destroy (instanceObj);
	}

//	public IEnumerator Destroy(GameObject go) {
//		yield return new WaitForFixedUpdate ();
//	}
}

