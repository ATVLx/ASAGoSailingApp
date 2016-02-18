using UnityEngine;
using System.Collections;

public class ApparentWindBoatControl : MonoBehaviour {
	public static ApparentWindBoatControl s_instance;

	public enum BoatPointOfSail {InIrons, PTCloseHaul, PTCloseReach, PTBeamReach, PTBroadReach, Run, STCloseHaul, STCloseReach, STBeamReach, STBroadReach}

	public Transform boatBody;
	public float speedInKnots = 4f;
	[System.NonSerialized]
	public Rigidbody myRigidbody;
	[System.NonSerialized]
	public BoatPointOfSail currentPOS;

	private float boatSpeedScalar = 1.5f;
	private bool highSpeed = false;
	private ApparentWindAnimHandler animHandler;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			Debug.LogWarning( gameObject.name + " is being deleted because it is a duplicate " + this.name + ".");
		}
	}

	void Start() {
		myRigidbody = GetComponent<Rigidbody>();
		animHandler = GameObject.FindObjectOfType<ApparentWindAnimHandler>();

		currentPOS = BoatPointOfSail.InIrons;
	}

	void FixedUpdate() {
		float speedCoefficient = 1f;
		switch( currentPOS ) {
		case BoatPointOfSail.InIrons:
			speedCoefficient = 0.01f;
			break;

		case BoatPointOfSail.PTCloseHaul:
		case BoatPointOfSail.STCloseHaul:
			speedCoefficient = 0.9f;
			break;

		case BoatPointOfSail.PTCloseReach:
		case BoatPointOfSail.STCloseReach:
			speedCoefficient = 1.2f;
			break;

		case BoatPointOfSail.PTBeamReach:
		case BoatPointOfSail.STBeamReach:
			speedCoefficient = 0.8f;
			break;

		case BoatPointOfSail.PTBroadReach:
		case BoatPointOfSail.STBroadReach:
			speedCoefficient = 1.1f;
			break;

		case BoatPointOfSail.Run:
			break;
		}

		if( highSpeed )
			speedCoefficient *= boatSpeedScalar;
			
		myRigidbody.velocity = boatBody.forward * (speedInKnots/NavBoatControl.METERS_PER_SECOND_TO_KNOTS) * speedCoefficient;
	}

	public void SetHighSpeed( bool val ) {
		highSpeed = val;	
	}
}
