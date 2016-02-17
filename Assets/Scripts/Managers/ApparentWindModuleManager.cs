using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the Apparent Wind module.
/// </summary>
public class ApparentWindModuleManager : MonoBehaviour {
	public static ApparentWindModuleManager s_instance;
	public enum GameState { Intro, Playing, Complete };

	public delegate void ArrowUpdate();
	public static event ArrowUpdate UpdateWindLineArrows;

	public GameState gameState = GameState.Intro;
	public Vector3 directionOfWind = new Vector3 (1f,0,1f);

	[System.NonSerialized]
	public bool hasClickedRun;
	[System.NonSerialized]
	public string currAnimState;

	[Header( "Line Renderers:" )]
	/// <summary>
	/// The mast position used when computing apparent wind arrows.
	/// </summary>
	public Transform mastRendererPosition;
	/// <summary>
	/// The position where the boat velocity arrow will start from.
	/// </summary>
	public Transform boatVelocityRendererOrigin;
	/// <summary>
	/// The position where the wind speed arrow will start from.
	/// </summary>
	public Transform windLineRendererOrigin;

	[Header( "Camera Lerp:" )]
	public Camera mainCamera;
	public Transform lowWindCameraPos;
	public Transform highWindCameraPos;
	public GameObject[] instructionPanels;

	private ApparentWindModuleGuiManager guiManager;
	private ApparentWindBoatControl apparentWindBoatControl;
	private int currentInstructionPanel = 0;
	private float boatVelocityRendererOffsetScalar = 1.75f;
	private float lowWindSpeedRendererOffset = 8f;
	private float highWindSpeedRendererOffset = 14f;
	private bool isWindSpeedSetToHigh = false;
	private bool cameraIsLerping = false;

	// GUI Text values
	private float lowWindSpeed = 9f;
	private float highWindSpeed = 13f;
	private float lowBoatSpeed = 3f;
	private float highBoatSpeed = 6f;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
			Debug.LogWarning( "Deleting "+ gameObject.name +" because it is a duplicate ApparentWindModuleManager." );
		}
	}

	void OnEnable() {
		GameManager.TogglePause += TogglePause;
	}

	void OnDisable() {
		GameManager.TogglePause -= TogglePause;
	}

	void Start() {
		// Set up variables for camera lerp
		if( mainCamera == null )
			mainCamera = Camera.main;
		if( lowWindCameraPos == null )
			lowWindCameraPos = GameObject.Find( "LowWindCameraPos" ).transform;
		if( highWindCameraPos == null )
			highWindCameraPos = GameObject.Find( "HighWindCameraPos" ).transform;

		// Get references to singletons
		guiManager = ApparentWindModuleGuiManager.s_instance;
		guiManager.UpdateTrueWindSpeed( lowWindSpeed );
		guiManager.UpdateBoatSpeed( lowBoatSpeed );

		apparentWindBoatControl = ApparentWindBoatControl.s_instance;
		ChangeState( GameState.Intro );
	}

	void Update() {
		switch( gameState ) {
		case GameState.Intro:
			break;
		case GameState.Playing:
			break;
		}
	}

	void LateUpdate() {
		// Update boat velocity renderer's position
		boatVelocityRendererOrigin.position = mastRendererPosition.position + apparentWindBoatControl.myRigidbody.velocity*boatVelocityRendererOffsetScalar;

		// Update wind line renderer's position
		Vector3 newOffset = windLineRendererOrigin.position;
		if( !isWindSpeedSetToHigh ) {
			newOffset = boatVelocityRendererOrigin.position + ( Vector3.forward * lowWindSpeedRendererOffset ) + ( Vector3.up * 0.05f );
		} else {
			newOffset = boatVelocityRendererOrigin.position + ( Vector3.forward * highWindSpeedRendererOffset ) + ( Vector3.up * 0.05f );
		}
		windLineRendererOrigin.position = newOffset;

		// Update line renderers
		// TODO Make this use and event just like the arrows do down bottom
		boatVelocityRendererOrigin.GetComponent<ConnectLineRenderer>().UpdatePosition();
		windLineRendererOrigin.GetComponent<ConnectLineRenderer>().UpdatePosition();
		mastRendererPosition.GetComponent<ConnectLineRenderer>().UpdatePosition();

		CalculateApparentWind();

		// Update line arrows
		if( UpdateWindLineArrows != null )
			UpdateWindLineArrows();
	}

	public void ChangeState( GameState newState ) {
		switch( newState ) {
		case GameState.Intro:
			guiManager.ToggleGameplayUI( true );
			guiManager.ToggleGameplayUI( false );
			break;
		case GameState.Playing:
			guiManager.ToggleInstructionsUI( false );
			guiManager.ToggleGameplayUI( true );
			break;
		case GameState.Complete:
			// TODO Do completion animation
			GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.MainMenu );
			break;
		}
		gameState = newState;
	}

	private void CalculateApparentWind() {
		if( apparentWindBoatControl.currentPOS != ApparentWindBoatControl.BoatPointOfSail.InIrons ) {
			Vector3 fakeWindVector = (boatVelocityRendererOrigin.position - windLineRendererOrigin.position).normalized;
			Vector3 fakeBoatSpeedVector = (mastRendererPosition.position - boatVelocityRendererOrigin.position).normalized;

			if( !isWindSpeedSetToHigh ) {
				fakeWindVector *= lowWindSpeed;
				fakeBoatSpeedVector *= lowBoatSpeed;
			} else {
				fakeWindVector *= highWindSpeed;
				fakeBoatSpeedVector *= highBoatSpeed;
			}

			Vector3 calculatedApparentWind = fakeWindVector + fakeBoatSpeedVector;

			guiManager.UpdateApparentWindSpeed( calculatedApparentWind.magnitude );
		} else {
			if( !isWindSpeedSetToHigh ) 
				guiManager.UpdateApparentWindSpeed( lowWindSpeed );
			else
				guiManager.UpdateApparentWindSpeed( highWindSpeed );
		}
	}

	/// <summary>
	/// Lerps the camera.
	/// </summary>
	/// <param name="lerpToHighWindPos">If set to <c>true</c> lerps to high wind position.</param>
	private void LerpCamera( bool lerpToHighWindPos ) {
		StartCoroutine( LerpCameraToPos( lerpToHighWindPos, 0.35f ) );
	}

	/// <summary>
	/// Lerps the camera to position.
	/// </summary>
	/// <returns>The camera to position.</returns>
	/// <param name="lerpToHighWindPos">If set to <c>true</c> lerps to high wind position from low wind position.</param>
	/// <param name="duration">Duration of lerp in seconds.</param>
	private IEnumerator LerpCameraToPos( bool lerpToHighWindPos, float duration ) {
		Vector3 startPos, endPos;
		if( lerpToHighWindPos ) {
			startPos = lowWindCameraPos.position;
			endPos = highWindCameraPos.position;
		} else {
			startPos = highWindCameraPos.position;
			endPos = lowWindCameraPos.position;
		}

		float lerpDuration = duration;
		float lerpTimer = 0f;

		while( lerpTimer < lerpDuration ) {
			if( (lerpTimer/lerpDuration) >= 0.99f ) {
				mainCamera.transform.position = endPos;
				break;
			}

			mainCamera.transform.position = Vector3.Slerp( startPos, endPos, lerpTimer/lerpDuration );
			yield return null;
			lerpTimer += Time.deltaTime;
		}

		cameraIsLerping = false;
	}

	/// <summary>
	/// Actions taken when the Wind Speed Toggle Button is pressed in the scene.
	/// </summary>
	public void WindSpeedToggleButton() {
		if( !cameraIsLerping ) {
			cameraIsLerping = true;
			// If we are currently in high wind speed, lerp to low wind camera position
			if( isWindSpeedSetToHigh ) {
				LerpCamera( false );
				guiManager.UpdateTrueWindSpeed( lowWindSpeed );
				guiManager.UpdateBoatSpeed( lowBoatSpeed );
				apparentWindBoatControl.SetHighSpeed( false );
				guiManager.lowWindButton.gameObject.SetActive( true );
				guiManager.highWindButton.gameObject.SetActive( false );
 			}
			else {
				LerpCamera( true );
				guiManager.UpdateTrueWindSpeed( highWindSpeed );
				guiManager.UpdateBoatSpeed( highBoatSpeed );
				apparentWindBoatControl.SetHighSpeed( true );
				guiManager.lowWindButton.gameObject.SetActive( false );
				guiManager.highWindButton.gameObject.SetActive( true );
			}

			isWindSpeedSetToHigh = !isWindSpeedSetToHigh;
		}
	}

	/// <summary>
	/// Action taken when the GUI "Done" button is pressed.
	/// </summary>
	public void DoneButton() {
		ConfirmationPopUp.s_instance.InitializeConfirmationPanel( "Exit Apparent Wind module?", (bool confirmed) => {
			if( confirmed == true ) {
				Debug.Log( "Accepted to go to next level." );
				ChangeState( GameState.Complete );
			} else {
				Debug.Log( "Declined to go to next level." );
			}
		});
	}

	public void TogglePause( bool toggleOn ) {
		guiManager.ToggleGameplayUI( !toggleOn );
	}
}
