﻿using UnityEngine;
using System.Collections;

public class ManOverboard : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			MOBManager.s_instance.Fail ();
		}
	}
}
