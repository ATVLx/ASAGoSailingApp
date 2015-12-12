using UnityEngine;
using System.Collections;

public class HoverFollowCam : MonoBehaviour
{
	//Camera that follows the boat during navigation mode

	Transform player, camPos;
	float camDistanceToCamPos;
	float smoothRate = 1f;
	float verticalLookOffset = 5f;
	public enum CameraMode {stationary, follow, lerpToDestination};
	public CameraMode thisCameraMode = CameraMode.stationary;
	public Vector3 panAwayPosition, startPosition;
	float lerpTimer, lerpDuration = 15f;
	//switches
	bool isAdjustingToCamPos;
	bool isPanningOut;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		camPos = GameObject.FindGameObjectWithTag("CamPos").transform;
	}

	
	void Update()
	{
		transform.LookAt(new Vector3(player.position.x, player.position.y+verticalLookOffset, player.position.z));
	}

	void FixedUpdate() {
		switch (thisCameraMode) {
		case CameraMode.follow :
			transform.position -= (transform.position - camPos.position) * smoothRate *Time.deltaTime;
			break;
		case CameraMode.stationary :
			break;
		case CameraMode.lerpToDestination :
			if (isPanningOut) {
				float fraction;
				fraction = (Time.time - lerpTimer) / lerpDuration;
				transform.position = Vector3.Lerp(startPosition, panAwayPosition, fraction);
				if (fraction >= .99f) {
					isPanningOut = false;
				}
			}
			break;
		}
	}


	//call this to pan out and away from boat at end of level
	public void PanOut() {
		lerpTimer = Time.time;
		isPanningOut = true;
		startPosition = transform.position;
		panAwayPosition = new Vector3(transform.position.x + -50f, transform.position.y + 100f, transform.position.z + -50f);
		thisCameraMode = CameraMode.lerpToDestination;
	}
}
