using UnityEngine;
using System.Collections;

public class FpsController : MonoBehaviour {
	public float sensitivityX;
	public float sensitivityY;

	public bool invertX;
	public bool invertY;

	public float minX;
	public float maxX;

	public bool smoothMove;
	public bool smoothLook;
	public float smoothFactor;
	public float smoothLookFactor;

	public float gravityMultiplier;
	public float speed;
	public float runMultiplier;
	public float crouchMultiplier;
	public float jumpSpeed;

	public bool prevGrounded;
	public float stickToGroundForce;

	public bool isRunning;
	public bool isCrouching;
	public bool isJumping;
	private bool jump;
	private Vector2 moveInput;
	private Vector2 lookInput;
	private bool lookLeft;
	private bool lookRight;
	private float lean;

	public LayerMask leanMask;

	private float height;
	public float CrouchSpeed = 1.0f;	// Speed crouch and uncrouching occurs
	public float CrouchScale = 0.5f;	// Amount player is squashed when crouching

	public bool doMovement;

//	private CapsuleCollider collider;
	private Transform cameraTransform;
	private CharacterController characterController;
	private CollisionFlags collisionFlags;
	private Vector3 moveDir = Vector3.zero;

	public Vector3 Velocity{
		get { return characterController.velocity; }
	}

	public float MaxSpeed {
		get { return speed * runMultiplier; }
	}

	public bool IsGrounded {
		get { return characterController.isGrounded; }
	}

	public bool JustLanded {
		get { return IsGrounded && !prevGrounded; }
	}

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController>();
//		collider = GetComponent<CapsuleCollider> ();
		height = characterController.height;
		cameraTransform = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {

		GetInput ();

		prevGrounded = characterController.isGrounded;

		Look ();

		if (doMovement) {
			Lean ();
			Crouch ();
			Move ();
		} else {
			Stop ();
		}
		Fall ();

		collisionFlags = characterController.Move(moveDir*Time.deltaTime);
	}

	private void GetInput(){
		moveInput = GlobalScript.GetStick ("Left");
		lookInput = GlobalScript.GetStick ("Right");
		isRunning = GlobalScript.GetButtonHeld ("Run");
		isCrouching = isCrouching ^ GlobalScript.GetButton ("Crouch");
		jump = GlobalScript.GetButton ("Jump");
		lookLeft = GlobalScript.GetButtonHeld ("Camera Left");
		lookRight = GlobalScript.GetButtonHeld ("Camera Right");
		lean = GlobalScript.GetAxis ("Triggers");
	}

	private void Move(){
		if (prevGrounded) {
			moveDir.y = -stickToGroundForce;

			isJumping = false;
			Vector3 desiredMove = transform.forward * moveInput.y + transform.right * moveInput.x;

			// get a normal for the surface that is being touched to move along it
			RaycastHit hitInfo;
			Physics.SphereCast (transform.position, characterController.radius, Vector3.down, out hitInfo,
				characterController.height / 2f, ~0, QueryTriggerInteraction.Ignore);
			desiredMove = Vector3.ProjectOnPlane (desiredMove, hitInfo.normal).normalized;

			float currentSpeed = speed;
			if (isRunning)
				currentSpeed *= runMultiplier;
			if (isCrouching)
				currentSpeed *= crouchMultiplier;
			//			currentSpeed *= 1 + (runMultiplier - 1)/(runMultiplier) * (runMultiplier*runPercent);
			//			currentSpeed *= 1 + (crouchMultiplier - 1)/(crouchMultiplier) * (crouchMultiplier*crouchPercent);

			if (smoothMove) {
				moveDir.x = Mathf.Lerp (moveDir.x, desiredMove.x * currentSpeed, smoothFactor*Time.deltaTime);
				moveDir.z = Mathf.Lerp (moveDir.z, desiredMove.z * currentSpeed, smoothFactor*Time.deltaTime);
			}
			else{
				moveDir.x = desiredMove.x * currentSpeed;
				moveDir.z = desiredMove.z * currentSpeed;
			}

			//jump
			if (jump) {
				moveDir.y = jumpSpeed;
				isJumping = true;
			}
		}
	}

	private void Stop(){
		if (smoothMove) {
			moveDir.x = Mathf.Lerp (moveDir.x, 0, smoothFactor * Time.deltaTime);
			moveDir.z = Mathf.Lerp (moveDir.z, 0, smoothFactor * Time.deltaTime);
		} else {
			moveDir.x = 0;
			moveDir.y = 0;
		}
	}

	private void Fall(){
		if(!prevGrounded)
			moveDir += Physics.gravity*gravityMultiplier*Time.deltaTime;
	}

	private void Look(){
		float yRot = lookInput.x * sensitivityX * (invertX ? -1 : 1);
		float xRot = lookInput.y * sensitivityY * (invertY ? -1 : 1);

		Vector3 characterRot = transform.localRotation.eulerAngles;
		characterRot.y += yRot;// * Time.deltaTime;
		if(smoothLook)
			transform.localRotation = Quaternion.Slerp (transform.localRotation, Quaternion.Euler (characterRot), smoothLookFactor*Time.deltaTime);
		else
			transform.localRotation = Quaternion.Euler (characterRot);

		Vector3 cameraRot = cameraTransform.localRotation.eulerAngles;
		cameraRot.x += xRot;// * Time.deltaTime;
		if (smoothLook)
			cameraTransform.localRotation = Quaternion.Slerp (cameraTransform.localRotation, ClampRotationAroundXAxis (Quaternion.Euler (cameraRot)), smoothLookFactor * Time.deltaTime);
		else
			cameraTransform.localRotation = ClampRotationAroundXAxis (Quaternion.Euler (cameraRot));
	}

	private void Crouch()
	{
		float lastHeight = characterController.height;
		float desiredHeight = isCrouching ? CrouchScale*height : height;

		if (lastHeight != desiredHeight) {
			characterController.height = Mathf.Lerp (lastHeight, desiredHeight, CrouchSpeed * Time.deltaTime);
			transform.position += new Vector3 (0, (characterController.height - lastHeight) * 0.5f + 0.001f, 0);
		}
	}

	private void Lean()
	{
		Vector3 targetPosition = cameraTransform.localPosition;
		float targetX = -1 * lean;
		targetPosition.x = targetX;

		RaycastHit hit = new RaycastHit();
		bool collided = Physics.Raycast (cameraTransform.position, targetX*cameraTransform.right, out hit, Mathf.Abs (targetX), leanMask);
		Debug.DrawRay (cameraTransform.position, targetX*cameraTransform.right, Color.red);
		if (!collided)
			cameraTransform.localPosition = targetPosition;
//			cameraTransform.localPosition = Vector3.Slerp (cameraTransform.localPosition, targetPosition, 10*Time.deltaTime);//new Vector3 (targetPosition, currentPosition.y, currentPosition.z);



		float lookRotate = 0f;
//		if (lookLeft && lookRight)
//			lookRotate = 180f;
//		else if (lookLeft)
//			lookRotate = -90f;
//		else if (lookRight)
//			lookRotate = 90f;

		Vector3 localRotation = cameraTransform.localRotation.eulerAngles;

		float targetAngle = 10 * lean;
		Quaternion targetRotation = Quaternion.Euler (localRotation.x, lookRotate, targetAngle);
//		cameraTransform.localRotation = targetRotation;
		cameraTransform.localRotation = Quaternion.Slerp (cameraTransform.localRotation, targetRotation, 10*Time.deltaTime);
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody body = hit.collider.attachedRigidbody;
		//dont move the rigidbody if the character is on top of it
		if (collisionFlags == CollisionFlags.Below)
		{
			return;
		}

		if (body == null || body.isKinematic)
		{
			return;
		}
		body.AddForceAtPosition(characterController.velocity*0.1f, hit.point, ForceMode.Impulse);
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

		angleX = Mathf.Clamp (angleX, minX, maxX);

		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}
}
