using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the Apparent Wind module.
/// </summary>
public class ApparentWindModuleManager : MonoBehaviour {
	public static ApparentWindModuleManager s_instance;
	public enum GameState { Intro, Playing, Complete };

	public GameState gameState = GameState.Intro;
	[System.NonSerialized]
	public bool hasClickedRun;
	[System.NonSerialized]
	public string currAnimState;
	public Vector3 directionOfWind = new Vector3 (1f,0,1f);
	public GameObject[] instructionPanels;

	private int currentInstructionPanel = 0;

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
		}
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
