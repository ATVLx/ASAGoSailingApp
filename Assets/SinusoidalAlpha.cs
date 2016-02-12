using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SinusoidalAlpha : MonoBehaviour {

	float opac = 0;

	void Update () {
		opac += Time.deltaTime;
		GetComponent<Text>().color = new Color(1,1,1,Mathf.Sin(opac));
	}
}
