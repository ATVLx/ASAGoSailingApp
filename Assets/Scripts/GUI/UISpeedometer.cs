using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISpeedometer : MonoBehaviour {

	public Text velocityText;
	public RectTransform speedDialPivot;
	public Rigidbody boatRigidbody;

	private float maxSpeed;

	void Start () {
		if( boatRigidbody == null ) {
			Debug.LogWarning( gameObject.name +"'s "+ this.name +" was missing a reference to player's boat Rigidbody. Looking for boat by searching for \"Player\" tag instead." );
			boatRigidbody = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<Rigidbody>();
		}
	}

	void LateUpdate () {
		float boatVelocity = boatRigidbody.velocity.magnitude * NavBoatControl.METERS_PER_SECOND_TO_KNOTS;
		velocityText.text = Mathf.Round( boatVelocity / 2.1f ).ToString();

		float newRot = boatVelocity / 15f;
		Quaternion newSpeedDialPivotRot = Quaternion.Euler( 0f, 0f, -(newRot*135f) );			// -135 because the dial rotates between 0 and -135
		speedDialPivot.rotation = newSpeedDialPivotRot;
	}
}
