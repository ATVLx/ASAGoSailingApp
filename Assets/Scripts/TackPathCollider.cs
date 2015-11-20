using UnityEngine;
using System.Collections;

public class TackPathCollider : MonoBehaviour {


	//this script generates the red and green lines used to show tack pathes in the navigation sailing module

	public bool isRed;
	public bool isPlayer;
	Vector3 currentIntersectionPointRed, currentIntersectionPointGreen;
	int currLayerMask;
	LineRenderer thisLineRenderer;

	void Start () {
		if (isPlayer) {
			currLayerMask = 1 << LayerMask.NameToLayer("Player");
		}
		else {
			currLayerMask = 1 << LayerMask.NameToLayer("Target");
		}
		currLayerMask = ~currLayerMask;
		thisLineRenderer = GetComponent<LineRenderer>();
	}



	void Update () {

		//shoot 4 raycasts in the directions of 45 + 90 + 90 + 90 to the direction of wind
		float yRotationWindValue = Vector3.Angle (Vector3.forward, WindManager.s_instance.directionOfWind);

		for (int i = 0; i < 4; i++) {

			Vector3 dir = Quaternion.AngleAxis(yRotationWindValue + 45f + (90f * i), Vector3.up) * Vector3.forward;
			RaycastHit hit;
			if (Physics.Raycast(transform.position, dir, out hit, 500000f, currLayerMask)) {
				if (hit.collider.tag == "green") {
					currentIntersectionPointGreen = hit.point;
				}
				else if (hit.collider.tag == "red") {
					currentIntersectionPointRed = hit.point;

				}
			}
		}

		if (isRed) {
			thisLineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y-.37f,transform.position.z));
			thisLineRenderer.SetPosition(1, new Vector3(currentIntersectionPointRed.x,currentIntersectionPointRed.y-.37f,currentIntersectionPointRed.z));
		}
		else {
			thisLineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y-.37f,transform.position.z));
			thisLineRenderer.SetPosition(1, new Vector3(currentIntersectionPointGreen.x,currentIntersectionPointGreen.y-.37f,currentIntersectionPointGreen.z));
		}
	}


}
