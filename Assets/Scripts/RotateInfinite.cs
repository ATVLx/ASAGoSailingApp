using UnityEngine;
using System.Collections;

public class RotateInfinite : MonoBehaviour {

	public float xAxis, yAxis, zAxis;

	void Update () {
		transform.Rotate (xAxis, yAxis, zAxis);
	}
}
