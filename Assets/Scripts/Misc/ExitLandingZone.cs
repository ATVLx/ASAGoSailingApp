using UnityEngine;
using System.Collections;

public class ExitLandingZone : MonoBehaviour {

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag == "Player" && MOBManager.s_instance !=null) {
			MOBManager.s_instance.Fail ();
		}
		if (other.gameObject.tag == "Player" && DockingManager.s_instance !=null) {
			DockingManager.s_instance.Fail ();
		}	
	}
}
