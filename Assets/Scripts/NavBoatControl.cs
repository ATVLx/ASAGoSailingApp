using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NavBoatControl : BoatBase {

	public enum BoatSideFacingWind {Port, Starboard};
	public static NavBoatControl s_instance;

	private Rigidbody myRigidbody;
	private float currThrust = 0f;
	private float weakThrust = 150f, strongThrust = 2500f;
	private float angleToAdjustTo;
	private float turnStrength = 5f;
	private float weakTurnStrength = 4f;
	private float strongTurnStrength = 5f;
	private float turningRate = 60f;
	private float rudderRotation =40f;
	private float deadZone = 45f;

	private float currRutterRotation = 0f;			// in degrees
	private float maxRutterRotation = 170f;			// in degrees
	private float rutterRotationSpeed = 5f;			// in deg/s
 
	private Quaternion comeAboutStart, comeAboutEnd;
	private Quaternion targetRudderRotation = Quaternion.identity;

	public ParticleSystem left, right;
	public bool canMove = false;
	public AudioSource correct;
	public Animator boatKeel;
	public GameObject arrow;
	public Transform rudderR, rudderL;
	public GameObject redNavObj, greenNavObj;
	public Transform red1,red2,green1,green2;

	public Text pointOfSail;

	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
	}

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	void Update () {
		HandleRutterRotation();

		MastRotation();

		//add keeling into the boat rotation
		float animatorBlendVal;

		if (angleWRTWind < 360f && angleWRTWind > 180f) {
			animatorBlendVal = (angleWRTWind-180f)/360f;
		}
		else {
			animatorBlendVal = (angleWRTWind/360f + .5f);
		}

		if ((angleWRTWind < 360f && angleWRTWind > 315f) ||
			(angleWRTWind > 0f && angleWRTWind < 45f)) {
			pointOfSail.text = "In Irons";
		}
		else if ((angleWRTWind < 315f && angleWRTWind > 293f) ||
		    (angleWRTWind > 0f && angleWRTWind < 45f)) {
			pointOfSail.text = "Close-Hauled Starboard Tack";
		}
		else if ((angleWRTWind < 293f && angleWRTWind > 270f) ||
		         (angleWRTWind > 0f && angleWRTWind < 45f)) {
			pointOfSail.text = "Close Reach Starboard Tack";
		}
		else if ((angleWRTWind < 270f && angleWRTWind > 240f) ||
		         (angleWRTWind > 0f && angleWRTWind < 45f)) {
			pointOfSail.text = "Beam Reach Starboard Tack";
		}
		else if ((angleWRTWind < 240f && angleWRTWind > 190f) ||
		         (angleWRTWind > 0f && angleWRTWind < 45f)) {
			pointOfSail.text = "Broad Reach Starboard Tack";
		}
		else if (angleWRTWind < 190f && angleWRTWind > 170f) {
			pointOfSail.text = "Run";
		}
		else if (angleWRTWind > 120f && angleWRTWind < 170f) {
			pointOfSail.text = "Broad Reach Port Tack";
		}
		else if (angleWRTWind > 90f && angleWRTWind < 120f) {
			pointOfSail.text = "Beam Reach Port Tack";
		}
		else if (angleWRTWind > 66f && angleWRTWind < 90f) {
			pointOfSail.text = "Close Reach Port Tack";
		}
		else if (angleWRTWind > 45f && angleWRTWind < 66f){
			pointOfSail.text = "Close-Hauled Port Tack";
		}

		boatKeel.SetFloat("rotation", animatorBlendVal);

		if (Mathf.Abs(Vector3.Angle(WindManager.s_instance.directionOfWind, transform.forward)) < deadZone) {
			if(currThrust > 0) {
				currThrust -= 10f;
			}
			else {
				currThrust = 0;
				left.enableEmission = false;
				right.enableEmission = false;

			}
			turnStrength = weakTurnStrength;

		}
	
		else {
			left.enableEmission = true;
			right.enableEmission = true;
			if (currThrust < strongThrust) {
				currThrust += 10f;
			}
			else {
				currThrust = strongThrust;
			}
			turnStrength = strongTurnStrength;

		}
		print (currThrust + "CT");
		if (NavManager.s_instance.gameState == NavManager.GameState.Win) {
			arrow.SetActive(false);
		}
	}

	void FixedUpdate () {
	
		if (canMove) {
			if(Input.GetKey(KeyCode.LeftArrow)) {
				myRigidbody.AddRelativeTorque (-Vector3.up*turnStrength);
				targetRudderRotation = Quaternion.Euler(0, rudderRotation,0);
			}
			
			else if(Input.GetKey(KeyCode.RightArrow)) {
				myRigidbody.AddRelativeTorque (Vector3.up*turnStrength);
				targetRudderRotation = Quaternion.Euler(0, -rudderRotation,0);

			}
			else {
				targetRudderRotation = Quaternion.identity;
	
			}

//			rudderR.localRotation = Quaternion.RotateTowards(rudderR.localRotation, targetRudderRotation, turningRate * Time.deltaTime);
//			rudderL.localRotation = Quaternion.RotateTowards(rudderL.localRotation, targetRudderRotation, turningRate * Time.deltaTime);

			myRigidbody.AddForce (transform.forward * currThrust);
		}
	}

	void OnTriggerEnter(Collider other) {
//		print (other.tag + " " + NavManager.s_instance.ReturnCurrNavPointName());
		if (other.tag == "NavTarget" && other.name == NavManager.s_instance.ReturnCurrNavPointName() && Vector3.Distance(transform.position, other.transform.position) <100f) {
			NavManager.s_instance.SwitchNavigationPoint();
			correct.Play();
		}

		if (other.tag == "CollisionObject") {
			myRigidbody.AddForce (transform.forward * -1 * currThrust);
		}
	
	}

	void OnTriggerStay(Collider other){
		if (other.tag == "collisionObject") {
			myRigidbody.AddForce(transform.forward * -1 * currThrust);
		}
	}

	private void HandleRutterRotation() {
		float x = Input.GetAxis( "Horizontal" );

		rudderR.localRotation = Quaternion.RotateTowards(rudderR.localRotation, targetRudderRotation, turningRate * Time.deltaTime);
		rudderL.localRotation = Quaternion.RotateTowards(rudderL.localRotation, targetRudderRotation, turningRate * Time.deltaTime);
	}
}
