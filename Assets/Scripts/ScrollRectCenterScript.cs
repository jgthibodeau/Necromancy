using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent( typeof( ScrollRect ) )]
public class ScrollRectCenterScript : MonoBehaviour {
	
	public float scrollSpeed = 10f;
	
	ScrollRect scrollRect;
	RectTransform rectTransform;
	RectTransform contentRectTransform;
	RectTransform selectedRectTransform;
	
	void Awake() {
		scrollRect = GetComponent<ScrollRect>();
		rectTransform = GetComponent<RectTransform>();
		contentRectTransform = scrollRect.content;
	}
	
	void Update() {
		UpdateScrollToSelected();
	}
	
	void UpdateScrollToSelected() {
		
		// grab the current selected from the eventsystem
		GameObject selected = EventSystem.current.currentSelectedGameObject;
		
		if ( selected == null ) {
			return;
		}
		if ( selected.transform.parent != contentRectTransform.transform ) {
			return;
		}
		
		selectedRectTransform = selected.GetComponent<RectTransform>();
		
		// math stuff
		Vector3 selectedDifference = rectTransform.localPosition - selectedRectTransform.localPosition;
		float contentHeightDifference = ( contentRectTransform.rect.height - rectTransform.rect.height );
		
		float selectedPosition = ( contentRectTransform.rect.height - selectedDifference.y );
		float currentScrollRectPosition = scrollRect.normalizedPosition.y * contentHeightDifference;
		float above = currentScrollRectPosition /*- ( selectedRectTransform.rect.height/2 )*/ + rectTransform.rect.height;
		float below = currentScrollRectPosition + ( selectedRectTransform.rect.height );

		// check if selected is out of bounds
		print (selectedPosition+" "+above);
		if (selectedPosition > above) {
			float step = selectedPosition - above;
			float newY = currentScrollRectPosition + step;
			float newNormalizedY = newY / contentHeightDifference;
			scrollRect.normalizedPosition = Vector2.Lerp (scrollRect.normalizedPosition, new Vector2 (0, newNormalizedY), scrollSpeed * Time.unscaledDeltaTime);
		} else if (selectedPosition < below) {
			float step = selectedPosition - below;
			float newY = currentScrollRectPosition + step;
			float newNormalizedY = newY / contentHeightDifference;
			scrollRect.normalizedPosition = Vector2.Lerp (scrollRect.normalizedPosition, new Vector2 (0, newNormalizedY), scrollSpeed * Time.unscaledDeltaTime);
		}
		
	}
	
}