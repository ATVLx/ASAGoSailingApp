using UnityEngine;
using System.Collections;

public class QuaternionLerp : MonoBehaviour {


	//This script is designed for a one time playing camera move, use two transforms two set two positions and quaternions for a camera move transition
	//which is triggered by StartLerp

	public Quaternion lerpStart, lerpEnd;
	public Vector3 lerpPosStart, lerpPosEnd;
	bool isLerping;
	float lerpTimer, lerpDuration;
	void Start () {
		lerpStart = transform.GetChild(0).rotation;
		lerpEnd = transform.GetChild(1).rotation;
		lerpPosStart = transform.GetChild(0).position;
		lerpPosEnd = transform.GetChild(1).position;

	}
	
	// Update is called once per frame
	void Update () {

		if (isLerping) {
			float percentageLerp = (Time.time - lerpTimer)/lerpDuration;
			transform.rotation = Quaternion.Lerp(lerpStart, lerpEnd, percentageLerp);
			transform.position = Vector3.Lerp(lerpPosStart, lerpPosEnd, percentageLerp);
			if (percentageLerp > .9999) {
				transform.rotation = Quaternion.Lerp(lerpStart, lerpEnd, 1);
				isLerping = false;
			}
		}

	}

	public void StartLerp (float duration = 2f) {
		transform.rotation = Quaternion.Lerp(lerpStart, lerpEnd, 0);
		transform.position = Vector3.Lerp(lerpPosStart, lerpPosEnd, 0);
		lerpDuration = duration;
		lerpTimer = Time.time;
		isLerping = true;
	}
}
