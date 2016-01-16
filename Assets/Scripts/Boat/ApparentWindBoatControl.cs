using UnityEngine;
using System.Collections;

public class ApparentWindBoatControl : MonoBehaviour {
 	
	public Transform boatBody;
	public float speedInKnots = 4f;

	private Transform myTransform;
	private Rigidbody myRigidbody;

	void Start() {
		myTransform = transform;
		myRigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate() {
		myRigidbody.velocity = boatBody.forward * (speedInKnots/NavBoatControl.METERS_PER_SECOND_TO_KNOTS);
	}
}
