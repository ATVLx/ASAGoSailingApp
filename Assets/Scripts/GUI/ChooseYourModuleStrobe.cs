using UnityEngine;
using UnityEngine.UI;

public class ChooseYourModuleStrobe : MonoBehaviour {

	float strobeSpeed = 1f;
	float maxAlpha = 1f;
	float minAlpha = 0.5f;
	float opacity = 0;

	void Update () {
		opacity += Time.deltaTime;
		GetComponent<Text>().color = new Color(1,1,1, ( minAlpha + Mathf.PingPong( Time.time*strobeSpeed, maxAlpha-minAlpha ) ));
	}
}
