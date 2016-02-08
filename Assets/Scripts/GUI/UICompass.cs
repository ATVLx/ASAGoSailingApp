using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICompass : MonoBehaviour {

	/// <summary>
	/// The childed compass indicator.
	/// </summary>
	public RectTransform childedCompassIndicator;
	public Text headingText;

	public Transform boatTransform;

	void Start () {
		if( childedCompassIndicator == null ) {
			childedCompassIndicator = transform.GetChild( 0 ).GetComponent<RectTransform>();
		}

		if( boatTransform == null ) {
			Debug.LogWarning( gameObject.name +"'s "+ this.name +" was missing a reference to player's boat transform. Looking for boat by searching for \"PLayer\" tag instead." );
			boatTransform = GameObject.FindGameObjectWithTag( "Player" ).transform;
		}
	}

	void LateUpdate () {
		float boatYRot = boatTransform.rotation.eulerAngles.y;
		UpdateHeadingText( boatYRot );
		Quaternion newRot = Quaternion.Euler( 0f, 0f, -boatYRot );
		childedCompassIndicator.rotation = newRot;
	}

	private void UpdateHeadingText( float boatRotation ) {
		float trunkatedRotation = ((boatRotation + 22.5f) % 360f) / 45f;
		Debug.Log( trunkatedRotation.ToString() );
		if( trunkatedRotation < 1f ) {
			headingText.text = "N";
		} else if ( trunkatedRotation < 2f ) {
			headingText.text = "NE";
		} else if ( trunkatedRotation < 3f ) {
			headingText.text = "E";
		} else if ( trunkatedRotation < 4f ) {
			headingText.text = "SE";
		} else if ( trunkatedRotation < 5f ) {
			headingText.text = "S";
		} else if ( trunkatedRotation < 6f ) {
			headingText.text = "SW";
		} else if ( trunkatedRotation < 7f ) {
			headingText.text = "W";
		} else  {
			headingText.text = "NW";
		}
	}
}
