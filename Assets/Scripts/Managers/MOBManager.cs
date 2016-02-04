using UnityEngine;
using System.Collections;

public class MOBManager : MonoBehaviour {
	enum MOBState {briefing, setup1Instructions, setup2Instructions, gameplay, reset, win};
	MOBState curState;
	public static MOBManager s_instance;
	[SerializeField]
	GameObject playerBoat;
	[SerializeField]
	GameObject setup1, setup2;
	bool switchToGamePlay, switchToReset;
	[SerializeField]
	Fader win,lose;
	[SerializeField]
	Camera overhead, main;

	[SerializeField]
	Transform setup1transform, setup2transform;

	[Header("UI")]
	public GameObject briefingUI;
	public GameObject setup1InstructionsUI;
	public GameObject setup2InstructionsUI;
	public GameObject gameplayUI;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	void Update () {
		switch (curState) {
		case MOBState.setup1Instructions:
		case MOBState.setup2Instructions:
			{
				if (switchToGamePlay) {
					playerBoat.GetComponent<Rigidbody> ().isKinematic = false;
					switchToGamePlay = false;
					curState = MOBState.gameplay;
				}
				break;
			}
		case MOBState.gameplay:
			{
				if (switchToReset) {
					switchToReset = false;
					curState = MOBState.reset;
				}
				break;
			}
		case MOBState.reset:
			{
//				StartCoroutine ("PauseBoats");
				if (switchToGamePlay) {
					switchToGamePlay = false;
					curState = MOBState.gameplay;
				}
				break;
			}
		}
	}

	public void StartGame() {
		switchToGamePlay = true;
		CameraMain ();
	}

	public void WinScenario() {
		if (curState == MOBState.gameplay) {
			win.StartFadeOut ();
			switchToReset = true;
			StartCoroutine ( WinReset() );
		}
	}

	public void Fail(){
		if (curState == MOBState.gameplay) {
			lose.StartFadeOut ();
			switchToReset = true;
			StopAllCoroutines ();
			StartCoroutine ("FailReset");
		}
	}

	public IEnumerator Land() {
		if (curState == MOBState.gameplay) {
			yield return new WaitForSeconds (3f);
			WinScenario ();
		}
	}

	void CameraMain() {
		
		overhead.GetComponent<Camera> ().enabled = false;
		main.GetComponent<Camera> ().enabled = true;
	}

	void CameraOverhead() {
		overhead.GetComponent<Camera> ().enabled = true;
		main.GetComponent<Camera> ().enabled = false;
	}

	IEnumerator FailReset () {
		CameraOverhead ();
		yield return new WaitForSeconds (3f);
		CameraMain ();
		if (setup2.gameObject.activeSelf == false) {
			playerBoat.transform.position = setup1transform.position;
			playerBoat.transform.rotation = setup1transform.rotation;
		} else {
			playerBoat.transform.position = setup2transform.position;
			playerBoat.transform.rotation = setup2transform.rotation;
		}
		switchToGamePlay = true;

	}

	IEnumerator WinReset () {
		CameraOverhead ();
		yield return new WaitForSeconds (3f);
		if (setup2.activeSelf == false) {
			setup2.SetActive (true);
			setup1.SetActive (false);
			playerBoat.transform.position = setup2transform.position;
			playerBoat.transform.rotation = setup2transform.rotation;
			// Update UI state
			gameplayUI.SetActive( false );
			setup2InstructionsUI.SetActive( true );
			playerBoat.GetComponent<Rigidbody>().isKinematic = true;
			curState = MOBState.setup2Instructions;

//			yield return new WaitForSeconds (2f);
//			CameraMain ();
//			switchToGamePlay = true;
		} else {
			curState = MOBState.win;
			CongratulationsPopUp.s_instance.InitializeCongratulationsPanel( "Man Overbard" );
		}
	}

	public void ClosedInstructionsPanel() {
		switch( curState ) {
		case MOBState.briefing:
			{
				briefingUI.SetActive( false );
				setup1InstructionsUI.SetActive( true );
				curState = MOBState.setup1Instructions;
				break;
			}
		case MOBState.setup1Instructions:
			{
				setup1InstructionsUI.SetActive( false );
				gameplayUI.SetActive( true );
				StartGame();
				break;
			}
		case MOBState.setup2Instructions:
			{
				setup2InstructionsUI.SetActive( false );
				gameplayUI.SetActive( true );
				StartGame();
				break;
			}
		}
	}
}
