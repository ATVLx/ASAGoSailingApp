using UnityEngine;
using System.Collections;

/// <summary>
/// Placed on an enormous sphere collider and serves as the bounds for the world. If the player exits the trigger, his position is set back to the center of the level.
/// </summary>
public class WorldResetTrigger : MonoBehaviour {
	void OnTriggerExit( Collider col ) {
		if( col.tag == "Player" ) {
			Vector3 resetPosition = new Vector3( 0f, col.transform.position.y, 0f );
			col.transform.position = resetPosition;
		}
	}
}
