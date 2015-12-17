using UnityEngine;
using System.Collections;

public class ArroyDestroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag=="arrow") {
			Destroy(other.gameObject);
		}
	}
}
