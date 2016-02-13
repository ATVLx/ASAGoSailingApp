using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TackManager : MonoBehaviour {
	enum TackState {tackingIntro, jibingIntro, gameplay, reset, win};
	TackState curState;
	public static TackManager s_instance;
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
	public GameObject tackingInstructionsUI;
	public GameObject jibingInformationUI;
	public GameObject jibingInstructionsUI;
	public GameObject gameplayUI;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	void Start() {
		curState = TackState.tackingIntro;
	}

	void Update () {
		switch (curState) {
		case TackState.tackingIntro:
		case TackState.jibingIntro:
			{
				if (switchToGamePlay) {
					playerBoat.GetComponent<Rigidbody> ().isKinematic = false;
					switchToGamePlay = false;
					curState = TackState.gameplay;
				}
				break;
			}
		case TackState.gameplay:
			{
				if (switchToReset) {
					switchToReset = false;
					curState = TackState.reset;
				}
				break;
			}
		case TackState.reset:
			{
				//				StartCoroutine ("PauseBoats");
				if (switchToGamePlay) {
					switchToGamePlay = false;
					curState = TackState.gameplay;
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
		if (curState == TackState.gameplay) {
			win.StartFadeOut ();
			switchToReset = true;
			StartCoroutine ("WinReset");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.correct);

		}
	}

	public void Fail(){
		if (curState == TackState.gameplay) {
			lose.StartFadeOut ();
			switchToReset = true;
			StopAllCoroutines ();
			StartCoroutine ("FailReset");
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.wrong);

		}
	}

	public IEnumerator Land() {
		if (curState == TackState.gameplay) {
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
			// UI
			gameplayUI.SetActive( false );
			jibingInformationUI.SetActive( true );
			playerBoat.GetComponent<Rigidbody>().isKinematic = true;
			curState = TackState.jibingIntro;

//			yield return new WaitForSeconds (2f);
//			CameraMain ();
//			switchToGamePlay = true;
		} else {
			curState = TackState.win;

			CongratulationsPopUp.s_instance.InitializeCongratulationsPanel( "Tacking" );
		}
	}

	public void ClosedInstructionsPanel() {
		switch( curState ) {
		case TackState.tackingIntro:
			{
				tackingInstructionsUI.SetActive( false );
				gameplayUI.SetActive( true );
				StartGame();
				break;
			}
		case TackState.jibingIntro:
			{
				jibingInstructionsUI.SetActive( false );
				gameplayUI.SetActive( true );
				StartGame();
				break;
			}
		}
	}
}