using UnityEngine;
using System.Collections;

public class AnimationScript : MonoBehaviour {

	public Animator animator;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
