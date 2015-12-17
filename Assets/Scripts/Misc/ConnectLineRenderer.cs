using UnityEngine;
using System.Collections;

[RequireComponent (typeof (LineRenderer))]
public class ConnectLineRenderer : MonoBehaviour {

	public Transform startPos, endPos;

	private LineRenderer myLineRenderer;

	void Start () {
		myLineRenderer = GetComponent<LineRenderer>();
	}

	void LateUpdate () {
		Vector3[] positions = new Vector3[2] {startPos.position, endPos.position};
		myLineRenderer.SetPositions( positions );
	}
}
