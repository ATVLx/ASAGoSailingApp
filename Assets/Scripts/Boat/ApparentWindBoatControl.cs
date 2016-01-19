using UnityEngine;
using System.Collections;

public class ApparentWindBoatControl : MonoBehaviour {
	public static ApparentWindBoatControl s_instance;

	public Transform boatBody;
	public float speedInKnots = 4f;
	[System.NonSerialized]
	public Rigidbody myRigidbody;

//	private Transform myTransform;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			Debug.LogWarning( gameObject.name + " is being deleted because it is a duplicate " + this.name + ".");
		}
	}

	void Start() {
//		myTransform = transform;
		myRigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate() {
		myRigidbody.velocity = boatBody.forward * (speedInKnots/NavBoatControl.METERS_PER_SECOND_TO_KNOTS);
	}
}
