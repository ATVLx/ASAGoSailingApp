using UnityEngine;
using System.Collections;

public class ExitLandingZone : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			MOBManager.s_instance.Fail ();
		}
	}
}
