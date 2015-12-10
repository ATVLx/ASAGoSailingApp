using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public static GUIManager s_instance;

	public GameObject reviewPage, idlePage, instructionsPage, gamePlayPage, winPage;
	public Text currTarget;
	public Text timeText;
	public GameObject[] ratingObjects;

	void Awake() {
		if( s_instance == null )
			s_instance = this;
		else
			Destroy(this.gameObject);
	}

	void Start () {	
	}

	void Update () {
		switch( GameManager.s_instance.gameState ) {
		case GameManager.GameState.Instructions:
			break;
		case GameManager.GameState.Gameplay:
			//directionalArrow.transform.LookAt(navigationPoints[currNavPoint].transform);
			currTarget.text = "Destination: " + GameManager.s_instance.navigationPoints[GameManager.s_instance.currNavPoint].name;
			timeText.text = "Elapsed time: " + GameManager.s_instance.elapsedTime.ToString("F2") + "s";
			break;
		case GameManager.GameState.Win:
			break;
		case GameManager.GameState.Lose:
			break;
		}
	}

	/// <summary>
	/// Updates the GUI to match NavManager's(GameManager) state.
	/// </summary>
	public void UpdateState() {
		switch (GameManager.s_instance.gameState) {
		case GameManager.GameState.Instructions :
			idlePage.SetActive(false);
			instructionsPage.SetActive(true);
			break;
		case GameManager.GameState.Gameplay :
			instructionsPage.SetActive(false);
			//NavBoatControl.s_instance.arrow.SetActive(true);
			gamePlayPage.SetActive(true);
			break;
		case GameManager.GameState.Win :
			//NavBoatControl.s_instance.arrow.SetActive(false);
			//GameObject.FindGameObjectWithTag("arrow").SetActive(false);
			//directionalArrow.SetActive(false);
			ratingObjects[GameManager.s_instance.rating].SetActive(true);
			break;
		}
	}
}
