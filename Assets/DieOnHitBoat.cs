using UnityEngine;
using System.Collections;

public class DieOnHitBoat : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter( Collider col ) {
		if (col.tag == "Player") {
			Destroy (gameObject);
		}
			
	}
}
