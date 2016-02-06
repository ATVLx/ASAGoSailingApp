using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICompass : MonoBehaviour {

	/// <summary>
	/// The childed compass indicator.
	/// </summary>
	public RectTransform childedCompassIndicator;

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
		Quaternion newRot = Quaternion.Euler( 0f, 0f, boatTransform.rotation.eulerAngles.y );
		childedCompassIndicator.rotation = newRot;
	}
}
