using UnityEngine;
using System.Collections;

public class Unrotatable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.transform.rotation = Quaternion.Euler(90, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.rotation = Quaternion.Euler(90, 0, 0);
	}
}
