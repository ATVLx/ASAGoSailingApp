using UnityEngine;
using System.Collections;

public class ApparentWindBoatControl : MonoBehaviour {
	public static ApparentWindBoatControl s_instance;

	public enum BoatPointOfSail {InIrons, PTCloseHaul, PTCloseReach, PTBeamReach, PTBroadReach, Run, STCloseHaul, STCloseReach, STBeamReach, STBroadReach}

	public Transform boatBody;
	private float speedInKnots = 1f;
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
			speedCoefficient = 5.5f;
			break;

		case BoatPointOfSail.PTCloseReach:
		case BoatPointOfSail.STCloseReach:
			speedCoefficient = 6.2f;
			break;

		case BoatPointOfSail.PTBeamReach:
		case BoatPointOfSail.STBeamReach:
			speedCoefficient = 6.8f;
			break;

		case BoatPointOfSail.PTBroadReach:
		case BoatPointOfSail.STBroadReach:
			speedCoefficient = 5.6f;
			break;

		case BoatPointOfSail.Run:
			speedCoefficient = 3.8f;
			break;
		}

		if( !highSpeed )
			speedCoefficient /= boatSpeedScalar;
			
		myRigidbody.velocity = boatBody.forward * (speedInKnots * speedCoefficient/NavBoatControl.METERS_PER_SECOND_TO_KNOTS);
	}

	public void SetHighSpeed( bool val ) {
		highSpeed = val;	
	}
}
