using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This class controls the movement input and physics of the boat in the navigation module.
/// </summary>
public class NavBoatControl : MonoBehaviour {
	public const float METERS_PER_SECOND_TO_KNOTS = 1.94384f;

	//test
	public Text thrustVal, velocity;
	/// <summary>
	/// The boat model referring to a child transform with all non-physics components
	/// </summary>
	public GameObject boatModel;
	public static NavBoatControl s_instance;

	public Animator sail;
	protected Rigidbody myRigidbody;
	private float currThrust = 0f;
	private float sinkMultiplier;
	private float angleToAdjustTo;
	private float turnStrength = 75f;
	/// <summary>
	/// The rudder rotation speed in degrees/sec.
	/// </summary>
	private float rudderRotationSpeed = 50f;
	private float maxRudderRotation = 60f;
	protected float sailEffectiveness, optimalAngle;
	private float rudderNullZone = 0.1f;
	private float boatRotationVelocityScalar = .07f;
	private float boatMovementVelocityScalar = 18000f;
	private float keelCoefficient = 10f;
	private float velocityKeelCoefficient = 7f; //assumes max speed of 7
	private Quaternion comeAboutStart, comeAboutEnd;
	private bool isCrashing;
	public bool canMove = false;
	public bool controlsAreActive = true;
	public GameObject arrow;
	public Transform boom;
	public Transform rudderR, rudderL;
	public Transform respawnTransform;
	/// <summary>
	/// This is the text file that we store in Unity Project folder that holds the values in CSV format of all of the points of sail names and angles associated with them
	/// </summary>
	public Text pointOfSail;
	[Header("UI")]
	public Slider boomSlider;
	public Slider rudderSlider;

	//Jibe Logic Vars
	/// <summary>
	/// The angle WRT wind returns the value of 0-360 which is used to determine which side of the wind you are on.
	/// </summary>
	protected float angleWRTWind;
	protected float lerpTimer, lerpDuration=.5f, blendFloatValue, lastAngleWRTWind;
	public bool rotateMast = false;
	protected bool isJibing = false;
	protected float lerpStart, lerpEnd;
	protected Vector3 boatDirection;

	private float boatThrust = 0f;

	// Boat rudders reset
	private float rudderResetTimeBuffer = .1f;
	private float rudderResetTimer = 0f;
	private float rudderLerpSpeed = 1115f;
	private float rudderStartVal = 0f;
	private bool rudderIsLerping = false;
	/// <summary>
	/// The rudder slider selected. This requires the UI slider to have an OnPointerDown and OnPointerUp event to set this.
	/// </summary>
	private bool rudderSliderSelected = true;


	void Start () {
		myRigidbody = GetComponent<Rigidbody>();
		Navigation newNav = new Navigation ();
		newNav.mode = Navigation.Mode.None;
		boomSlider.navigation = newNav;
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
		//velocity.text = "Knots: "+ Mathf.Round(myRigidbody.velocity.magnitude*METERS_PER_SECOND_TO_KNOTS);
//		velocity.text = "" + Mathf.Round(myRigidbody.velocity.magnitude*METERS_PER_SECOND_TO_KNOTS);
		HandleRudderRotation();
		IdentifyPointOfSail();
	}

	public float ReturnSailEfficiency() {
		return sailEffectiveness;
	}

	void FixedUpdate () {	
		MastRotation();
		CalculateForwardThrust();
		ApplyForwardThrust ();
		ApplyBoatRotation ();
		SetSailAnimator ();
//		HandleWindArrowMovement ();

	}

	void OnTriggerEnter(Collider other) {
//		print (other.tag + " " + NavManager.s_instance.ReturnCurrNavPointName());
		if (other.tag == "NavTarget" && other.name == NavManager.s_instance.ReturnCurrNavPointName() && Vector3.Distance(transform.position, other.transform.position) <100f) {
			NavManager.s_instance.SwitchNavigationPoint();
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

	/// <summary>
	/// Sets the rudderSliderSelected bool true or false. This methos is called externally by the OnPointerDown() and OnPointerUp() event triggers on the rudderSlider
	/// </summary>
	/// <param name="selectionState">If set to <c>true</c> it indicates that the rudderSlider was selected.</param>
	public void RudderSliderValueWasChangedTrue( ) {
		rudderSliderSelected = true;
	}

	public void RudderSliderValueWasChangedFalse( ) {
		rudderSliderSelected = false;
	}

	private void HandleRudderRotation() {
		float horizontalInput = Input.GetAxis( "Horizontal" );
		if( rudderSliderSelected == false && horizontalInput == 0f && rudderSlider.value != 0f ) {
			if( rudderResetTimer >= rudderResetTimeBuffer && !rudderIsLerping ) {
				rudderIsLerping = true;
				rudderStartVal = rudderSlider.value;
			} else {
				rudderResetTimer += Time.deltaTime;
			}

			if( rudderIsLerping ) {
				float t =  1f -(( Mathf.Abs(rudderSlider.value) - ( rudderLerpSpeed*Time.deltaTime ) ) / Mathf.Abs(rudderStartVal));
				if( t >= 0.95f ) {
					rudderIsLerping = false;
					rudderSlider.value = Mathf.Lerp( rudderStartVal, 0f, 1f );
					return;
				}
				rudderSlider.value = Mathf.Lerp( rudderStartVal, 0f, t );
			}				
		} else {
			rudderIsLerping = false;
			rudderResetTimer = 0f;
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
		}
		rudderL.localRotation = Quaternion.Euler( new Vector3( 0f, rudderSlider.value, 0f ) );
		rudderR.localRotation = Quaternion.Euler( new Vector3( 0f, rudderSlider.value, 0f ) );
	}

	protected void CalculateForwardThrust() {
		float inIronsBufferZone = 15f;
		float inIronsNullZone = 30f;
		float effectiveAngle =15f;
		
		// If we are within the in irons range check to see if we are in the buffer zone
//		if (angleWRTWind < (inIronsNullZone + inIronsBufferZone))
//			effectiveAngle = Vector3.Angle( Vector3.forward, transform.forward ) > inIronsNullZone ? angleWRTWind - inIronsNullZone : 0f;
//		else {
//			effectiveAngle = 15f;
//		}
		
		optimalAngle = Vector3.Angle( Vector3.forward, transform.forward ) * 0.33f; //TODO Fiddle around with the constant to see what works for us
		if (boomSlider != null) {
			sailEffectiveness = Vector3.Angle (Vector3.forward, transform.forward) > inIronsNullZone ? optimalAngle / (Mathf.Abs (boomSlider.value - optimalAngle) + optimalAngle) : 0f;
		} else {
			sailEffectiveness = Vector3.Angle (Vector3.forward, transform.forward) > inIronsNullZone ? optimalAngle / (Mathf.Abs (angleWRTWind - optimalAngle) + optimalAngle) : 0f;
		}
		sailEffectiveness = Mathf.Pow(sailEffectiveness,3f);
		boatThrust = (effectiveAngle/inIronsBufferZone) * sailEffectiveness * boatMovementVelocityScalar;
	}

	protected void ApplyForwardThrust () {
		myRigidbody.AddForce( transform.forward * boatThrust);
	}
	#region SailAnimPlusKeel
	protected void SetSailAnimator () {
		//sail animator handles luffing etc...
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

		if (angle >= 115f&&!isJibing) {
			sail.SetFloat ("sailtrim", isNegative*-1);// -1 bc jon setup animator backwards
		}

		else if (boomSlider.value < optimalAngle && angle > 5f && angle <= 165f) {
			blendFloatValue = isNegative *angle/5f;
			sail.SetFloat ("sailtrim", isNegative*-1);// -1 bc jon setup animator backwards
		}
		else if (boomSlider.value < optimalAngle && angle < 5f) {
			isNegative = isNegative *(angle/5f);
			sail.SetFloat ("sailtrim", isNegative*-1);// -1 bc jon setup animator backwards
		}
		else if (boomSlider.value < optimalAngle && angle > 165f){
			isNegative = isNegative *((180-angle)/15f);
			sail.SetFloat ("sailtrim", isNegative*-1);// -1 bc jon setup animator backwards
		}
		else {
			sail.SetFloat ("sailtrim", blendFloatValue*isNegative*-1);// -1 bc jon setup animator backwards
		}
		//handle keeling
		float zAxisRotation = 0f; //what we use to set the keel value
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
	#endregion
	#region ApplyBoatRotation
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
		}
	}
	#endregion
	#region SetPointOfSailText
	protected void IdentifyPointOfSail() {
		if( pointOfSail == null ) {
//			Debug.LogWarning( gameObject.name +"'s "+ this.GetType().ToString() +" is missing a reference to the \"Point of Sail\" Text object." );
			return;
		}
		if ((angleWRTWind < 360f && angleWRTWind > 330f) ||
		    (angleWRTWind > 0f && angleWRTWind < 30f)) {
			pointOfSail.text = "In Irons";
		}
		else if ((angleWRTWind < 330f && angleWRTWind > 320f) ||
		         (angleWRTWind > 0f && angleWRTWind < 30f)) {
			pointOfSail.text = "Close-Hauled Starboard Tack";
		}
		else if ((angleWRTWind < 320f && angleWRTWind > 290f) ||
		         (angleWRTWind > 0f && angleWRTWind < 30f)) {
			pointOfSail.text = "Close Reach Starboard Tack";
		}
		else if ((angleWRTWind <= 290f && angleWRTWind > 250f) ||
		         (angleWRTWind > 0f && angleWRTWind < 30f)) {
			pointOfSail.text = "Beam Reach Starboard Tack";
		}
		else if ((angleWRTWind < 250f && angleWRTWind > 190f) ||
		         (angleWRTWind > 0f && angleWRTWind < 30f)) {
			pointOfSail.text = "Broad Reach Starboard Tack";
		}
		else if (angleWRTWind < 190f && angleWRTWind > 170f) {
			pointOfSail.text = "Run";
		}
		else if (angleWRTWind > 110f && angleWRTWind < 170f) {
			pointOfSail.text = "Broad Reach Port Tack";
		}
		else if (angleWRTWind >= 70f && angleWRTWind < 110f) {
			pointOfSail.text = "Beam Reach Port Tack";
		}
		else if (angleWRTWind > 40f && angleWRTWind < 70f) {
			pointOfSail.text = "Close Reach Port Tack";
		}
		else if (angleWRTWind > 30f && angleWRTWind < 40f){
			pointOfSail.text = "Close-Hauled Port Tack";
			print ("CH");
		}
	}
	#endregion
	#region MastRotation
	protected void MastRotation() {

		//handles jibes, and mast rotation
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
			ApplySailTrim();
			
		} else if (isJibing) {
			float fracJourney = (Time.time - lerpTimer)/lerpDuration;
			float lerpAngleFloatVal = Mathf.Lerp(lerpStart, lerpEnd, fracJourney);
			boom.localRotation = Quaternion.Euler(0,lerpAngleFloatVal,0);
			if (fracJourney > .99f) {
				isJibing = false;
				fracJourney=1;
				lerpAngleFloatVal = Mathf.Lerp(lerpStart, lerpEnd, fracJourney);
				boom.localRotation = Quaternion.Euler(0,lerpAngleFloatVal,0);
				if (rudderSlider!=null)rudderSlider.interactable = true;
				boomSlider.interactable = true;
				controlsAreActive = true;
			}
		}

	}
	#endregion
	#region Jibe
	protected virtual void Jibe(float negative) {
		isJibing = true;
		lerpTimer = Time.time;
		lerpStart = boom.localRotation.eulerAngles.y > 180 ? boom.localRotation.eulerAngles.y - 360 : boom.localRotation.eulerAngles.y;
		lerpEnd = -lerpStart;
		if (rudderSlider!=null)rudderSlider.interactable = false;
		boomSlider.interactable = false;
		controlsAreActive = false;
//		if (SoundtrackManager.s_instance != null)
//			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.gybe);
		if (MOBManager.s_instance != null) {
			MOBManager.s_instance.Fail();
		}
	}
	#endregion
	#region ApplySailTrim
	protected void ApplySailTrim() {
//		if( controlsAreActive ) {
//			float input = Input.GetAxis( "Vertical" );
//			// If player is pressing down
//			if( input < 0f ) {
//				boomSlider.value += boomTrimSpeed * Time.deltaTime;
//			}
//			// If player is pressing up
//			if( input > 0f ) {			
//				boomSlider.value -= boomTrimSpeed * Time.deltaTime;
//			}
//		}

		float maxBoomAngle = boomSlider.maxValue;
		// If we're less than 90 degrees from in irons clamp boom's max angle
		if( Vector3.Angle( Vector3.forward, transform.forward) <= 90f )
			maxBoomAngle = Vector3.Angle( Vector3.forward, transform.forward );

		float clampedBoomAngle = Mathf.Clamp( boomSlider.value, 0f, maxBoomAngle );
		// Mirror canvas's position dependingon what way we are facing the wind.
		Vector3 newBoomDirection = boom.localRotation * Vector3.forward;    
		if( angleWRTWind >= 180f ) {
//			boom.localRotation = Quaternion.Euler (0, clampedBoomAngle, 0);
			newBoomDirection = Vector3.RotateTowards(newBoomDirection, Quaternion.Euler( 0f, clampedBoomAngle, 0f )*Vector3.forward,0.05f, 0.05f);
		} else {
//			boom.localRotation = Quaternion.Euler (0, -clampedBoomAngle, 0);

			newBoomDirection = Vector3.RotateTowards(newBoomDirection, Quaternion.Euler( 0f, -clampedBoomAngle, 0f )*Vector3.forward,0.05f, 0.05f);
		}
		boom.localRotation = Quaternion.LookRotation (newBoomDirection);
	}
	#endregion
	#region SinkMechanics
	private void BoatHasCrashed() {
		isCrashing = true;
		StartCoroutine ("Sink");
	}

	IEnumerator Sink() {
		myRigidbody.mass *= 10f;
//		GameObject.FindGameObjectWithTag ("deathPopUp").GetComponent<Text> ().enabled = true;
//		GameObject.FindGameObjectWithTag ("deathPopUp").GetComponent<Text> ().text = "You crashed and sank, try again!";
		Camera.main.GetComponent<HoverFollowCam> ().thisCameraMode = HoverFollowCam.CameraMode.stationary;

		yield return new WaitForSeconds (4f);
		if (respawnTransform == null) {
			transform.position = Vector3.zero;
			Debug.LogWarning ("No Respawn Transformed Assigned to NavBoatControl");
		} else {
			transform.position = respawnTransform.position;
		}
		transform.rotation = Quaternion.identity;
		if( GameManager.s_instance.thisLevelState == GameManager.LevelState.Navigation )
			transform.rotation = respawnTransform.rotation;
		myRigidbody.mass /= 10f;
		myRigidbody.isKinematic = true;
		yield return new WaitForSeconds (.1f);
		myRigidbody.isKinematic = false;

//		GameObject.FindGameObjectWithTag ("deathPopUp").GetComponent<Text> ().enabled = false;
		Camera.main.GetComponent<HoverFollowCam> ().thisCameraMode = HoverFollowCam.CameraMode.follow;
		isCrashing = false;
	}
	#endregion
	void OnCollisionEnter (Collision thisCollision) {
		if (thisCollision.gameObject.tag == "collisionObject" && !isCrashing) {
			if (TackManager.s_instance != null) {
				TackManager.s_instance.Fail ();
				return;
			}
			BoatHasCrashed ();
		}
		if (thisCollision.gameObject.tag == "ROWFail" ) {

			if (RightOfWayManager.s_instance != null) {
				RightOfWayManager.s_instance.Fail ();
			} else {
				
				SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.crash);
				StartCoroutine ("Sink");
				if (thisCollision.gameObject.GetComponent<EvilYacht> ()!=null)
				thisCollision.gameObject.GetComponent<EvilYacht> ().Kill ();
			}
		}
	}

	void HandleWindArrowMovement() {
		foreach (WindArrow x in GetComponentsInChildren<WindArrow>()) {
			x.transform.Translate (Vector3.forward*.5f);
		}
	}
}
