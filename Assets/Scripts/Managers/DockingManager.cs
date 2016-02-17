using UnityEngine;
using System.Collections;

public class DockingManager : MonoBehaviour {
	enum DockingState {briefing, instructions, gameplay, reset, win};
	DockingState curState;
	public static DockingManager s_instance;
	[SerializeField]
	GameObject playerBoat;
	[SerializeField]
	GameObject setup1, setup2;
	bool isFailing = false;
	bool switchToGamePlay, switchToReset;
	[SerializeField]
	Fader win,lose;
	[SerializeField]
	Camera overhead, main;

	[SerializeField]
	Transform setup1transform, setup2transform;

	[Header("UI")]
	public GameObject briefingUI;
	public GameObject instructionsUI;
	public GameObject gameplayUi;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	void Start () {
		curState = DockingState.briefing;
	}

	IEnumerator HackyBSFix() {
		//fixes the provlem where you win when you lose
		yield return new WaitForSeconds (1f);
		StopAllCoroutines ();

	}

	void Update () {
		switch (curState) {
		case DockingState.instructions:
			{
				if (switchToGamePlay) {
					playerBoat.GetComponent<Rigidbody> ().isKinematic = false;
					switchToGamePlay = false;
					curState = DockingState.gameplay;
				}
				break;
			}
		case DockingState.gameplay:
			{
				if (switchToReset) {
					switchToReset = false;
					curState = DockingState.reset;
				}
				break;
			}
		case DockingState.reset:
			{
				//				StartCoroutine ("PauseBoats");
				if (switchToGamePlay) {
					switchToGamePlay = false;
					curState = DockingState.gameplay;
					StartCoroutine ("HackingBSFix");
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
		if (curState == DockingState.gameplay) {
			win.StartFadeOut ();
			switchToReset = true;
			if (SoundtrackManager.s_instance != null)
				SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.correct);
			StartCoroutine ("WinReset");
		}
	}

	public void Fail(){
		if (curState == DockingState.gameplay && isFailing == false) {
			isFailing = true;
			lose.StartFadeOut ();
			if (SoundtrackManager.s_instance != null)
				SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.wrong);
			switchToReset = true;
			StopAllCoroutines ();
			StartCoroutine ("FailReset");
		}
	}

	public IEnumerator Land() {
		if (curState == DockingState.gameplay&&!isFailing) {
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
		isFailing = false;


	}

	IEnumerator WinReset () {
		CameraOverhead ();
		if (SoundtrackManager.s_instance != null)
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.bell);
		yield return new WaitForSeconds (3f);
		if (setup2.activeSelf == false) {
			setup2.SetActive (true);
			setup1.SetActive (false);
			playerBoat.transform.position = setup2transform.position;
			playerBoat.transform.rotation = setup2transform.rotation;
			yield return new WaitForSeconds (2f);
			CameraMain ();
			switchToGamePlay = true;


		} else {
			curState = DockingState.win;
			if (SoundtrackManager.s_instance != null)
				SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.bell);
			CongratulationsPopUp.s_instance.InitializeCongratulationsPanel( "Docking" );
		}
	}

	public void ClosedInstructionsPanel() {
		switch( curState ) {
		case DockingState.briefing:
			briefingUI.SetActive( false );
			instructionsUI.SetActive( true );
			curState = DockingState.instructions;
			break;

		case DockingState.instructions:
			instructionsUI.SetActive( false );
			gameplayUi.SetActive( true );
			StartGame();
			break;
		}
	}
}
