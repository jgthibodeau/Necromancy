using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaintingScript : MonoBehaviour {
	public enum State{Locked,Unlocked};
	public State currentState = State.Locked;

	private List<Vector2> cutPoints = new List<Vector2>();

	private bool cutting = false;
	private bool cut = false;

	public float cutTolerance = 0.01f;
	public int minCuts = 5;

	public float randomnessAmount;
	public float randomAngle;
	
	public float rotateSpeed = 0.5f;
	public float moveSpeed = 0.05f;
	public float rollSpeed;

	private Bounds paintingBounds;

	public Transform painting;
	private Sprite originalPainting;
	public Transform cutter;
	public Sprite paintingKnife;
	public Sprite paintingKnifeCutting;
	public LineRenderer lineRenderer;
	public GameObject lockedObject;
	
	// Inputs
	private bool interact;
	private bool cancel;
	private Vector2 prevInputLeft;
	private Vector2 inputLeft;
	private Vector2 prevInputRight;
	private Vector2 inputRight;
	private float leftTrigger;
	private float rightTrigger;
	
	// Sounds
//	public AudioClip newTumblerClip;
//	public AudioClip correctAngleClip;
	public AudioClip doneCuttingClip;
	public AudioSource audioSource;
	public AudioSource cuttingAudioSource;
	
	void Start(){
		originalPainting = painting.GetComponent<SpriteRenderer> ().sprite;
		paintingBounds = painting.GetComponent<Renderer>().bounds;
		cutter.position = new Vector3(paintingBounds.max.x, cutter.position.y, paintingBounds.max.z);
	}
	
	void Update(){
		switch(currentState){
		case State.Locked:
			GetInput ();
			
			if (cancel) {
				Deactivate ();
				return;
			}

			if(!Looped()){

				//check if cutting
				if(rightTrigger > 0.25f){

					cutting=true;
					cut=true;

					//set cutter sprite to the cutting knife
					cutter.GetComponent<SpriteRenderer>().sprite = paintingKnifeCutting;

					//if moving
					if(inputRight.magnitude > 0){
						//update knife position
						Vector3 oldPosition = cutter.position;

						AddRandomness(inputRight.magnitude);

						MoveKnife(inputRight);

						//add point to line renderer if it's new
						if (!cutter.position.Equals(oldPosition)){
							Vector3 point = new Vector3(cutter.localPosition.x, cutter.localPosition.y - .05f, cutter.localPosition.z);

							int x = (int)(originalPainting.rect.width/2f + originalPainting.pixelsPerUnit*point.x);
							int y = (int)(originalPainting.rect.height/2f + originalPainting.pixelsPerUnit*point.z);

							Vector2 prevPoint = new Vector2(x-10,y-10);
							if(cutPoints.Count > 0)
								prevPoint = cutPoints[cutPoints.Count-1];
							if(x != prevPoint.x || y != prevPoint.y){
								cutPoints.Add(new Vector2(x, y));
								lineRenderer.SetVertexCount(cutPoints.Count);
								lineRenderer.SetPosition(cutPoints.Count-1, point);
							}

							
							if(!cuttingAudioSource.isPlaying)
								cuttingAudioSource.Play();
						}
						else{
							cuttingAudioSource.Pause ();
							cuttingAudioSource.Stop ();
						}
					} else{
						cuttingAudioSource.Pause ();
						cuttingAudioSource.Stop ();
					}
				}
				else{
					cutting=false;
					cuttingAudioSource.Pause ();
					cuttingAudioSource.Stop ();

					//set cutter sprite to the regular knife
					cutter.GetComponent<SpriteRenderer>().sprite = paintingKnife;

					//clear randomness
					randomAngle = 0f;
				}
			}
			else{
				cuttingAudioSource.Pause ();
				cuttingAudioSource.Stop ();
				audioSource.PlayOneShot(doneCuttingClip,1f);

				//Replace texture with the newly cut one
				SpriteRenderer renderer = painting.GetComponent<SpriteRenderer>();
				Texture2D tex = renderer.sprite.texture;

				Texture2D newTex = (Texture2D)GameObject.Instantiate(tex);

				for(int i=0;i<originalPainting.rect.width;i++){
					for(int j=0;j<originalPainting.rect.height;j++){
						if(!GlobalScript.IsPointInPolygon(cutPoints, new Vector2(i,j)))
							newTex.SetPixel(i,j,Color.clear);
					}
				}
				
				newTex.Apply();
				renderer.sprite = Sprite.Create(newTex, originalPainting.rect, new Vector2(.5f,.5f), originalPainting.pixelsPerUnit);
			}


			Vector3 actualDirectionOfMotion = new Vector3 (inputRight.x, 0, inputRight.y);
			if (actualDirectionOfMotion.magnitude > 0) {
				float angle = Vector3.Angle (actualDirectionOfMotion, Vector3.forward) * Mathf.Sign (actualDirectionOfMotion.x);
				cutter.rotation = Quaternion.Euler (90, angle-randomAngle, 180);
			}

			break;
		case State.Unlocked:
			//TODO
			lineRenderer.enabled = false;
			painting.localScale = Vector3.Slerp (painting.localScale, new Vector3(0f,1f,1f), rollSpeed*Time.deltaTime);
			if(painting.localScale.x <= .3f)
				Deactivate();
			break;
		}
	}

	public bool Looped(){
		if (cutPoints.Count > minCuts && Vector2.Distance (cutPoints [0], cutPoints [cutPoints.Count - 1]) < cutTolerance) {
			currentState = State.Unlocked;
			return true;
		}
		return false;
	}

	public void AddRandomness(float speed){
		randomAngle += speed * Random.Range (-1 * randomnessAmount, randomnessAmount);
	}

	public void MoveKnife(Vector2 input){
		//TODO maybe remove this
		Vector3 movement = new Vector3 (input.x, 0f, input.y);

		movement = Quaternion.AngleAxis(-randomAngle, Vector3.up) * movement;

		Vector3 fixedPosition = Vector3.Slerp(cutter.position, cutter.position + movement, Time.deltaTime*moveSpeed);

		paintingBounds = painting.GetComponent<Renderer>().bounds;

		if (fixedPosition.x < paintingBounds.min.x)
			fixedPosition.x = paintingBounds.min.x;
		else if (fixedPosition.x > paintingBounds.max.x)
			fixedPosition.x = paintingBounds.max.x;

		if (fixedPosition.z < paintingBounds.min.z)
			fixedPosition.z = paintingBounds.min.z;
		else if (fixedPosition.z > paintingBounds.max.z)
			fixedPosition.z = paintingBounds.max.z;

		cutter.position = fixedPosition;
	}
	
	bool Unlocked(){
		return false;
		return true;
	}
	
	void FixedUpdate(){
		UpdateLocation ();
	}
	
	//Disable the lock, set all variables to default, return control to player
	void Deactivate(){
		cuttingAudioSource.Stop ();
		GameObject.Find ("Player").GetComponent<PlayerScript>().ChangeState(PlayerScript.State.Moving);
		this.gameObject.SetActiveRecursively(false);
	}
	
	void GetInput(){
		interact = GlobalScript.GetButton (GlobalScript.Interact);
		cancel = GlobalScript.GetButton (GlobalScript.Cancel);
		prevInputLeft = inputLeft;
		inputLeft = GlobalScript.GetAxis(GlobalScript.LeftStick);
		prevInputRight = inputRight;
		inputRight = GlobalScript.GetAxis(GlobalScript.RightStick);
		leftTrigger = GlobalScript.GetTrigger (GlobalScript.LeftTrigger);
		rightTrigger = GlobalScript.GetTrigger (GlobalScript.RightTrigger);
	}
	
	//Ensure the lock is always centered on the screen
	void UpdateLocation(){
		float oldY = this.transform.position.y;
		this.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
		this.transform.position = new Vector3 (this.transform.position.x, oldY, this.transform.position.z);
	}
}
