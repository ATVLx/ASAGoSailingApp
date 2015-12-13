using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrimManager : MonoBehaviour {



	bool answerSubmitted;
	int posIndex = 0;
	[SerializeField]
	Button submitButton;
	[SerializeField]
	Slider sailEfficiencySlider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (sailEfficiencySlider.value > .8f) {
			submitButton.interactable = true;
		} else {
			submitButton.interactable = false;
		}

	}

	public void NextPOSButton () {
		
	}
}
