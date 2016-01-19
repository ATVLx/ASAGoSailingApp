using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	
	void LateUpdate () {
		transform.LookAt( Camera.main.transform.position, Vector3.down );
	}
}
