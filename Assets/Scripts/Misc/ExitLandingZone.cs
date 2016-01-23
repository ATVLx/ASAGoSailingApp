using UnityEngine;
using System.Collections;

public class ExitLandingZone : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" && MOBManager.s_instance !=null) {
			MOBManager.s_instance.Fail ();
		}
		if (other.tag == "Player" && DockingManager.s_instance !=null) {
			DockingManager.s_instance.Fail ();
		}	
	}
}
