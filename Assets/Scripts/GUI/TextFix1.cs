using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextFix1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Text> ().text = "or " + "<color=#FFFFFF>click here </color>" +  "to skip to level 2";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
