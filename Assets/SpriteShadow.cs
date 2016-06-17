using UnityEngine;
using System.Collections;

public class SpriteShadow : MonoBehaviour {
	public bool castShadows;
	public bool receiveShadows;

	// Use this for initialization
	void Start () {
		Renderer renderer = GetComponent<Renderer> ();
		renderer.receiveShadows = receiveShadows;
		renderer.castShadows = castShadows;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
