using UnityEngine;
using System.Collections;

public class EnterLandingZone : MonoBehaviour {

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			MOBManager.s_instance.WinScenario ();
		}
	}
}
