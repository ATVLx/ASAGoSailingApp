using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ApparentWindModuleGuiManager : MonoBehaviour {
	public static ApparentWindModuleGuiManager s_instance;

	public RectTransform gameplayUI;
	public RectTransform instructionsUI;
	public Button pauseButton;
	public Text trueWindText;
	public Text boatSpeedText;
	public Text apparentWindText;
	public Button highWindButton;
	public Button lowWindButton;

	private float currentWindSpeed;
	private float currentBoatSpeed;
	private float currentApparentWindSpeed;

	void Awake() {
		if( s_instance == null )
			s_instance = this;
		else {
			Debug.LogWarning( "Destroying " + this.gameObject + " because it is a duplicate ApparentWindModuleGuiManager." );
			Destroy( this.gameObject );
		}
	}

	void Start() {
		if( pauseButton == null ) {
			pauseButton = GameObject.FindObjectOfType<PauseButton>().GetComponent<Button>();
		}
	}

	void LateUpdate() {
		trueWindText.text = "<color=#FF0000FF>True Wind:</color>\t\t\t\t" + currentWindSpeed.ToString() + " knots";
		apparentWindText.text = "<color=#FF8000FF>Apparent Wind:</color>\t" + currentApparentWindSpeed.ToString("F1") + " knots";

		float boatSpeed = currentBoatSpeed;
		if( ApparentWindBoatControl.s_instance.currentPOS == ApparentWindBoatControl.BoatPointOfSail.InIrons )
			boatSpeed = 0f;
			boatSpeedText.text = "<color=#FFFF00FF>Boat Speed:</color>\t\t\t" + boatSpeed.ToString() + " knots";
	}

	public void UpdateTrueWindSpeed( float newSpeed ) {
		currentWindSpeed = newSpeed;
	}

	public void UpdateBoatSpeed( float newSpeed ) {
		currentBoatSpeed = newSpeed;
	}

	public void UpdateApparentWindSpeed( float newSpeed ) {
		currentApparentWindSpeed = newSpeed;
	}

	public void ToggleInstructionsUI( bool toggleOn ) {
		instructionsUI.gameObject.SetActive( toggleOn );
	}

	public void ToggleGameplayUI( bool toggleOn ) {
		gameplayUI.gameObject.SetActive( toggleOn );
		pauseButton.gameObject.SetActive( toggleOn );
	}
}
