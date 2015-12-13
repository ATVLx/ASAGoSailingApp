using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NavBoatControl : MonoBehaviour {

	//test
	public Text thrustVal, velocity;
	public GameObject boatModel;
	public static NavBoatControl s_instance;

	public Animator sail;
	protected Rigidbody myRigidbody;
	private const float METERS_PER_SECOND_TO_KNOTS = 1.94384f;
	private float currThrust = 0f;
	private float angleToAdjustTo;
	private float turnStrength = 50f;
	/// <summary>
	/// The rudder rotation speed in degrees/sec.
	/// </summary>
	private float rudderRotationSpeed = 50f;
	/// <summary>
	/// The boom trim speed in degrees/sec.
	/// </summary>
	private float boomTrimSpeed = 30f;
	private float maxRudderRotation = 60f;
	protected float sailEffectiveness;
	private float rudderNullZone = 0.2f;
	private float boatRotationVelocityScalar = .07f;
	private float boatMovementVelocityScalar = 12000f;
	private float keelCoefficient = 10f;
	private float velocityKeelCoefficient = 7f; //assumes max speed of 7
	private Quaternion comeAboutStart, comeAboutEnd;

	public bool canMove = false;
	public bool controlsAreActive = true;
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
	protected float lerpTimer, lerpDuration=.5f, blendFloatValue, lastAngleWRTWind;
	public bool rotateMast = false;
	protected bool isJibing = false;
	protected float lerpStart, lerpEnd;
	protected Vector3 boatDirection;


	void Start () {
		myRigidbody = GetComponent<Rigidbody>();

		// Subscribe to boom slider update event
		if( boomSlider != null ){
		}
		else
			Debug.LogError( "NavBoatControl doesn't have a reference to the Boom Slider." );
		// Subscribe to rudder slider update event
		if( rudderSlider != null ) {
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
		velocity.text = "Knots: " + Mathf.Round(myRigidbody.velocity.magnitude*METERS_PER_SECOND_TO_KNOTS);

		MastRotation();		
		HandleRudderRotation();
		IdentifyPointOfSail();

	}

	void FixedUpdate () {	
			ApplyForwardThrust ();
			ApplyBoatRotation ();
	}

	void OnTriggerEnter(Collider other) {
//		print (other.tag + " " + NavManager.s_instance.ReturnCurrNavPointName());
		if (other.tag == "NavTarget" && other.name == GameManager.s_instance.ReturnCurrNavPointName() && Vector3.Distance(transform.position, other.transform.position) <100f) {
			GameManager.s_instance.SwitchNavigationPoint();
		}

		if (other.tag == "CollisionObject") {
			myRigidbody.AddForce (transform.forward * -1 * currThrust);
			canMove = false;
			BoatHasCrashed();
		}
	}

	void OnTriggerStay(Collider other){
		if (other.tag == "collisionObject") {
			myRigidbody.AddForce(transform.forward * -1 * currThrust);
		}
	}

	private void HandleRudderRotation() {
		float horizontalInput = Input.GetAxis( "Horizontal" );
		float rudderDirectionScalar = 0f;

		if( controlsAreActive ) {
			if( horizontalInput < 0f ) {
				// If player is pressing left
				rudderDirectionScalar = 1f;
			} else if( horizontalInput > 0f ) {
				// If player is pressing right
				rudderDirectionScalar = -1f;
			}
		}

		rudderSlider.value += rudderRotationSpeed*rudderDirectionScalar*Time.deltaTime;

		rudderL.localRotation = Quaternion.Euler( new Vector3( 0f, rudderSlider.value, 0f ) );
		rudderR.localRotation = Quaternion.Euler( new Vector3( 0f, rudderSlider.value, 0f ) );
	}

	protected void ApplyForwardThrust () {
		float inIronsBufferZone = 15f;
		float inIronsNullZone = 30f;
		float effectiveAngle;

		// If we are within the in irons range check to see if we are in the buffer zone
		if (angleWRTWind < (inIronsNullZone + inIronsBufferZone))
			effectiveAngle = Vector3.Angle( Vector3.forward, transform.forward ) > inIronsNullZone ? angleWRTWind - inIronsNullZone : 0f;
		else {
			effectiveAngle = 15f;
		}
		//Debug.Log( "Effective angle: " + effectiveAngle );
		//Debug.Log( "AngleWRTWind: " + angleWRTWind );
		
		float optimalAngle = Vector3.Angle( Vector3.forward, transform.forward ) * 0.33f; //TODO Fiddle around with the constant to see what works for us
		if (boomSlider != null) {
			sailEffectiveness = Vector3.Angle (Vector3.forward, transform.forward) > inIronsNullZone ? optimalAngle / (Mathf.Abs (boomSlider.value - optimalAngle) + optimalAngle) : 0f;
		} else {
			if (angleWRTWind > 180f) {
				angleWRTWind -= 180f;
			}
			sailEffectiveness = Vector3.Angle (Vector3.forward, transform.forward) > inIronsNullZone ? optimalAngle / (Mathf.Abs (angleWRTWind - optimalAngle) + optimalAngle) : 0f;


		}
		sailEffectiveness = Mathf.Pow(sailEffectiveness,3f);
		float boatThrust = (effectiveAngle/inIronsBufferZone) * sailEffectiveness * boatMovementVelocityScalar;
		myRigidbody.AddForce( transform.forward * boatThrust);
//		thrustVal.text = "boat Thrust: " + Mathf.Round(boatThrust*100);

		//sail animator
		float isNegative = -1f;//which side of the wind are we on -1 is 0-180 1 is 180-360
		float angle = angleWRTWind; //angle is an acute angle rather than 0-360
		if (angleWRTWind > 180f) {
			angle = 360f - angleWRTWind;
			isNegative = 1f;
		}
		if (sailEffectiveness > .85f) {
			blendFloatValue = 1f;
		} else if (sailEffectiveness < -.85f) {
			blendFloatValue = -1f;
		} else {
			blendFloatValue = sailEffectiveness;
		}
		sail.SetFloat ("sailtrim", blendFloatValue*isNegative*-1);// -1 bc jon setup animator backwards

		float zAxisRotation = 0f; //what we use to set the keel value

		//handle keeling
		if (angle < 45f && angle > 30f) {
			zAxisRotation = (((3*(angle-30f))) / 45f) * sailEffectiveness;
		} 
		else if (angle > 45f && angle < 90f) {
			zAxisRotation = (((45f-angle)+45f)/45) * sailEffectiveness;
		}
		Vector3 newRotation = transform.rotation.eulerAngles;
		newRotation = new Vector3 (newRotation.x, newRotation.y, zAxisRotation*keelCoefficient*isNegative*myRigidbody.velocity.magnitude/velocityKeelCoefficient);
		boatModel.transform.rotation = Quaternion.Euler (newRotation); 
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
		if (Mathf.Abs (rudderSlider.value) > rudderNullZone * maxRudderRotation) {
			myRigidbody.AddTorque (-Vector3.up * rudderSlider.value * turnStrength * velocityScalar);
		} else {
//			myRigidbody.angularVelocity = Vector3.zero;
		}
	}

	protected void IdentifyPointOfSail() {
		//add keeling into the boat rotation


		
		if ((angleWRTWind < 360f && angleWRTWind > 315f) ||
		    (angleWRTWind > 0f && angleWRTWind < 30f)) {
			pointOfSail.text = "In Irons";
		}
		else if ((angleWRTWind < 330f && angleWRTWind > 293f) ||
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
		else if (angleWRTWind > 30f && angleWRTWind < 66f){
			pointOfSail.text = "Close-Hauled Port Tack";
		}
		
	}

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
			if (lastAngleWRTWind!=0 && !isJibing){
				Jibe (-1f);
			}
		}
		if(angleWRTWind <= 180 && lastAngleWRTWind >= 180
		   && angleWRTWind > 170) {
			if (lastAngleWRTWind!=0 && !isJibing){
				Jibe (1f);
			}
		}
		
		if (!isJibing) {
			// get the boats z rotation and as a constant value for the start and end quaternions of the lerp to influence the lerp
			SetBoomRotation();
			
		} else if (isJibing) {
			float fracJourney = (Time.time - lerpTimer)/lerpDuration;
			float lerpAngleFloatVal = Mathf.Lerp(lerpStart, lerpEnd, fracJourney);
			boom.localRotation = Quaternion.Euler(0,lerpAngleFloatVal,0);
			if (fracJourney > .99f) {
				isJibing = false;
				fracJourney=1;
				lerpAngleFloatVal = Mathf.Lerp(lerpStart, lerpEnd, fracJourney);
				boom.localRotation = Quaternion.Euler(0,lerpAngleFloatVal,0);
				rudderSlider.interactable = true;
				boomSlider.interactable = true;
				controlsAreActive = true;
			}
		}

	}
	
	protected virtual void Jibe(float negative) {
		isJibing = true;
		lerpTimer = Time.time;
		lerpStart = boom.localRotation.eulerAngles.y > 180 ? boom.localRotation.eulerAngles.y - 360 : boom.localRotation.eulerAngles.y;
		lerpEnd = -lerpStart;
		rudderSlider.interactable = false;
		boomSlider.interactable = false;
		controlsAreActive = false;
	}

	private void SetBoomRotation() {
		if( controlsAreActive ) {
			float input = Input.GetAxis( "Vertical" );
			// If player is pressing down
			if( input < 0f ) {
				boomSlider.value += boomTrimSpeed * Time.deltaTime;
			}
			// If player is pressing up
			if( input > 0f ) {			
				boomSlider.value -= boomTrimSpeed * Time.deltaTime;
			}
		}

		float maxBoomAngle = boomSlider.maxValue;
		// If we're less than 90 degrees from in irons clamp boom's max angle
		if( Vector3.Angle( Vector3.forward, transform.forward) <= 90f )
			maxBoomAngle = Vector3.Angle( Vector3.forward, transform.forward );

		float clampedBoomAngle = Mathf.Clamp( boomSlider.value, 0f, maxBoomAngle );
		// Mirror canvas's position dependingon what way we are facing the wind.
		Vector3 newBoomDirection = boom.localRotation * Vector3.forward;    
		if( angleWRTWind >= 180f ) {
			newBoomDirection = Vector3.RotateTowards(newBoomDirection, Quaternion.Euler( 0f, clampedBoomAngle, 0f )*Vector3.forward,0.1f, 0.1f);
		} else {
			newBoomDirection = Vector3.RotateTowards(newBoomDirection, Quaternion.Euler( 0f, -clampedBoomAngle, 0f )*Vector3.forward,0.1f, 0.1f);
		}
		boom.localRotation = Quaternion.LookRotation (newBoomDirection);
	}

	private void BoatHasCrashed() {

	}
}
