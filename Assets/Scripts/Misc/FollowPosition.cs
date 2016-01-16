using UnityEngine;
using System.Collections;

/// <summary>
/// Follows boat position depending on what bools are enabled.
/// </summary>
public class FollowPosition : MonoBehaviour {

	public Transform followObject;

	[Header( "Follow Position:" )]

	/// <summary>
	/// Follow x position?
	/// </summary>
	public bool x = false;
	/// <summary>
	/// Follow y position?
	/// </summary>
	public bool y = false;
	/// <summary>
	///  Follow z position?
	/// </summary>
	public bool z = false;

	private Transform myTransform;

	void Start () {
		myTransform = transform;
	}

	void LateUpdate () {
		if( followObject != null ) {
			float newX = ( x ) ? followObject.position.x : myTransform.position.x;
			float newY = ( y ) ? followObject.position.y : myTransform.position.y;
			float newZ = ( z ) ? followObject.position.z : myTransform.position.z;

			myTransform.position = new Vector3( newX, newY, newZ );
		} else {
			Debug.LogError( gameObject.name + " is missing a followObject reference." );
		}
	}
}
