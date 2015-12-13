using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrimBoatControl : NavBoatControl {

	[SerializeField]
	Slider sailEffiencySlider;
	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		sailEffiencySlider.value = sailEffectiveness;
		print (sailEffectiveness);
	}

	void FixedUpdate () {	
		ApplyForwardThrust ();
	}
}
