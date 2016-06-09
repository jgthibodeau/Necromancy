using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightCaster : MonoBehaviour {
	EntityDetector detector;
	Light light;
	Texture2D cookie;
	int tick = 0;
	int tickRate = 5;

	void Start(){
		detector = GetComponent<EntityDetector> ();
		light = GetComponent<Light> ();
		cookie = (Texture2D)light.cookie;
	}

	void FixedUpdate(){
		if (tick == 0)
			UpdateLight ();
		tick = (tick + 1) % tickRate;
	}

	void UpdateLight(){
		//do nothing if light is turned off
		if (!light.enabled)
			return;

		float lightLevel = 0f;
		
//		Debug.DrawRay (transform.position, transform.forward, Color.cyan);
		foreach (GameObject go in detector.entities.Keys) {
			DetectionData detectionData = (DetectionData)detector.entities [go];
			LightReceiver lightReceiver = go.GetComponent<LightReceiver> ();

			//remove this light from the entity if it no longer sees it
			if (detectionData.markedForDeletion) {
				detector.DeleteEntity (go);
				lightReceiver.RemoveLight (this);
			}

			/*Calculate light intensity at each point*/
			Vector3 point = detectionData.bottom();

			//calculate relative distance from light to point
			Vector3 direction = point - transform.position;
			float downwardDistance = Vector3.Dot (direction, transform.forward);
			Vector3 downward = transform.position + transform.forward * downwardDistance;

			Debug.DrawLine (transform.position, downward, Color.cyan);
			Debug.DrawLine (downward, point, Color.green);

			//calculate radius of light at point
			float radians = light.spotAngle * Mathf.PI / 180;
			float radius = Mathf.Abs (Mathf.Tan(radians/2)*downwardDistance);
			Debug.DrawRay (downward, (transform.right * radius), Color.red);

			//calculate relative location of point
			float scale =  cookie.width / (radius * 2);
			float relativeX = Mathf.Clamp (Vector3.Dot ((point - downward), transform.right) * scale + cookie.width/2, 0, cookie.width);
			float relativeY = Mathf.Clamp (Vector3.Dot ((point - downward), transform.up) * scale + cookie.width/2, 0, cookie.width);

			//get pixel data at relative point
			float alpha = cookie.GetPixel ((int)relativeX, (int)relativeY).a;
			float alphaPercent = (alpha - 0.027f) / 0.173f;

			//range + intensity -> real intensity
			float percentToApply = alphaPercent*detectionData.percentVisible;
			float percentIntensity = (light.range - downwardDistance) / light.range;
			float actualIntensity = light.intensity * percentIntensity;
			lightLevel = percentToApply * actualIntensity;
			Debug.Log (light.spotAngle/2+" "+downwardDistance+" "+radius+" "+lightLevel);

			//give light level to the entitiy
			lightReceiver.SetLight (this, lightLevel);
		}
	}
}

