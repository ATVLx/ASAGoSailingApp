using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public enum GameState {Idle, Review, Instructions, CameraPan, Gameplay, Win, Lose};
	public GameState gameState = GameState.Idle;
	public GameObject[] navigationPoints;
	public static GameManager s_instance;
	public bool hasReachedAllTargets;
	public bool hasFinishedCameraPanning;
	public int currNavPoint = 0;
	public AudioSource beep;
	public AudioSource[] tracksMusic;
	private float startTime;
	public float elapsedTime;
	[System.NonSerialized]
	public int rating;
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
			if (Input.GetKeyDown(KeyCode.Space) || ( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended ) ){
				NavBoatControl.s_instance.arrow.SetActive(false);
				gameState = GameState.Review;
				beep.Play();
				int rand = Random.Range(0,tracksMusic.Length);
				tracksMusic[rand].Play();
				GUIManager.s_instance.UpdateState();
			}
			break;
		case GameState.Review :
			if (Input.GetKeyDown(KeyCode.Space) || ( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended ) ){
				gameState = GameState.Instructions;
				beep.Play();
				GUIManager.s_instance.UpdateState();
			}
			break;
		case GameState.Instructions :
			if (Input.GetKeyDown(KeyCode.Space) || ( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended ) ){
			//	Camera.main.GetComponent<HoverFollowCam>().enabled = false;
			//Camera.main.GetComponent<Cinematographer>().RollCamera();
				gameState = GameState.CameraPan;
				GUIManager.s_instance.UpdateState();
			}
			break;
		case GameState.CameraPan: 
				Camera.main.GetComponent<HoverFollowCam>().enabled = true;
				NavBoatControl.s_instance.arrow.SetActive(true);

				gameState = GameState.Gameplay;
				NavBoatControl.s_instance.canMove = true;
				NavBoatControl.s_instance.GetComponent<GhostPathRecorder>().StartRecording();
				beep.Play();
				StartClock();
				GUIManager.s_instance.UpdateState();
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
				gameState = GameState.Win;
				GUIManager.s_instance.UpdateState();
				break;
			}
			directionalArrow.transform.LookAt(navigationPoints[currNavPoint].transform);
			elapsedTime = Time.time - startTime;			                                   
			break;
		case GameState.Win :
			break;
		}

	}

	void StartClock() {
		startTime = Time.time;
	}
}
