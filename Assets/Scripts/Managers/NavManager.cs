using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class NavManager : MonoBehaviour {

	public enum GameState {Idle, Review, Instructions, CameraPan, Gameplay, Win};
	public GameState gameState = GameState.Idle;
	public GameObject[] navigationPoints;
	public GameObject reviewPage, idlePage, instructionsPage, gamePlayPage, winPage;
	public static NavManager s_instance;
	public bool hasReachedAllTargets;
	public bool hasFinishedCameraPanning;
	int currNavPoint = 0;
	public Text currTarget;
	public AudioSource beep;
	public AudioSource[] tracksMusic;
	float startTime, elapsedTime;
	public Text timeText;
	int rating;
	public GameObject[] ratingObjects;
	public GameObject directionalArrow;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	public void SwitchNavigationPoint() {
		navigationPoints[currNavPoint].transform.GetChild(0).gameObject.SetActive(false);

		currNavPoint++;
		if (currNavPoint < navigationPoints.Length) {
			navigationPoints[currNavPoint].transform.GetChild(0).gameObject.SetActive(true);
		}
		else {
			hasReachedAllTargets= true;
			winPage.SetActive(true);
			gamePlayPage.SetActive(false);
			NavBoatControl.s_instance.transform.GetChild(0).gameObject.SetActive(false);

		}

	}
	public void MainMenu() {
		Application.LoadLevel(0);
	}

	public string ReturnCurrNavPointName() {
		if (currNavPoint<navigationPoints.Length) {
			return navigationPoints[currNavPoint].name;
		}
		else {
			return "";
		}
	}
	// Update is called once per frame
	void Update () {
		switch (gameState) {
		case GameState.Idle :
			if (Input.GetKeyDown(KeyCode.Space)){
				NavBoatControl.s_instance.arrow.SetActive(false);
				idlePage.SetActive(false);
				reviewPage.SetActive(true);
				gameState = GameState.Review;
				beep.Play();
				int rand = Random.Range(0,tracksMusic.Length);
				tracksMusic[rand].Play();
			}
			break;
		case GameState.Review :
			if (Input.GetKeyDown(KeyCode.Space)){
				reviewPage.SetActive(false);
				instructionsPage.SetActive(true);
				gameState = GameState.Instructions;
				beep.Play();

			}
			break;
		case GameState.Instructions :
			if (Input.GetKeyDown(KeyCode.Space)){
				instructionsPage.SetActive(false);
				Camera.main.GetComponent<HoverFollowCam>().enabled = false;
				Camera.main.GetComponent<Cinematographer>().RollCamera();
				gameState = GameState.CameraPan;
			}
			break;
		case GameState.CameraPan: 
			if (hasFinishedCameraPanning){
				instructionsPage.SetActive(false);
				Camera.main.GetComponent<HoverFollowCam>().enabled = true;
				NavBoatControl.s_instance.arrow.SetActive(true);

				gameState = GameState.Gameplay;
				NavBoatControl.s_instance.canMove = true;
				gamePlayPage.SetActive(true);
				beep.Play();
				StartClock();

			}

			break;
		case GameState.Gameplay :
			if (hasReachedAllTargets) {
				NavBoatControl.s_instance.arrow.SetActive(false);
				Camera.main.GetComponent<HoverFollowCam>().PanOut();
				GameObject.FindGameObjectWithTag("arrow").SetActive(false);
				directionalArrow.SetActive(false);
				NavBoatControl.s_instance.canMove = false;
				if (elapsedTime > 200f) {
					rating = 0;
				}
				else if (elapsedTime < 150f) {
					rating = 2;
				}
				else {
					rating = 1;
				}
				ratingObjects[rating].SetActive(true);
				gameState = GameState.Win;
				break;
			}
			directionalArrow.transform.LookAt(navigationPoints[currNavPoint].transform);
			elapsedTime = Time.time - startTime;
			currTarget.text = "Destination: " + navigationPoints[currNavPoint].name;
			timeText.text = "Elapsed time: " + elapsedTime.ToString("F2") + "s";
			                                   
			break;
		case GameState.Win :
			break;
		}

	}

	void StartClock() {
		startTime = Time.time;
	}
}
