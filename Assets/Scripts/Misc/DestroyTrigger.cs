using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
/// <summary>
/// Destroys any object marked with the "Destroyable" tag. Requires a collider and marks it as a trigger.
/// </summary>
public class DestroyTrigger : MonoBehaviour {

	void Start() {
		GetComponent<Collider>().isTrigger = true;
	}

	void OnTriggerEnter( Collider col ) {
		DestroyableObject temp = col.GetComponent( "DestroyableObject" ) as DestroyableObject;

		if( temp != null && temp.isDestroyable == true ) {
			Destroy( col.gameObject );
		}
	}
}
