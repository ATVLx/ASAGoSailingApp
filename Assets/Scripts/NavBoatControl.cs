using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NavBoatControl : MonoBehaviour {

	public enum BoatSideFacingWind {Port, Starboard};
	public static NavBoatControl s_instance;

	private Rigidbody myRigidbody;
	private Transform myTransform;
	private float currThrust = 0f;
	private float weakThrust = 150f, strongThrust = 2500f;
	private float angleToAdjustTo;
	private float turnStrength = .04f;
	private float currRudderRotation = 0f;
	private float rudderRotationSpeed = 100f;
	private float maxRudderRotation = 60f;
	private float deadZone = 45f;
	private float boatRotationVelocityScalar = 1f;
	private float boatMovementVelocityScalar = 500f;
 
	private Quaternion comeAboutStart, comeAboutEnd;
	private Quaternion targetRudderRotation = Quaternion.identity;

	public ParticleSystem left, right;
	public bool canMove = false;
	public AudioSource correct;
	public Animator boatKeel;
	public GameObject arrow;
	public Transform boom;
	public Transform rudderR, rudderL;
	/// <summary>
	/// This is the text file that we store in Unity Project folder that holds the values in CSV format of all of the points of sail names and angles associated with them
	/// </summary>
	public Text pointOfSail;
	public Slider boomSlider;
	public Slider rudderSlider;

	//Jibe Logic Vars
	/// <summary>
	/// The angle WRT wind returns the value of 0-360 which is used to determine which side of the wind you are on.
	/// </summary>
	private float angleWRTWind;
	protected float lerpTimer, lerpDuration=1f, blendFloatValue, lastAngleWRTWind;
	public bool rotateMast = false;
	protected bool isJibing = false;
	public GameObject mast;
	protected Quaternion lerpStart, lerpEnd;
	protected Vector3 boatDirection;



	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
		myTransform = GetComponent<Transform>();

		// Subscribe to boom slider update event
		if( boomSlider != null ) {
			//boomSlider.onValueChanged.AddListener( delegate {UpdateBoomAngle();} );
		}
		else
			Debug.LogError( "NavBoatControl doesn't have a reference to the Boom Slider." );
		// Subscribe to rudder slider update event
		if( rudderSlider != null ) {
			//rudderSlider.onValueChanged.AddListener( delegate {UpdateRudderAngle();} );
			rudderSlider.maxValue = maxRudderRotation;
			rudderSlider.minValue = -maxRudderRotation;
		}
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
		ApplyBoomRotation ();
		IdentifyPointOfSail();

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
			ApplyForwardThrust ();
			ApplyBoatRotation ();
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
		float rudderDirectionScalar = 0f;

		if( horizontalInput < 0f ) {
			// If player is pressing left
			rudderDirectionScalar = 1f;
		} else if( horizontalInput > 0f ) {
			// If player is pressing right
			rudderDirectionScalar = -1f;
		}

		rudderSlider.value += rudderRotationSpeed*rudderDirectionScalar*Time.deltaTime;

		rudderL.localRotation = Quaternion.Euler( new Vector3( 0f, rudderSlider.value, 0f ) );
		rudderR.localRotation = Quaternion.Euler( new Vector3( 0f, rudderSlider.value, 0f ) );
	}

	private void ApplyBoomRotation () {
		boom.localRotation = Quaternion.Euler (0, boomSlider.value, 0);
	}

	private void ApplyForwardThrust () {
		float inIronsBufferZone = 15f;
		float inIronsNullZone = 30f;
		float effectiveAngle;

		// If we are within the in irons range check to see if we are in the buffer zone
		if (angleWRTWind < (inIronsNullZone + inIronsBufferZone))
			effectiveAngle = Vector3.Angle( Vector3.forward, transform.forward ) > inIronsNullZone ? angleWRTWind - inIronsNullZone : 0f;
		else {
			effectiveAngle = 15f;
		}
		Debug.Log( "Effective angle: " + effectiveAngle );
		//Debug.Log( "AngleWRTWind: " + angleWRTWind );
		
		float optimalAngle = Vector3.Angle( Vector3.forward, transform.forward ) * 0.33f;		//TODO Fiddle around with the constant to see what works for us
		float sailEffectiveness = Vector3.Angle( Vector3.forward, transform.forward ) > inIronsNullZone ? boomSlider.value / optimalAngle : 0f;
		float boatThrust = (effectiveAngle/inIronsBufferZone) * sailEffectiveness * boatMovementVelocityScalar;
		myRigidbody.AddForce( transform.forward * boatThrust);
	}
	
	private void ApplyBoatRotation() {
		// Depending on forward velocity of the boat, it will rotate faster or slower.
		// We will have a base level rotation speed for when the boat is still.
		// We will always apply torque with a scalar whose value is dependent on velocity
		float velocityScalar;
		if (myRigidbody.velocity.magnitude == 0) {
			velocityScalar = 1;
		} else {
			velocityScalar = 1f + myRigidbody.velocity.magnitude * boatRotationVelocityScalar;
		}
		myRigidbody.AddTorque (-Vector3.up*rudderSlider.value*turnStrength*velocityScalar);
	}

	private void IdentifyPointOfSail() {
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


	//Start Jibe Logic

	protected void MastRotation() {
		//handles sail blend shape, jibes, and mast rotation
		
		
		lastAngleWRTWind = angleWRTWind;

		boatDirection = transform.forward;
		///angleWRTWind gives a value between 0-360
		angleWRTWind = Vector3.Angle(boatDirection, Vector3.forward); 
		if (transform.rotation.eulerAngles.y > 180f ) {
			angleWRTWind = 360-angleWRTWind;
		}

		if (float.IsNaN(angleWRTWind)) {
			angleWRTWind=0;
		}
		if ((angleWRTWind >= 180 && lastAngleWRTWind <= 180 
		     && angleWRTWind <190)) {
			if (lastAngleWRTWind!=0){
				Jibe (-1f);
			}
		}
		if(angleWRTWind <= 180 && lastAngleWRTWind >= 180
		   && angleWRTWind > 170) {
			if (lastAngleWRTWind!=0){
				Jibe (1f);
			}
		}
		
		if (!isJibing) {
			//			get the boats z rotation and as a constant value for the start and end quaternions of the lerp to influence the lerp
			mast.transform.localRotation = Quaternion.Lerp (Quaternion.identity, Quaternion.Inverse(transform.localRotation), 0.5f);
			
		} else if (isJibing) {
			float percentageLerp = (Time.time - lerpTimer)/lerpDuration;
			mast.transform.rotation = Quaternion.Lerp(lerpStart, lerpEnd, percentageLerp);
			if (percentageLerp > .98) {
				mast.transform.rotation = Quaternion.Lerp(lerpStart, lerpEnd, 1);
				isJibing = false;
			}
		}

	}
	
	protected override void Jibe(float negative) {
		print ("JIBE");
		isJibing = true;
		lerpTimer = Time.time;
		lerpStart = Quaternion.Inverse(mast.transform.localRotation);
		lerpEnd = Quaternion.Inverse(mast.transform.localRotation * Quaternion.Inverse(Quaternion.Euler(0,negative*180f,0)));
		
	}


}
