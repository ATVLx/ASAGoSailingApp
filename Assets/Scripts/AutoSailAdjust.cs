using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoSailAdjust : MonoBehaviour {

	public Text angleWRTWindDebug,angleWRTWindDebug2;
	Vector3 directionWindComingFrom = new Vector3(0f,0f,1f);
	float angleToAdjustTo;
	float angleWRTWind;
	Vector3 boatDirection;
	public GameObject boat;
	bool isJibing;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		directionWindComingFrom = GameManager.s_instance.directionOfWind;
		//figure out angular relation ship between boat and wind
		Vector3 localTarget = boat.transform.InverseTransformPoint(directionWindComingFrom);
		//isNegative lets us know port vs starboard
		float isNegative = Mathf.Atan2(localTarget.x, localTarget.z)/Mathf.Abs(Mathf.Atan2(localTarget.x, localTarget.z));
		boatDirection = boat.transform.forward;
		angleWRTWind = Vector3.Angle(boatDirection,directionWindComingFrom) * isNegative;
		if (float.IsNaN(angleWRTWind)) {
			angleWRTWind=0;
		}
//		transform.localRotation = Quaternion.Euler(0,angleWRTWind/2,0);
		transform.rotation = Quaternion.Lerp(Quaternion.identity, boat.transform.rotation, 0.5f);
	}

	void FixedUpdate () {

	}
}