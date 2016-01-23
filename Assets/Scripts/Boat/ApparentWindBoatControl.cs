using UnityEngine;
using System.Collections;

public class ApparentWindBoatControl : MonoBehaviour {
	public static ApparentWindBoatControl s_instance;

	public Transform boatBody;
	public float speedInKnots = 4f;
	[System.NonSerialized]
	public Rigidbody myRigidbody;

//	private Transform myTransform;
	private float boatSpeedScalar = 1.5f;
	private bool highSpeed = false;

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
		float speedCoefficient = 1f;
		if( highSpeed )
			speedCoefficient = boatSpeedScalar;
			
		myRigidbody.velocity = boatBody.forward * (speedInKnots/NavBoatControl.METERS_PER_SECOND_TO_KNOTS) * speedCoefficient;
	}

	public void SetHighSpeed( bool val ) {
		highSpeed = val;	
	}
}
