using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This class handles player input and movement on the trim module, which only allows the player to adjust sail trim.
/// </summary>
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
		sailEffiencySlider.value = sailEffectiveness*1.3f;
	}

	void FixedUpdate () {
		///angleWRTWind gives a value between 0-360 logic was called in NavBoatControl fixed update needed to be re
		SetAngleWRTWind();
		CalculateForwardThrust();
		if( canMove )				//TODO Refactor this so that the base class also uses canMove
			ApplyForwardThrust();
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
