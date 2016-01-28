using UnityEngine;
using System.Collections;

public class GreenGuidelines : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			TackManager.s_instance.Fail ();
		}
	}
}
