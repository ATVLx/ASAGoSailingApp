using UnityEngine;
using System.Collections;

public class RotationLock : MonoBehaviour {

	//locks an object wrt wind

	public float lockAngle = 45f;
	
	// Update is called once per frame
	void Update () {
		float angleOfWindWRTIdentity;
		angleOfWindWRTIdentity = Vector3.Angle(Vector3.forward, WindManager.s_instance.directionOfWind);
		transform.rotation = Quaternion.Euler(new Vector3(0, angleOfWindWRTIdentity, 0) + new Vector3(0,lockAngle,0));
	}
}
