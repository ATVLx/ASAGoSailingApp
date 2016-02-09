using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Nav manager. This class handles the State machine for the boat navigation module it also handles the game flow and level progress logic.
/// </summary>
public class NavManager : MonoBehaviour {

	public static NavManager s_instance;

	public enum GameState {Instructions, Gameplay, Win};
	public GameState gameState = GameState.Instructions;
	public GameObject[] navigationPoints;
	public bool hasReachedAllTargets;
	public int currNavPoint = 0;
	private float startTime;
	public float elapsedTime;
	[SerializeField] Rigidbody boat;
	[SerializeField] GameObject INarrows, OUTarrows, levelTrigger0, levelTrigger1;
	[Header("UI")]
	[SerializeField] Text timerText;
	[SerializeField] GameObject GameplayUI;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	void Update () {

		switch (gameState) 
		{
		case GameState.Instructions :
			break;

		case GameState.Gameplay :
			elapsedTime = Time.time - startTime;
			SetTimerText(false);
			break;

		case GameState.Win :
			break;
		}
	}

	public void ChangeState( GameState newState ) {
		switch( newState )
		{

		case GameState.Gameplay:
			GameplayUI.SetActive( true );
			break;

		case GameState.Win:
			boat.isKinematic = true;
			CongratulationsPopUp.s_instance.InitializeCongratulationsPanel ("Navigation");
			SetTimerText (true);
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
		}
		gameState = newState;
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
		ChangeState (GameState.Win);
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

	void SetTimerText(bool x = false) {
		string min;
		string sec;
		min = Mathf.CeilToInt((elapsedTime / 60)-1).ToString();
		sec = (elapsedTime % 60).ToString("F0");
		if (!x) {
			timerText.text = "Time:\n" + min + "\'" + sec + "\"";
		} else {
			timerText.text = "Your Time Was:\n" + min + "\'" + sec + "\"";
		}
	}

	public void StartGame () {
		boat.isKinematic = false;
		startTime = Time.time;
		ChangeState( GameState.Gameplay );
	}
}
