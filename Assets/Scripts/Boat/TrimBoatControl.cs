﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
	This class handles player input and movement on the trim module, which only allows the player to adjust sail trim
*/

public class TrimBoatControl : NavBoatControl {

	[SerializeField]
	Slider sailEffiencySlider;
	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		MastRotation ();
		IdentifyPointOfSail ();
		sailEffiencySlider.value = sailEffectiveness;
	}

	void FixedUpdate () {
		///angleWRTWind gives a value between 0-360 logic was called in NavBoatControl fixed update needed to be re
		SetAngleWRTWind();
		ApplyForwardThrust ();
		SetSailAnimator ();
	}

	void SetAngleWRTWind () {
		angleWRTWind = Vector3.Angle(boatDirection, Vector3.forward);
		if (transform.rotation.eulerAngles.y > 180f ) {
			angleWRTWind = 360-angleWRTWind;
		}

		if (float.IsNaN(angleWRTWind)) {
			angleWRTWind=0;
		}	
	}
}