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
		switch( NavManager.s_instance.gameState ) {
		case NavManager.GameState.Idle:
			break;
		case NavManager.GameState.Review:
			break;
		case NavManager.GameState.Instructions:
			break;
		case NavManager.GameState.CameraPan:
			break;
		case NavManager.GameState.Gameplay:
			//directionalArrow.transform.LookAt(navigationPoints[currNavPoint].transform);
			currTarget.text = "Destination: " + NavManager.s_instance.navigationPoints[NavManager.s_instance.currNavPoint].name;
			timeText.text = "Elapsed time: " + NavManager.s_instance.elapsedTime.ToString("F2") + "s";
			break;
		case NavManager.GameState.Win:
			break;
		case NavManager.GameState.Lose:
			break;
		}
	}

	/// <summary>
	/// Updates the GUI to match NavManager's(GameManager) state.
	/// </summary>
	public void UpdateState() {
		switch (NavManager.s_instance.gameState) {
		case NavManager.GameState.Idle :
			break;
		case NavManager.GameState.Review :
			//NavBoatControl.s_instance.arrow.SetActive(false);
			idlePage.SetActive(false);
			reviewPage.SetActive(true);
			break;
		case NavManager.GameState.Instructions :
			reviewPage.SetActive(false);
			instructionsPage.SetActive(true);
			break;
		case NavManager.GameState.CameraPan:
			instructionsPage.SetActive(false);
			break;
		case NavManager.GameState.Gameplay :
			instructionsPage.SetActive(false);
			//NavBoatControl.s_instance.arrow.SetActive(true);
			gamePlayPage.SetActive(true);
			break;
		case NavManager.GameState.Win :
			//NavBoatControl.s_instance.arrow.SetActive(false);
			//GameObject.FindGameObjectWithTag("arrow").SetActive(false);
			//directionalArrow.SetActive(false);
			ratingObjects[NavManager.s_instance.rating].SetActive(true);
			break;
		}
	}
}
