using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {
	public Material highlightMat;
	public bool justHighlighted;
	public bool isHighlighted;

	public Color emissionColor;
	public float emissionAmount;

	private Renderer rend;
	private Material[] originalMats;
	private Material[] matsWithHighlight;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> ();

		originalMats = rend.materials;
		matsWithHighlight = new Material[rend.materials.Length + 1];
		originalMats.CopyTo(matsWithHighlight, 0);
		matsWithHighlight [matsWithHighlight.Length - 1] = highlightMat;
	}

	void Update () {
		//if we were told to highlight, and we arent, trigger the highlight
		if (justHighlighted) {
			if (!isHighlighted) {
				isHighlighted = true;
//				rend.materials = matsWithHighlight;

//				DynamicGI.SetEmissive (rend, emissionColor * emissionAmount);
			}
			justHighlighted = false;
		}
		//if we were not told to highlight, disable it
		else {
			isHighlighted = false;
//			rend.materials = originalMats;

//			DynamicGI.SetEmissive (rend, emissionColor * 0f);
		}
	}

	public void SetHighlight() {
		justHighlighted = true;
	}
}
