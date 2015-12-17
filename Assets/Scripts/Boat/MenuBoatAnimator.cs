using UnityEngine;
using System.Collections;

public class MenuBoatAnimator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Animator> ().SetFloat ("sailtrim", .84f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
