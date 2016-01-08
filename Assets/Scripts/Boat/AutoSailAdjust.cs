﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/*
	This class sets the mast to swing at a constant rate wrt wind
*/
public class AutoSailAdjust : MonoBehaviour {

	public Text angleWRTWindDebug,angleWRTWindDebug2;
	Vector3 directionWindComingFrom = new Vector3(0f,0f,1f);
	float angleToAdjustTo;
	float angleWRTWind;
	Vector3 boatDirection;
	public GameObject boat;
	bool isJibing;

	// Update is called once per frame
	void Update () {
		if (ApparentWindModuleManager.s_instance != null) {
			directionWindComingFrom = ApparentWindModuleManager.s_instance.directionOfWind;
		}
		else {
			directionWindComingFrom = Vector3.forward;
		}
		//figure out angular relation ship between boat and wind
		Vector3 localTarget = boat.transform.InverseTransformPoint(directionWindComingFrom);
		//isNegative lets us know port vs starboard
		float isNegative = Mathf.Atan2(localTarget.x, localTarget.z)/Mathf.Abs(Mathf.Atan2(localTarget.x, localTarget.z));
		boatDirection = boat.transform.forward;
		angleWRTWind = Vector3.Angle(boatDirection,directionWindComingFrom) * isNegative;
		if (float.IsNaN(angleWRTWind)) {
			angleWRTWind=0;
		}
		transform.localRotation = Quaternion.Lerp(Quaternion.identity, boat.transform.rotation, 0.33f);
	}

}