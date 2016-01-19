using UnityEngine;
using System.Collections;

public class TackGybeWinBox : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			TackManager.s_instance.WinScenario ();
		}
	}
}
