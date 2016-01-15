using UnityEngine;
using System.Collections;

//this is used on the Risorry ght of Way Module

public class BoolCollider : MonoBehaviour {

	public bool isTrue;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			if (isTrue) {

			} else {

			}
		}
	}
}
