using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
	public Color color;
	public Color pickupColor;
	public Color interactColor;
	public HighlightsFX.SortingType depthType;
	public GameObject highlightedObject;
	public GameObject highlightedPickupable;
	public GameObject highlightedInteractable;

//    [System.Serializable]
//    public class OutlineData
//    {
//        public Color color = Color.white;
//        public HighlightsFX.SortingType depthType;
//        public Renderer renderer;
//    }

    public HighlightsFX outlinePostEffect;
//    public OutlineData[] outliners;

//    private void Start()
//    {
//        foreach (var obj in outliners)
//        {
//            outlinePostEffect.AddRenderers(
//                new List<Renderer>() { obj.renderer }, 
//                obj.color, 
//                obj.depthType);
//        }
//    }
	public void SetRenderers(GameObject go) {
		if (go == null) {
			Debug.Log ("removing highlight");
			outlinePostEffect.ClearOutlineData ();
		}
		else if (highlightedObject != go) {
			Debug.Log ("highlighting " + go);
			outlinePostEffect.ClearOutlineData ();
			if (go != null) {
				List<Renderer> renderers = new List<Renderer> (go.GetComponentsInChildren<Renderer> ());
				for (int i = renderers.Count - 1; i >= 0; i--) {
					if (renderers [i] is ParticleSystemRenderer) {
						renderers.RemoveAt (i);
					}
//					Debug.Log(renderers[i]);
				}
				outlinePostEffect.AddRenderers (renderers, color, depthType);
			}
		}
	}

	public void SetPickupable(GameObject go) {
		highlightedPickupable = go;
	}

	public void SetInteractable(GameObject go) {
		highlightedInteractable = go;
	}

	public void UpdateHighlight() {
		outlinePostEffect.ClearOutlineData ();
		if (highlightedInteractable != null) {
			List<Renderer> renderers = new List<Renderer> (highlightedInteractable.GetComponentsInChildren<Renderer> ());
			HighlightRenderers (renderers, interactColor);
		}
		if (highlightedPickupable != null) {
			List<Renderer> renderers = new List<Renderer> (highlightedPickupable.GetComponentsInChildren<Renderer> ());
			HighlightRenderers (renderers, pickupColor);
		}
	}

	public void HighlightRenderers(List<Renderer> renderers, Color color){
		for (int i = renderers.Count - 1; i >= 0; i--) {
			if (renderers [i] is ParticleSystemRenderer) {
				renderers.RemoveAt (i);
			}
		}
		outlinePostEffect.AddRenderers (renderers, color, depthType);
	}
}
