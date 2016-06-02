using UnityEngine;
using System.Collections;

public class ToggleableScript : SavableScript {
	public bool active;
	public bool toggleRenderers;

	Renderer[] renderers;
	Renderer renderer;

	public virtual void Activate(){
		active = true;
		if (toggleRenderers)
			EnableRenderers ();
	}

	void EnableRenderers(){
		foreach (Renderer r in renderers)
			r.enabled = true;
		if (renderer != null)
			renderer.enabled = true;
	}

	void DisableRenderers(){
		foreach (Renderer r in renderers)
			r.enabled = false;
		if (renderer != null)
			renderer.enabled = false;
	}

	public virtual void Deactivate(){
		active = false;
		if (toggleRenderers)
			DisableRenderers ();
	}

	public virtual void Start(){
		renderers = GetComponentsInChildren<Renderer>();
		renderer = GetComponent<Renderer> ();

		if (toggleRenderers) {
			if (active)
				EnableRenderers ();
			else
				DisableRenderers ();
		}
	}

	void Update(){
		if(active)
			ToggledUpdate ();
	}

	public virtual void ToggledUpdate(){
	}
}
