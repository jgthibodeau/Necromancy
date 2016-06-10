using UnityEngine;
using System.Collections;

public class OccludableAudio : MonoBehaviour
{
//	private Transform m_MyTrans;
	private AudioSource m_Source;
	private AudioLowPassFilter m_Filter;
	private float m_Volume;
	private float m_Frequency;

	public Transform Listener;
	public LayerMask Mask;
	public float dampingFactor = 0.5f;
	public float mufflingFactor = 0.5f;
	public float fadeSpeed = 10f;

	void Start()
	{
		m_Source = GetComponent<AudioSource> ();
		m_Filter = GetComponent<AudioLowPassFilter> ();
		m_Volume = m_Source.volume;
		m_Frequency = m_Filter.cutoffFrequency;
		Listener = Camera.main.transform;
	}
	void Update()
	{
		Vector3 direction = Listener.position - transform.position;
		if (direction.magnitude < m_Source.maxDistance) {
			Debug.DrawRay (transform.position, direction, Color.gray);

			float volume = m_Volume;
			float frequency = m_Frequency;

			foreach (RaycastHit hit in Physics.RaycastAll (transform.position, direction, direction.magnitude, Mask)) {
				volume *= dampingFactor;
				frequency *= mufflingFactor;
			}

			m_Source.volume = Mathf.Lerp (m_Source.volume, volume, Time.deltaTime * fadeSpeed);
			m_Filter.cutoffFrequency = Mathf.Lerp (m_Filter.cutoffFrequency, frequency, Time.deltaTime * fadeSpeed);
		}
	}
}
