using UnityEngine;
using System.Collections;

public class GhostBoatAnimator : MonoBehaviour {


	public Animator blendShape;
	protected float lerpTimer, lerpDuration=1f, blendFloatValue, angleWRTWind, lastAngleWRTWind;
	protected bool isJibing = false;
	public GameObject mast;
	protected float lerpStart, lerpEnd;
	protected Vector3 directionWindComingFrom = new Vector3(0f,0f,1f);
	protected Vector3 boatDirection;
	float sailEffectiveness, optimalAngle;
	void Update() {
		MastRotation ();
		SetSailAnimator ();
	}

	protected void MastRotation() {
		//handles sail blend shape, jibes, and mast rotation

		lastAngleWRTWind = angleWRTWind;
		directionWindComingFrom = WindManager.s_instance.directionOfWind;

		boatDirection = transform.forward;
		angleWRTWind = Vector3.Angle(boatDirection,directionWindComingFrom);
		if (transform.rotation.eulerAngles.y > 180f ) {
			angleWRTWind = 360-angleWRTWind;
		}

		if (float.IsNaN(angleWRTWind)) {
			angleWRTWind=0;
		}
		if ((angleWRTWind >= 180 && lastAngleWRTWind <= 180 
			&& angleWRTWind <190)) {
			if (lastAngleWRTWind!=0){
				Jibe (-1f);
			}
		}
		if(angleWRTWind <= 180 && lastAngleWRTWind >= 180
			&& angleWRTWind > 170) {
			if (lastAngleWRTWind!=0){
				Jibe (1f);
			}
		}

		if (!isJibing) {

			//			get the boats z rotation and as a constant value for the start and end quaternions of the lerp to influence the lerp
			mast.transform.localRotation = Quaternion.Lerp (Quaternion.identity, Quaternion.Inverse(transform.localRotation), 0.33f);

		}

		else if (isJibing) {
			float fracJourney = (Time.time - lerpTimer)/lerpDuration;
			float lerpAngleFloatVal = Mathf.Lerp(lerpStart, lerpEnd, fracJourney);
			mast.transform.localRotation = Quaternion.Euler(0,lerpAngleFloatVal,0);
			if (fracJourney > .99f) {
				isJibing = false;
				fracJourney=1;
				lerpAngleFloatVal = Mathf.Lerp(lerpStart, lerpEnd, fracJourney);
				mast.transform.localRotation = Quaternion.Euler(0,lerpAngleFloatVal,0);
			}
		}
	}

	void SetSailAnimator () {
		sailEffectiveness = .6f;
		//sail animator handles luffing etc...
		float isNegative = -1f;//which side of the wind are we on -1 is 0-180 1 is 180-360
		float angle = angleWRTWind; //angle is an acute angle rather than 0-360
		if (angleWRTWind > 180f) {
			angle = 360f - angleWRTWind;
			isNegative = 1f;
		}
		if (sailEffectiveness > .85f) {
			blendFloatValue = 1f;
		} else if (sailEffectiveness < -.85f) {
			blendFloatValue = -1f;
		} else {
			blendFloatValue = sailEffectiveness;
		}

		if (angle > 5f && angle <= 165f) {
			blendFloatValue = isNegative *angle/5f;
			print ("is Negative set to " + isNegative);
			blendShape.SetFloat ("sailtrim", isNegative*-1);// -1 bc jon setup animator backwards
		}
		else if (angle < 5f) {
			isNegative = isNegative *(angle/5f);
			blendShape.SetFloat ("sailtrim", isNegative*-1);// -1 bc jon setup animator backwards
		}
		else if (angle > 165f){
			isNegative = isNegative *((180-angle)/15f);
			blendShape.SetFloat ("sailtrim", isNegative*-1);// -1 bc jon setup animator backwards
		}
		else {
			blendShape.SetFloat ("sailtrim", blendFloatValue*isNegative*-1);// -1 bc jon setup animator backwards
		}

	}

	protected void Jibe(float negative) {
		isJibing = true;
		lerpTimer = Time.time;
		lerpStart = mast.transform.localRotation.eulerAngles.y > 180 ? mast.transform.localRotation.eulerAngles.y - 360 : mast.transform.localRotation.eulerAngles.y;
		lerpEnd = -lerpStart;

	}
}
