using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cinematographer : MonoBehaviour {

	/*

	Cinematographer takes a list of gameobjects, vector3s, floats
	
	and uses these to move a camera as it transitions between looking at different objects

	it also keeps text or UI panels that are associated with each 


	 */

	Quaternion lerpStart, lerpEnd;
	public List<Transform> quaternions;
	public List<GameObject> textUIObjects;
	public List<float> timeAtEachObject;
	int currentIndex = 0;
	public bool hasStarted;
	float pauseTimer;
	Vector3 lerpPositionStart, lerpPositionEnd;
	//lerp
	float lerpTimer;
	float lerpDuration = 3f;
	bool isLerping;

	public void RollCamera () {
		pauseTimer = Time.time;
		Camera.main.transform.rotation = quaternions[currentIndex].rotation;
		Camera.main.transform.position = quaternions[currentIndex].position;
		textUIObjects[currentIndex].SetActive(true);
		hasStarted = true;

	}
	
	// Update is called once per frame
	void Update () {
		if (hasStarted) {
			if (Time.time - pauseTimer > timeAtEachObject[currentIndex]) {
				if (currentIndex < quaternions.Count - 1) {
					GotoNextPosition();
				}
				else {
					textUIObjects[currentIndex].SetActive(false);
					Camera.main.GetComponent<HoverFollowCam>().enabled = true;
					GameManager.s_instance.hasFinishedCameraPanning = true;
					hasStarted = false;
				}

			}


			if (isLerping) {
				float fraction = (Time.time - lerpTimer	)/ lerpDuration;
				Camera.main.transform.rotation = Quaternion.Lerp(lerpStart, lerpEnd, fraction);
				Camera.main.transform.position = Vector3.Lerp (lerpPositionStart, lerpPositionEnd, fraction * fraction * (3.0f - 2.0f * fraction));
				if (fraction > .9999f) {
					Camera.main.transform.rotation = Quaternion.Lerp(lerpStart, lerpEnd, 1f);
					Camera.main.transform.position = Vector3.Lerp (lerpPositionStart, lerpPositionEnd, 1f);

					isLerping = false;
				}
			}

		}
	}

	void GotoNextPosition () {
		pauseTimer = Time.time;
		lerpTimer = Time.time;
		textUIObjects[currentIndex].SetActive(false);
		currentIndex++;
		textUIObjects[currentIndex].SetActive(true);
		lerpEnd = quaternions[currentIndex].rotation;
		lerpStart = Camera.main.transform.rotation;
		lerpPositionStart = Camera.main.transform.position;
		lerpPositionEnd = quaternions[currentIndex].position;
		isLerping = true;

	}
}
