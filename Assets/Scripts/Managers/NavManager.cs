﻿using UnityEngine;
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
	[SerializeField] GameObject INArrows, OUTArrows, levelTrigger0, levelTrigger1;
	[SerializeField] Transform respawn2;

	[Header("UI")]
	[SerializeField] Text timerText;
	[SerializeField] Text yourTimeWasText;
	[SerializeField] GameObject GameplayUI;
	[SerializeField] GameObject timerTextParent;
	[SerializeField] GameObject yourTimeWasTextParent;

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
			GameplayUI.SetActive( false );

			CongratulationsPopUp.s_instance.InitializeCongratulationsPanel ("Sailing Course");
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
		INArrows.SetActive (false);
		OUTArrows.SetActive (true);
		levelTrigger0.SetActive (false);
		levelTrigger1.SetActive (true);

		NavBoatControl.s_instance.respawnTransform = respawn2;
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
		min = ( min.Length < 2 ) ? "0"+min : min;
		sec = ( sec.Length < 2 ) ? "0"+sec : sec;

		if (!x) {
			timerText.text = min.ToString() + "\'" + sec.ToString() + "\"";
		} else {
			timerTextParent.SetActive( false );
			yourTimeWasTextParent.SetActive( true );
			yourTimeWasText.text = min + "\'" + sec + "\"";
		}
	}

	public void StartGame () {
		boat.isKinematic = false;
		startTime = Time.time;
		ChangeState( GameState.Gameplay );
	}
}
