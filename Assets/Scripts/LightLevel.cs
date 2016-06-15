using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LightLevel : MonoBehaviour {
	public float attenuationOffset = 1f;
	public float attenuationFactor = 25f;
	public float distanceFactor = 2f;
	public float rangeFactor = 2f;
	public float intensityFactor = 1f;
	public float spotIntensityFactor = 0.33f;
	public float level = 0f;
	public Vector3[] lightSpots;

	public float priority;

	public Image lightUI;
	public Image lightSlider;


	private Collider collider;
	private Bounds bounds;

	// Use this for initialization
	void Start () {
		collider = GetComponent<Collider> ();
		lightSpots = new Vector3[3];
	}

	// Update is called once per frame
	void Update () {
		bounds = collider.bounds;

		lightSpots [0] = transform.position;
		lightSpots [1] = transform.up * (bounds.size.y / 4f - .1f);
		lightSpots [2] = lightSpots [1] * -1;

		lightSpots [1] += lightSpots [0];
		lightSpots [2] += lightSpots [0];

//		lightSpots [1] = transform.position;
//		lightSpots [2] = transform.position;

		level = 0f;

		//for all lights in scene:
		foreach(Light light in GameObject.FindObjectsOfType<Light> ()){
			float distance, angle;
			Vector3 point;
			VisibleByLight (light, out distance, out angle, out point);

			if (VisibleByLight (light, out distance, out angle, out point)) {
//				float attenuation = 1f / (1f + attenuationFactor * Mathf.Pow (distance, distanceFactor) / Mathf.Pow (light.range, rangeFactor));
				float attenuation = 1f;
				float cookieFactor = 1f;
				float brightness = 0F;

				if (light.type == LightType.Spot) {
					cookieFactor = CalculateCookieAlpha (light, angle, point);
					attenuation = (1 - distance / light.range);
					brightness = light.intensity * spotIntensityFactor * attenuation * cookieFactor;
				}
				else if (light.type == LightType.Point) {
					attenuation = 1f / (1f + attenuationFactor * Mathf.Pow (distance, distanceFactor)) * Mathf.Pow (light.range, rangeFactor) - attenuationOffset;
					brightness = Mathf.Pow (light.intensity, intensityFactor) * attenuation;
				}

				level += brightness;
			}
		}

		level = Mathf.Clamp (level, GlobalScript.minLightLevel, GlobalScript.maxLightLevel);
		level = (level - GlobalScript.minLightLevel) / GlobalScript.lightLevelRange;

		Color color = new Color (level, level, level, 255);
		lightUI.color = color;

		int low = 0;
		int high = 75;
		int difference = high - low;

		float height = low + difference * level;
		Vector3 lsp = lightSlider.rectTransform.anchoredPosition;
		lsp.y = height;
		lightSlider.rectTransform.anchoredPosition = lsp;

	}

	public bool VisibleByLight(Light light, out float minDistance, out float minAngle, out Vector3 minPoint){
		bool visible = false;
		minDistance = -1f;
		minAngle = -1f;
		minPoint = Vector3.zero;

		for (int i = 0; i < lightSpots.Length; i++) {
			Vector3 point = lightSpots [i];
			float distance = Vector3.Distance (point, light.transform.position);

			if (i == 0) {
				minDistance = distance;
				minPoint = point;
			}

			//cancel if player is too far away
			if (distance > light.range)
				break;

			RaycastHit hit;
			//if this is a spotlight, take into account its cone
			if (light.type == LightType.Spot) {
				Vector3 direction = point - light.transform.position;
				float angle = Vector3.Angle (light.transform.forward, direction);
				if (i == 0)
					minAngle = angle;

				if (angle*2 <= light.spotAngle && Physics.Linecast (light.transform.position, point, out hit)) {
					if (hit.transform == transform) {
						visible = true;
						Debug.DrawLine (light.transform.position, point, Color.blue);
					} else {
						Debug.DrawLine (light.transform.position, point, Color.red);
					}
				}
			}
			//otherwise, just do a regular raycast
			else{
				if (Physics.Linecast (light.transform.position, point, out hit)) {
					if (hit.transform == transform) {
						visible = true;
						Debug.DrawLine (light.transform.position, point, Color.blue);
					} else {
						Debug.DrawLine (light.transform.position, point, Color.red);
					}
				}
			}
		}
		return visible;
	}

	private float CalculateCookieAlpha(Light light, float angle, Vector3 point){
		Texture2D cookie = (Texture2D)light.cookie;
		if (cookie == null)
			return 1f;

		//calculate relative distance from light to point
		Vector3 direction = point - light.transform.position;
		float downwardDistance = Vector3.Dot (direction, light.transform.forward);
		Vector3 downward = light.transform.position +light.transform.forward * downwardDistance;

		Debug.DrawLine (light.transform.position, downward, Color.cyan);
		Debug.DrawLine (downward, point, Color.green);

		//calculate radius of light at point
		float radians = light.spotAngle * Mathf.PI / 180;
		float radius = Mathf.Abs (Mathf.Tan(radians/2)*downwardDistance);
		Debug.DrawRay (downward, (light.transform.right * radius), Color.red);

		//calculate relative location of point
		float scale =  cookie.width / (radius * 2);
		float relativeX = Mathf.Clamp (Vector3.Dot ((point - downward), light.transform.right) * scale + cookie.width/2, 0, cookie.width);
		float relativeY = Mathf.Clamp (Vector3.Dot ((point - downward), light.transform.up) * scale + cookie.width/2, 0, cookie.width);

		//get pixel data at relative point
		float alpha = cookie.GetPixel ((int)relativeX, (int)relativeY).a;
		return alpha;
	}
}