using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/*
	This class handles game flow in the Trim Module
*/
public class TrimManager : MonoBehaviour {

	enum TrimManagerState {Intro, Playing, Complete};
	TrimManagerState thisTrimManagerState;
	private float[] listOfPositions = {50f, 70f, 90f, 135f, 178f, 225f, 270f, 290f, 310f};
	bool answerSubmitted;
	int posIndex = 0;
	[SerializeField]
	Button submitButton, gotoNextModule;
	[SerializeField]
	Slider sailEfficiencySlider;
	[SerializeField]
	GameObject introText,goodJob,complete,panel;

	//switches
	bool switchToPlaying, switchToComplete;

	// Use this for initialization
	void Start () {
		submitButton.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		switch (thisTrimManagerState) {
		case TrimManagerState.Intro: 
			if (switchToPlaying) {
				thisTrimManagerState = TrimManagerState.Playing;
				submitButton.gameObject.SetActive(true);
				switchToPlaying = false;
			}
			break;

		case TrimManagerState.Playing:
			SubmitButtonLogic ();
			if (switchToComplete) {
				switchToComplete = false;
				submitButton.gameObject.SetActive( false );
				gotoNextModule.gameObject.SetActive (true);
				complete.SetActive (true);
				thisTrimManagerState = TrimManagerState.Complete;
				panel.SetActive (false);
			}
			break;

		case TrimManagerState.Complete:

			break;
		}

	
	}

	public void BeginTutorial () {
		introText.GetComponent<Fader> ().StartFade ();
		posIndex = 0;
		GameObject.FindGameObjectWithTag("Player").transform.rotation = Quaternion.Euler(new Vector3(0,listOfPositions[posIndex],0));
		switchToPlaying = true;
	}

	public void NextPOSButton () {
		posIndex++;
		goodJob.GetComponent<Fader> ().StartFade ();
		if (posIndex >= listOfPositions.Length) {
			switchToComplete = true;
			return;
		} else {
			GameObject.FindGameObjectWithTag ("Player").transform.rotation = Quaternion.Euler (new Vector3 (0, listOfPositions [posIndex], 0));
		}
	}

	void SubmitButtonLogic () {
		if (sailEfficiencySlider.value > .8f) {
			submitButton.interactable = true;
		} else {
			submitButton.interactable = false;
		}
	}
}
