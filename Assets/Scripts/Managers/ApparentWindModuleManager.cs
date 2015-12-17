using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Manages the Apparent Wind module.
/// </summary>
public class ApparentWindModuleManager : MonoBehaviour {
	public static ApparentWindModuleManager s_instance;
	public enum GameState { Intro, Playing, Complete };

	public GameState gameState = GameState.Intro;
	public List<Term> listOfPOSTerms,tempListPointTerms,randomListPoints;
	[System.NonSerialized]
	public bool hasClickedRun;
	[System.NonSerialized]
	public string currAnimState;
	public Vector3 directionOfWind = new Vector3 (1f,0,1f);
	public GameObject[] instructionPanels;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
			Debug.LogWarning( "Deleting "+ gameObject.name +" because it is a duplicate ApparentWindModuleManager." );
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
		ConfirmationPopUp.InitializeConfirmationPanel( "move on to the next level?", (bool confirmed) => {
			if( confirmed == true ) {
				ChangeState( GameState.Complete );
				Debug.Log( "Accepted to go to next level." );
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
