using UnityEngine;
using System.Collections;

public class MenuBoatAnimator : MonoBehaviour {

	//This class just sets the sail animator so that it looks good in the main menu
	void Start () {
		GetComponent<Animator> ().SetFloat ("sailtrim", .84f);
	}
}
