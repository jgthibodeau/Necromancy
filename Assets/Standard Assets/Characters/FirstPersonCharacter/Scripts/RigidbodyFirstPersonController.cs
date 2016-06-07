using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
			public float RunMultiplier = 2.0f;   // Speed when sprinting
			public float CrouchMultiplier = 0.5f;   // Speed when crouching
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;
			public bool m_Running;
			public bool m_Crouching;

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = ForwardSpeed;
				}

				if (m_Running)
				{
					CurrentTargetSpeed *= RunMultiplier;
				}

				if (m_Crouching)
				{
					CurrentTargetSpeed *= CrouchMultiplier;
				}
            }

            public bool Running
            {
                get { return m_Running; }
			}

			public bool Crouching
			{
				get { return m_Crouching; }
			}
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }

		private float height;
		public float CrouchSpeed = 1.0f;	// Speed crouch and uncrouching occurs
		public float CrouchScale = 0.5f;	// Amount player is squashed when crouching

        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
		public ControllerLook controllerLook = new ControllerLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
		public bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;

		private float m_Lean;

		[HideInInspector] public bool doMovement = true;

		private Vector2 movementInput;

        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            { return movementSettings.Running; }
		}

		public bool Crouching
		{
			get
			{ return movementSettings.Crouching; }
		}


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
			height = m_Capsule.height;
			mouseLook.Init (transform, cam.transform);
			controllerLook.Init (transform, cam.transform);
        }


        private void Update()
        {
            RotateView();

			if (doMovement)
				GetInput ();
        }


        private void FixedUpdate()
        {
            GroundCheck();

			if (doMovement) {

				if ((Mathf.Abs (movementInput.x) > 0.25f || Mathf.Abs (movementInput.y) > 0.25f) && (advancedSettings.airControl || m_IsGrounded)) {
					// always move along the camera forward as it is the direction that it being aimed at
					Vector3 desiredMove = cam.transform.forward * movementInput.y + cam.transform.right * movementInput.x;
					desiredMove = Vector3.ProjectOnPlane (desiredMove, m_GroundContactNormal).normalized;

					desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
					desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
					desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
					if (m_RigidBody.velocity.sqrMagnitude <
					                (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed)) {
						m_RigidBody.AddForce (desiredMove * SlopeMultiplier (), ForceMode.Impulse);
					}
				}

			}

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
					m_Jump = false;
                    m_Jumping = true;
                }

				if (!m_Jumping && (!doMovement || (Mathf.Abs(movementInput.x) < float.Epsilon && Mathf.Abs(movementInput.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)))
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;

			Crouch ();
        }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private void GetInput()
        {
            
            movementInput = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal Left"),
                    y = CrossPlatformInputManager.GetAxis("Vertical Left")
                };
			movementSettings.UpdateDesiredTargetSpeed(movementInput);

			m_Lean = CrossPlatformInputManager.GetAxis ("Triggers");

			if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jumping)
				m_Jump = true;

			if (CrossPlatformInputManager.GetButton("Run"))
				movementSettings.m_Running = true;
			else
				movementSettings.m_Running = false;

			if (CrossPlatformInputManager.GetButtonDown ("Crouch")) {
				movementSettings.m_Crouching = !movementSettings.Crouching;
			}
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation (transform, cam.transform);
			controllerLook.LookRotation (transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }

			//rotate view about forward axis dependent on lean
			Lean ();
        }

		private void Lean()
		{
			Vector3 currentAngle = cam.transform.rotation.eulerAngles;
			float targetAngle = 10 * m_Lean;
			cam.transform.rotation = Quaternion.Euler (currentAngle.x, currentAngle.y, targetAngle);

			Vector3 currentPosition = cam.transform.localPosition;
			float targetPosition = -1 * m_Lean;
			cam.transform.localPosition = new Vector3 (targetPosition, currentPosition.y, currentPosition.z);
		}

		private void Crouch()
		{
			float lastHeight = m_Capsule.height;
			float desiredHeight = Crouching ? CrouchScale*height : height;

			m_Capsule.height = Mathf.Lerp (lastHeight, desiredHeight, CrouchSpeed*Time.deltaTime);

			transform.position += new Vector3(0, (m_Capsule.height - lastHeight)*0.5f+0.001f, 0);
		}

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}
