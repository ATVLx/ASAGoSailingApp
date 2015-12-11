using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextFix2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Text> ().text = "<color=#FFFFFF>click here </color>\n" +  "to learn the points of sail";

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
