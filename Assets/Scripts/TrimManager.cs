using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrimManager : MonoBehaviour {

	enum TrimManagerState {Intro, Playing, Complete};
	TrimManagerState thisTrimManagerState;
	private float[] listOfPositions = { 40f, 70f, 90f, 135f, 180f };
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
		switch (thisTrimManagerState) {
		case TrimManagerState.Intro: 

			break;

		case TrimManagerState.Playing:

			break;

		case TrimManagerState.Complete:

			break;
		}

	
	}

	public void NextPOSButton () {
		
	}

	void ButtonLogic () {
		if (sailEfficiencySlider.value > .8f) {
			submitButton.interactable = true;
		} else {
			submitButton.interactable = false;
		}
	}
}
