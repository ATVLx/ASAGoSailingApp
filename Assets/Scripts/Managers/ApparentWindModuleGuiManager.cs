using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ApparentWindModuleGuiManager : MonoBehaviour {
	public static ApparentWindModuleGuiManager s_instance;

	public Text trueWindText;
	public Text boatSpeedText;
	public Text apparentWindText;

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

	void LateUpdate() {
		trueWindText.text = "<color=#FF0000FF>True Wind:</color>\t\t\t" + currentWindSpeed.ToString() + " knots";
		boatSpeedText.text = "<color=#FFFF00FF>Boat Speed:</color>\t\t" + currentBoatSpeed.ToString() + " knots";
		apparentWindText.text = "<color=#FF8000FF>Apparent Wind:</color>\t" + currentApparentWindSpeed.ToString("F1") + " knots";
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
}
