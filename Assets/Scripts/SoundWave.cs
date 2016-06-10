using UnityEngine;
using System.Collections;

public class SoundWave : MonoBehaviour {
	public float volume;
	public float maxMagnitude;
	public float magnitude;
	public float speed;
	public SphereCollider collider;
	private int enemyLayer;

	public float Strength {
		get { return 1 - magnitude / maxMagnitude; }
	}

	public float CurrentVolume {
		get { return Strength * volume; }
	}

	// Use this for initialization
	void Start () {
		collider = GetComponent<SphereCollider> ();
		enemyLayer = LayerMask.NameToLayer("Enemy");
	}
	
	// Update is called once per frame
	void Update () {
		DebugExtension.DebugCircle (transform.position, magnitude);

		magnitude = magnitude + speed * Time.deltaTime;
		collider.radius = magnitude;

		if (magnitude > maxMagnitude)
			Destroy (gameObject);
	}

	void OnTriggerEnter(Collider other) {
		SoundDetector sd = other.gameObject.GetComponent<SoundDetector> ();
		if (sd != null) {
			sd.ReceiveSound (this);
		}
	}
}
