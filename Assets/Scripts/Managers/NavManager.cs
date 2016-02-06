using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
	This class handles the State machine for the boat navigation module
  	it also handles the game flow and level progress logic
*/


public class NavManager : MonoBehaviour {

	public static NavManager s_instance;

	public enum GameState {IntroCredit, MainMenu, Pause, Instructions, Gameplay, Win, Lose, GameOver};
	public GameState gameState = GameState.Instructions;
	public GameObject[] navigationPoints;
	public bool hasReachedAllTargets;
	public int currNavPoint = 0;
	private float startTime;
	public float elapsedTime;

	[SerializeField] GameObject INarrows, OUTarrows, levelTrigger0, levelTrigger1;

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
				ChangeState( GameState.Gameplay );
			break;

		case GameState.Gameplay :
			if (hasReachedAllTargets) {
				ChangeState( GameState.Win );
				break;
			}
//			directionalArrow.transform.LookAt(navigationPoints[currNavPoint].transform);
			elapsedTime = Time.time - startTime;			                                   
			break;

		case GameState.Win :
			break;
		}
	}

	public void ChangeState( GameState newState ) {
		switch( newState )
		{
		case GameState.IntroCredit:
			break;

		case GameState.MainMenu:
			break;

		case GameState.Instructions:
			break;

		case GameState.Gameplay:
			Camera.main.GetComponent<HoverFollowCam>().enabled = true;
//			NavBoatControl.s_instance.arrow.SetActive(true);

			NavBoatControl.s_instance.canMove = true;
			StartClock();
			break;

		case GameState.Pause:
			break;

		case GameState.Win:
			NavBoatControl.s_instance.arrow.SetActive(false);
			Camera.main.GetComponent<HoverFollowCam>().PanOut();
			GameObject.FindGameObjectWithTag("arrow").SetActive(false);
			NavBoatControl.s_instance.canMove = false;
			CongratulationsPopUp.s_instance.InitializeCongratulationsPanel( "Navigation" );
//			if (elapsedTime > 200f) {
////				rating = 0;
//			}
//			else if (elapsedTime < 150f) {
////				rating = 2;
//			}
//			else {
////				rating = 1;
//			}
			break;

		case GameState.Lose:
			break;

		case GameState.GameOver:
			break;
		}

		gameState = newState;
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

	public void WinNavModule () {

	}

	public void HasReachedHarbor () {
		INarrows.SetActive (false);
		OUTarrows.SetActive (true);
		levelTrigger0.SetActive (false);
		levelTrigger1.SetActive (true);
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
