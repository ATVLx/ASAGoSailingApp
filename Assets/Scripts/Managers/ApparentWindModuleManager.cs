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

	public GameState gameState;
	public List<Term> listOfPOSTerms,tempListPointTerms,randomListPoints;
	[System.NonSerialized]
	public bool hasClickedRun;
	[System.NonSerialized]
	public string currAnimState;
	public Vector3 directionOfWind = new Vector3 (1f,0,1f);

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
			Debug.LogWarning( "Deleting "+ gameObject.name +" because it is a duplicate ApparentWindModuleManager." );
		}
	}

	/// <summary>
	/// Action taken when the GUI "Done" button is pressed.
	/// </summary>
	public void DoneButton() {
		ConfirmationPopUp.s_instance.InitializeConfirmationPanel( "move on to the next level?", (bool confirmationBool) => {
			if( confirmationBool == true ) {
				//TODO Uncomment this section when we are ready to change levels
//				GameManager.s_instance.LoadNextLevel();
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
