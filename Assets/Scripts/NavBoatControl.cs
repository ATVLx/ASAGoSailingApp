using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NavBoatControl : BoatBase {

	public enum BoatSideFacingWind {Port, Starboard};
	public static NavBoatControl s_instance;

	private Rigidbody myRigidbody;
	private Transform myTransform;
	private float currThrust = 0f;
	private float weakThrust = 150f, strongThrust = 2500f;
	private float angleToAdjustTo;
	private float turnStrength = 5f;
	private float weakTurnStrength = 4f;
	private float strongTurnStrength = 5f;
	private float turningRate = 60f;
	private float maxRudderRotation =40f;
	private float deadZone = 45f;

	private float currBoomRotation = 0f;
	private float currBoomValue = 0f;
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
	public Slider boomSlider;
	public Slider rudderSlider;

	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
		myTransform = GetComponent<Transform>();

		// Subscribe to boom slider update event
		if( boomSlider != null )
			boomSlider.onValueChanged.AddListener( delegate {UpdateBoomAngle();} );
		else
			Debug.LogError( "NavBoatControl doesn't have a reference to the Boom Slider." );
		// Subscribe to rudder slider update event
		if( rudderSlider != null )
			rudderSlider.onValueChanged.AddListener( delegate {UpdateRudderAngle();} );
		else
			Debug.LogError( "NavBoatControl doesn't have a reference to the Rudder Slider." );
	}

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	void Update () {
		MastRotation();

		ApplyBoatAngle();

		float inIronsBufferZone = 15f;
		float inIronsNullZone = 30f;
		float effectiveAngle = 15f;
		float angleWithRespectToWind = Mathf.Abs(Vector3.Angle(WindManager.s_instance.directionOfWind, transform.forward));

		// If we are within the in irons range check to see if we are in the buffer zone
		if ( angleWithRespectToWind < (inIronsNullZone+inIronsBufferZone) )
			effectiveAngle = angleWithRespectToWind > inIronsNullZone ? angleWithRespectToWind - inIronsNullZone : 0f;
	
		float optimalAngle = myTransform.rotation.y * 0.45f;								//TODO Fiddle around with the constant to see what works for us
		float sailEffectiveness = optimalAngle != 0f ? currBoomRotation / optimalAngle : 0f;

		float boatThrust = (effectiveAngle/inIronsBufferZone) * sailEffectiveness * 10f; 	//TODO Fiddle with this constant for speed of boat
		currThrust = boatThrust;
//		if (Mathf.Abs(Vector3.Angle(WindManager.s_instance.directionOfWind, transform.forward)) < deadZone) {
//			if(currThrust > 0) {
//				currThrust -= 10f;
//			} else {
//				currThrust = 0;
//				left.enableEmission = false;
//				right.enableEmission = false;
//			}
//			turnStrength = weakTurnStrength;
//		} else {
//			left.enableEmission = true;
//			right.enableEmission = true;
//			if (currThrust < strongThrust) {
//				currThrust += 10f;
//			} else {
//				currThrust = strongThrust;
//			}
//			turnStrength = strongTurnStrength;
//		}
//
//		print (currThrust + "CT");

		if (NavManager.s_instance.gameState == NavManager.GameState.Win) {
			arrow.SetActive(false);
		}
	}

	void FixedUpdate () {	
		if (canMove) {
			HandleRutterRotation();

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
		float horizontalInput = Input.GetAxis( "Horizontal" );
		
		if( horizontalInput < -0.1f ) {
			// If player is pressing left
			myRigidbody.AddRelativeTorque (-Vector3.up*turnStrength);
			targetRudderRotation = Quaternion.Euler(0, maxRudderRotation,0);
			rudderSlider.value = -1f;
		} else if( horizontalInput > 0.1f ) {
			// If player is pressing right
			myRigidbody.AddRelativeTorque (Vector3.up*turnStrength);
			targetRudderRotation = Quaternion.Euler(0, -maxRudderRotation,0);
			rudderSlider.value = 1f;
		} else {
			targetRudderRotation = Quaternion.identity;
			rudderSlider.value = 0f;
		}
		
		rudderR.localRotation = Quaternion.RotateTowards(rudderR.localRotation, targetRudderRotation, turningRate * Time.deltaTime);
		rudderL.localRotation = Quaternion.RotateTowards(rudderL.localRotation, targetRudderRotation, turningRate * Time.deltaTime);

	}

	private void ApplyBoatAngle() {
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
	}
	
	public void UpdateBoomAngle() {
		currBoomValue = boomSlider.value;
	}

	public void UpdateRudderAngle() {
		targetRudderRotation = Quaternion.Euler( new Vector3( 0f, rudderSlider.value*maxRudderRotation, 0f ) );
	}
}
