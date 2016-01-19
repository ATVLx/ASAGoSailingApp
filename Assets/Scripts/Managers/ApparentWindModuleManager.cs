using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the Apparent Wind module.
/// </summary>
public class ApparentWindModuleManager : MonoBehaviour {
	public static ApparentWindModuleManager s_instance;
	public enum GameState { Intro, Playing, Complete };

	[System.NonSerialized]
	public bool hasClickedRun;
	[System.NonSerialized]
	public string currAnimState;

	public GameState gameState = GameState.Intro;
	public Vector3 directionOfWind = new Vector3 (1f,0,1f);
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
	public GameObject[] instructionPanels;

	private ApparentWindBoatControl apparentWindBoatControl;
	private int currentInstructionPanel = 0;
	private float lowWindSpeedRendererOffset = 6f;
	private float highWindSpeedRendererOffset = 10f;
	public bool highWindSpeed = false;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
			Debug.LogWarning( "Deleting "+ gameObject.name +" because it is a duplicate ApparentWindModuleManager." );
		}
	}

	void Start() {
		if( gameState == GameState.Intro )
			instructionPanels[0].SetActive( true );

		apparentWindBoatControl = ApparentWindBoatControl.s_instance;
	}

	void Update() {
		switch( gameState ) {
		case GameState.Intro:
			if( (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended ) || Input.GetMouseButtonDown( 0 ) ) {
				instructionPanels[currentInstructionPanel].SetActive( false );

				if( currentInstructionPanel == instructionPanels.Length-1 ) {
					ChangeState( GameState.Playing );
					return;
				}

				currentInstructionPanel++;
				instructionPanels[currentInstructionPanel].SetActive( true );
			}
			break;
		case GameState.Playing:
			Vector3 boatDir = apparentWindBoatControl.myRigidbody.velocity.normalized;
			float boatVel = apparentWindBoatControl.myRigidbody.velocity.magnitude;
			boatVelocityRendererOrigin.position = mastRendererPosition.position + ( boatDir * boatVel );
			break;
		}
	}

	void LateUpdate() {
		Vector3 newOffset = windLineRendererOrigin.position;

		if( !highWindSpeed ) {
			newOffset = boatVelocityRendererOrigin.position + ( Vector3.forward * lowWindSpeedRendererOffset );
		} else {
			newOffset = boatVelocityRendererOrigin.position + ( Vector3.forward * highWindSpeedRendererOffset );
		}

		windLineRendererOrigin.position = newOffset;
	}

	public void ChangeState( GameState newState ) {
		switch( newState ) {
		case GameState.Intro:
			break;
		case GameState.Playing:
			foreach( GameObject panel in instructionPanels )
				panel.SetActive( false );
			break;
		case GameState.Complete:
			// Do completion animation
			// After tell GM to change level
			break;
		}
		gameState = newState;
	}

	/// <summary>
	/// Action taken when the GUI "Done" button is pressed.
	/// </summary>
	public void DoneButton() {
		ConfirmationPopUp.s_instance.InitializeConfirmationPanel( "move on to the next level?", (bool confirmed) => {
			if( confirmed == true ) {
				Debug.Log( "Accepted to go to next level." );
				ChangeState( GameState.Complete );
			} else {
				Debug.Log( "Declined to go to next level." );
			}
		});
	}

	/// <summary>
	/// Action taken when the GUI pause button is pressed.
	/// </summary>
	public void PauseButton() {
		//TODO Tell GameManager to show pause menu.
	}
}
