using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CongratulationsPopUp : MonoBehaviour {
	public static CongratulationsPopUp s_instance;

	[SerializeField]
	RectTransform congratsPanel;
	[SerializeField]
	Text descriptionOfActionText;

	void Awake () {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy (this.gameObject);
			Debug.LogWarning("Deleted " + gameObject.name + " because it was a duplicate confirmation pop up instance");
		}
	}

	/// <summary>
	/// Initializes values and toggles on the confirmation panel.
	/// </summary>
	/// <param name="descriptionOfAction">Text to be shown when asking for confirmation.</param>
	/// <param name="callBack">Call back to be executed when this method exits.</param>
	public void InitializeCongratulationsPanel ( string completedModuleName ) {
		congratsPanel.gameObject.SetActive ( true );
		if( descriptionOfActionText != null ) {
			descriptionOfActionText.text = "You completed the "+ completedModuleName +" module!\n\nTouch the button below to return to the Main Menu.";
		}
		else
			Debug.LogError( gameObject.name +"'s ConfirmationPopUp component is missing a reference for DescriptionOfActionText.");
	}

	public void PressedMainMenu() {
		congratsPanel.gameObject.SetActive( false );
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.MainMenu );
	}
}
