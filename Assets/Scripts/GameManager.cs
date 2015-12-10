using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public enum GameState {IntroCredit, MainMenu, Pause, Instructions, Gameplay, Win, Lose, GameOver};
	[System.NonSerialized]
	public GameState gameState = GameState.Instructions;
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
			DontDestroyOnLoad( this.gameObject );
		}
		else {
			Destroy(gameObject);
		}
	}

	void Update () {
		switch (gameState) 
		{
		case GameState.Instructions :
			if (Input.GetKeyDown(KeyCode.Space) || ( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended ) ){
				ChangeState( GameState.Gameplay );
			}
			break;

		case GameState.Gameplay :
			if (hasReachedAllTargets) {
				ChangeState( GameState.Win );
				break;
			}
			directionalArrow.transform.LookAt(navigationPoints[currNavPoint].transform);
			elapsedTime = Time.time - startTime;			                                   
			break;

		case GameState.Win :
			break;
		}
	}

	public void ChangeState( GameState newState ) {
		gameState = newState;

		switch( newState )
		{
		case GameState.IntroCredit:
			break;

		case GameState.MainMenu:
			break;

		case GameState.Instructions:
			//NavBoatControl.s_instance.arrow.SetActive(false);
//			beep.Play();
//			int rand = Random.Range(0,tracksMusic.Length);
//			tracksMusic[rand].Play();
//			GUIManager.s_instance.UpdateState();
			break;

		case GameState.Gameplay:
			Camera.main.GetComponent<HoverFollowCam>().enabled = true;
			NavBoatControl.s_instance.arrow.SetActive(true);

			NavBoatControl.s_instance.canMove = true;
			NavBoatControl.s_instance.GetComponent<GhostPathRecorder>().StartRecording();
			beep.Play();
			StartClock();
			break;

		case GameState.Pause:
			break;

		case GameState.Win:
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
			break;

		case GameState.Lose:
			break;

		case GameState.GameOver:
			break;
		}

		GUIManager.s_instance.UpdateState();
	}

	void StartClock() {
		startTime = Time.time;
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
	
	public string ReturnCurrNavPointName() {
		if (currNavPoint<navigationPoints.Length) {
			return navigationPoints[currNavPoint].name;
		}
		else {
			return "";
		}
	}
}
