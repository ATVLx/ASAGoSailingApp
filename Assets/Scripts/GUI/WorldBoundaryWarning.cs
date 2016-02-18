using UnityEngine;
using System.Collections;

public class WorldBoundaryWarning : MonoBehaviour {

	public GameObject warningUIElement;

	private void OnTriggerExit( Collider col ) {
		if( col.tag == "Player" ) {
			warningUIElement.SetActive( true );
		}
	}

	private void OnTriggerEnter( Collider col ) {
		if( col.tag == "Player" ) {
			warningUIElement.SetActive( false );
		}
	}
}
