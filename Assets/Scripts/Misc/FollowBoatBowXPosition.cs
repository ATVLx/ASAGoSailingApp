using UnityEngine;
using System.Collections;
/*
	
*/
public class FollowBoatBowXPosition : MonoBehaviour {

	public Transform followObject;

	private Transform myTransform;

	void Start() {
		myTransform = transform;
	}

	void LateUpdate() {
		if( followObject != null ) {
			myTransform.position = new Vector3( followObject.position.x, myTransform.position.y, myTransform.position.z );
		} else {
			Debug.LogError( gameObject.name + " is missing a followObject reference." );
		}
	}
}
