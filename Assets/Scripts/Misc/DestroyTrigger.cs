using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
/// <summary>
/// Destroys any object marked with the "Destroyable" tag. Requires a collider and marks it as a trigger.
/// </summary>
public class DestroyTrigger : MonoBehaviour {

	/// <summary>
	/// List of layers that can be destroyed with this trigger.
	/// </summary>
	public int[] destroyableLayers = new int[1] {0};

	void Start() {
		GetComponent<Collider>().isTrigger = true;
	}

	void OnTriggerEnter( Collider col ) {
		DestroyableObject dO = col.GetComponent( "DestroyableObject" ) as DestroyableObject;

		if( dO != null  ) {
			foreach( int destroyLayer in destroyableLayers ) {
				if( destroyLayer == dO.destroyableLayerIndex )
					Destroy( col.gameObject );					
			}
		}
	}
}
