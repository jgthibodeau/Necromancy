using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour {
	List<Highlight> highlights = new List<Highlight> ();

	public Color emissionColor = new Color32 (0xFF, 0x84, 0x00, 0xFF);
	public float emissionAmount = 0.6f;

	public Material highlightMat;

	// Use this for initialization
	void Start () {
		foreach (Renderer rend in GetComponentsInChildren<Renderer> ()) {
			Highlight h = rend.gameObject.AddComponent<Highlight> ();
			h.highlightMat = highlightMat;
			h.emissionColor = emissionColor;
			h.emissionAmount = emissionAmount;
			highlights.Add (h);
		}
	}

	public void SetHighlight() {
		foreach (Highlight h in highlights) {
			h.SetHighlight ();
		}
	}
}
