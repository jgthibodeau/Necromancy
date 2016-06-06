using UnityEngine;
using System.Collections;

namespace ThirdPersonCamera
{
	[RequireComponent(typeof(CameraController)), RequireComponent(typeof(Follow)), RequireComponent(typeof(FreeForm))]
	public class PlayerCamera : MonoBehaviour
	{
		public float timeToActivate = 1.0f;
		public float timeToDeactivate = 1.0f;
		public float motionThreshold = 0.05f;
		public float maxTowardCameraAngle = 10f;

		private CameraController cameraController;
		private FreeForm freeForm;
		private Follow follow;

		private bool moving;
		private float moveStartTime;
		private float moveStopTime;
		private bool followDisabled;
		private Vector3 prevPosition;

		// Use this for initialization
		void Start()
		{
			cameraController = GetComponent<CameraController>();
			follow = GetComponent<Follow>();
			freeForm = GetComponent<FreeForm>();
			followDisabled = !follow.follow;
		}

		// Update is called once per frame
		void Update()
		{
			//if player is manually setting the camera
			if (freeForm.x != 0 || freeForm.y != 0)
			{
				follow.follow = false;
				followDisabled = true;
			}

			//if player is not manually setting the camera
			else
			{

				//check if moving
				Vector3 motionVector = cameraController.target.transform.position - prevPosition;
				if (motionVector.magnitude > motionThreshold) {
					if (!moving)
						moveStartTime = Time.realtimeSinceStartup;
					moving = true;
				} else {
					if (moving)
						moveStopTime = Time.realtimeSinceStartup;
					moving = false;
				}

				//if moving for long enough
				if (moving && (Time.realtimeSinceStartup - moveStartTime) > timeToActivate) {
					Vector3 targetAngle = cameraController.target.transform.forward;
					Vector3 cameraAngle = cameraController.transform.forward;
					cameraAngle.y = targetAngle.y;
					float angleBetween = Vector3.Angle (targetAngle, cameraAngle);

					//moving towards camera, stop following
					if(angleBetween > (180 - maxTowardCameraAngle)){
						follow.follow = false;
						followDisabled = true;
					}

					//moving away from camera, start following
					else{
						follow.follow = true;
						followDisabled = false;
					}
				}

				//if stopped for long enough
				if(!moving && (Time.realtimeSinceStartup - moveStopTime) > timeToDeactivate){
					//stop following
					follow.follow = false;
					followDisabled = true;
				}
			}

			//if player is no longer moving, stop following after some time

			prevPosition = cameraController.target.transform.position;
		}

		public void ActivateFollow()
		{
			if (freeForm.x == 0 && freeForm.y == 0)
			{
				follow.follow = true;
				followDisabled = false;
			}
			else
			{
				Invoke("ActivateFollow", timeToActivate);
			}
		}
	}
}

