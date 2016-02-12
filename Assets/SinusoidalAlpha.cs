using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SinusoidalAlpha : MonoBehaviour {

	float opac = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		opac += Time.deltaTime;
		GetComponent<Text>().color = new Color(1,1,1,Mathf.Sin(opac));
	}
}
