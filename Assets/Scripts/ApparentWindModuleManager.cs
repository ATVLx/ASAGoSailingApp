using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ApparentWindModuleManager : MonoBehaviour {

	//Manages the Points of Sail Module
	public enum GameState { Intro, Playing, Complete };
	public GameState gameState;
	public List<Term> listOfPOSTerms,tempListPointTerms,randomListPoints;
	List<pointOfSail> allPoints;
	bool userClickedStart = true;
	int requiredMastery = 2;
	int currIndex = 0;
	int currentLevel = 0;
	int totalMastery;
	int numberCorrect, numberWrong;

	public bool hasClickedRun;

	public ColorChange thisColorChange;
	public BreatheAnimation thisBreatheAnimation;

	float[] randomRotateValues = {35f,70f, 100f, 140f, 170f, 200f, 230f, 280f, 320f};
	float startTime, exitTime = 4f;
	float currMastery;
	public Vector3 directionOfWind = new Vector3 (1f,0,1f);
	public AudioSource wrong, correct, beep, waterPaddle;
	public GameObject circle1, circle2;
	bool clickedStart;

	public GameObject IdlePage, TestPage, InstructionsPage, GameplayPage, winPage, challengePage;
	GameObject currentPage;

	public TextAsset pointsOfSailTxt;
	public Slider masteryMeter;
	public Text youbeatlevel2;
	public Text endOfLevel2Text;
	public Text currentQuestion;
	public Text wrongAnswerText;
	public Text correctText;
	public Text winPercentage;
	public Timer1 timer;
	public string currAnimState;

	public static ApparentWindModuleManager s_instance;


	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
		}
		currentPage = IdlePage;
	}

	public void MainMenu() {
		Application.LoadLevel(0);
	}

	void Update () 
	{

		if (isCameraRotating) {
			float fracJourney = (Time.time - lerpTime)/ lerpDuration;
			if (fracJourney > .99f) {
				isCameraRotating = false;
				fracJourney = 1;

			}
			Camera.main.transform.rotation = Quaternion.Lerp(lerpStart, lerpFinish, fracJourney);
		}
		switch (gameState) {
//		case GameState.Idle :
//			if (clickedStart){
//				clickedStart = false;	
//				IdlePage.SetActive(false);
//				TestPage.SetActive(true);
//				gameState = GameState.TestPage;
//				beep.Play();
//			}
//			break;
	
//		case GameState.TestPage :
//			if (Input.GetKeyDown(KeyCode.Space)){
//				TestPage.SetActive(false);
//				InstructionsPage.SetActive(true);
//				gameState = GameState.Instructions;
//				beep.Play();
//			}
//			break;
//		case GameState.Instructions :
//			if (Input.GetKeyDown(KeyCode.Space)){
//				InstructionsPage.SetActive(false);
//				GameplayPage.SetActive(true);
//				gameState = GameState.Config;
//				beep.Play();
//				Camera.main.GetComponent<QuaternionLerp>().StartLerp(5f);
//			}
//			break;
//		case GameState.Config :
//			numberWrong = 0;
//			numberCorrect = 0;
//			directionOfWind = new Vector3(0,0,1f);
//			allPoints = new List<pointOfSail>();
//			allPoints = TextParser.Parse(pointsOfSailTxt);
//			listOfPOSTerms = new List<Term>();
//			randomListPoints = new List<Term>();
//
//			for (int i = 0; i < allPoints.Count; i++) { //fill out list of Term class instances
//				Term tempTerm = new Term();
//				tempTerm.initIndex = i;
//				tempTerm.pointOfSailAnswer = allPoints[i].sailTitle;
//				listOfPOSTerms.Add(tempTerm);
//			}
//			
//			tempListPointTerms = new List<Term>(listOfPOSTerms);
//			while (tempListPointTerms.Count > 0) //shuffle list
//			{
//				int randomIndex = Mathf.FloorToInt(Random.Range(0, tempListPointTerms.Count));//r.Next(0, inputList.Count); //Choose a random object in the list
//				randomListPoints.Add(tempListPointTerms[randomIndex]); //add it to the new, random list
//				tempListPointTerms.RemoveAt(randomIndex); //remove to avoid duplicates
//			}
//
//			totalMastery = requiredMastery * listOfPOSTerms.Count;
//			gameState = GameState.Intro;
//			break;
			
//		case GameState.Intro : 
//			if (userClickedStart) {
//				gameState = GameState.SetRound;
//			}
//			break;
			
//		case GameState.SetRound :
//			CheckForSequenceMastery(); //eliminate mastered sequences
//			InitiateTerm();
//			gameState = GameState.Playing;
//			circle2.SetActive(true);
//			circle1.SetActive(false);
//			break;
		case GameState.Playing :
			if (Input.GetKeyDown(KeyCode.Space)){ //when boat has been rotated
//				gameState = GameState.CheckAnswer;
			}
			break;
//		case GameState.Challenge :
//			if (Input.GetKeyDown(KeyCode.Space)){ //when boat has been rotated
//				gameState = GameState.Config;
//				beep.Play ();
//				challengePage.SetActive(false);
//				GameplayPage.SetActive(true);
//				
//			}
//			break;
//		case GameState.CheckAnswer :
//			if (Checker()) {
//				gameState = GameState.CorrectAnswer;
//				if (currentLevel == 1) {
//					RotateCameraRandom();
//				}
//			}
//			else {
//				gameState = GameState.WrongAnswer;
//			}
//			break;
			
//		case GameState.CorrectAnswer :
//			if (AnswerCorrect()){
//				WinRound();
//				gameState = GameState.WinScreen;
//			}
//			else {
//				gameState = GameState.SetRound;
//			}
//			break;
			
//		case GameState.WrongAnswer :
//			AnswerWrong();
//			gameState = GameState.Playing;
//			break;
			
//		case GameState.WinScreen :
//			GameplayPage.SetActive(false);
//			winPage.SetActive(true);
//			if (currentLevel == 1) {
//				thisBreatheAnimation.enabled = true;
//				thisColorChange.enabled = true;
//				youbeatlevel2.text = "you beat level 2";
//				endOfLevel2Text.text = "Click here to play level 2 again";
//			}
//			winPercentage.text = "Your score is " + Mathf.Ceil(((float)numberCorrect/((float)numberWrong+(float)numberCorrect))*100)+"%";
//			break;
		}
	}

	bool Checker() {
		if (randomListPoints[currIndex].pointOfSailAnswer.Replace("|"," ") == currAnimState) {
			return true;
		}
		else {
			return false;
		}
	}
	void InitiateTerm(){
		currentQuestion.text = "Turn the boat to " + randomListPoints[currIndex].pointOfSailAnswer.Replace("|"," ");
		timer.Reset(25f);
	}
	void WinRound(){
		startTime = Time.time;

	}
	bool AnswerCorrect(){
		circle1.SetActive(false);
		circle2.SetActive(true);
		correct.Play ();
		wrongAnswerText.enabled = false;
		correctText.enabled = true;
		correctText.GetComponent<Fader>().StartFadeOut();
		AdjustMasteryMeter(true);
		numberCorrect++;
		if (masteryMeter.value > .97f ) {
			return true;
		} else { 
			return false;
		}

	}
	void AnswerWrong(){
		numberWrong++;
		circle2.SetActive(false);
		circle1.SetActive(true);
		wrong.Play ();
		AdjustMasteryMeter(false);
		timer.timesUp = true;
		timer.pause = true;
		DisplayFeedbackText();
	}

	public void ClickStart () {
		clickedStart = true;
	}

	void DisplayFeedbackText () {
		//Nope, you selected this position, try again
		wrongAnswerText.enabled = true;
		wrongAnswerText.text = "Incorrect, you selected: " + currAnimState;

	}
	void GotoNextModule(){
		Application.LoadLevel(1);
	}

	void AdjustMasteryMeter(bool didAnswerCorrect) {
		
		if (didAnswerCorrect && !timer.timesUp) {
			
			listOfPOSTerms[randomListPoints[currIndex].initIndex].mastery += 1;
		}
		
		else if (!didAnswerCorrect) {
			if (listOfPOSTerms[randomListPoints[currIndex].initIndex].mastery > 0) {
				
				listOfPOSTerms[randomListPoints[currIndex].initIndex].mastery -= 1;
			}
		}
		SetMastery();
		
	}
	
	void SetMastery() {
		currMastery = 0;
		foreach (Term x in listOfPOSTerms) {
			currMastery+=x.mastery;
		}
		masteryMeter.value = (float)(currMastery)/totalMastery;
		masteryMeter.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "Mastery: " + Mathf.FloorToInt(masteryMeter.value*100f).ToString()+"%";
	}

	bool isCameraRotating;
	float lerpTime;
	float lerpDuration = .5f;
	Quaternion lerpStart;
	Quaternion lerpFinish;


	void RotateCameraRandom() {
		float chooseRandomRotateVal = randomRotateValues[Random.Range(0,randomRotateValues.Length-1)];
		lerpTime = Time.time;
		lerpStart = Camera.main.transform.rotation;
		lerpFinish = Quaternion.Euler (new Vector3 (90, chooseRandomRotateVal, 0));
		isCameraRotating = true;
	}

	public void SwitchToChallenge () {
		beep.Play ();
		IdlePage.SetActive (false);
		GameplayPage.SetActive (false);
		challengePage.SetActive (true);
		winPage.SetActive (false);
		Camera.main.GetComponent<QuaternionLerp>().StartLerp(5f);

		currentLevel = 1;
//		if (gameState == GameState.WinScreen) {
//			currMastery = 0;
//			masteryMeter.value = 0;
//			currentLevel = 1;
//			masteryMeter.transform.GetChild (1).transform.GetChild (1).GetComponent<Text> ().text = "Mastery 0%";
//			foreach (Term x in listOfPOSTerms) {
//				x.mastery = 0;
//			}
//		}
//		gameState = GameState.Challenge;
	}

	public void CheckForSequenceMastery() {
		if (randomListPoints.Count == 0) {
			WinRound();
			return;
		}
		else {
			for (int i = 0; i < randomListPoints.Count; i++) {
				if (randomListPoints[i].mastery == requiredMastery) { //skip over completed 
					randomListPoints.Remove(randomListPoints[i]);
				}
			}
			if (randomListPoints.Count > currIndex+1) {
				currIndex++;
			}
			else {
				currIndex = 0;
			}
		}
	}

}
