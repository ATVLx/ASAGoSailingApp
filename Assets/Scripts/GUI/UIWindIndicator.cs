using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIWindIndicator : MonoBehaviour {

	public Transform boatTransform;
	public Transform windIndicator;

	void Start () {
		if( boatTransform == null ) {
			Debug.LogWarning( gameObject.name +"'s "+ this.name +" was missing a reference to player's boat transform. Looking for boat by searching for \"Player\" tag instead." );
			boatTransform = GameObject.FindGameObjectWithTag( "Player" ).transform;
		}
	}

	void LateUpdate () {
		float boatYRot = boatTransform.rotation.eulerAngles.y;
		Quaternion newRot = Quaternion.Euler( 0f, 0f, boatYRot );
		windIndicator.rotation = newRot;
	}
}
