using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RightOfWayManager : MonoBehaviour {

	enum ROWState {briefing, instructions, gameplay, reset};
	ROWState curState;
	[SerializeField]
	GameObject AIboat, Player, MotorBoat;
	[SerializeField]
	Transform windward, leeward, port, starboard, overrun;
	[SerializeField]
	int scenario = 0;
	[SerializeField]
	Fader failure;
	[SerializeField]
	Fader success;
	[SerializeField]
	Camera cam1, cam2;

	[SerializeField] GameObject congratsText;
	bool isFailing;
	bool hasStarted;
	float resetDelay = 10f;
	public static RightOfWayManager s_instance;
	public bool switchToReset, switchToGamePlay;

	[Header("UI")]
	[SerializeField]
	Slider boomSlider;
	[SerializeField]
	Text hintText;
	public GameObject briefingUI, instructionsUI, gamePlayUI;
	public CanvasGroup windward_LeewardStandOnCG, windward_LeewardGiveWayCG, port_StarboardGiveWayCG, port_StarboardStandOnCG, powerboat_SailboatCG;

	private CanvasGroup currentGraphicCG;

	private float scenarioWaitTime = 0f;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	void Start () {
		curState = ROWState.briefing;
		ToggleBoatMovement (false);
	}

	// Update is called once per frame
	void Update () {
		print (scenario + " SCENARIO");
		switch (curState) {
		case ROWState.instructions:
			{
				if (switchToGamePlay) {
					switchToGamePlay = false;
					curState = ROWState.gameplay;
				}
				break;
			}
		case ROWState.gameplay:
			{
				if (switchToReset) {
					switchToReset = false;
					curState = ROWState.reset;
				}
				break;
			}
		case ROWState.reset:
			{
				StartCoroutine ( ShowSituation() );
				curState = ROWState.gameplay;
				break;
			}
		}
	}
	public void StartGame() {
		switchToGamePlay = true;
		gamePlayUI.SetActive (true);
		StartCoroutine ( ShowSituation() );
	}

	void ToggleCameras (){
		cam1.enabled = !cam1.isActiveAndEnabled;
		cam2.enabled = !cam2.isActiveAndEnabled;

	}

	IEnumerator ShowSituation () {
		if (!hasStarted) {
			hasStarted = true;
		}
		else {
			yield return new WaitForSeconds (2f);
		}
		ToggleCameras ();
		isFailing = false;
		ShowGraphics ();
		ToggleBoatMovement (false);
		yield return WaitForSecondsOrTap( resetDelay );
		SetPositions ();
		currentGraphicCG.alpha = 0f;
		gamePlayUI.SetActive( true );
		ToggleCameras ();
		ToggleBoatMovement (true);
	}

	void ToggleBoatMovement (bool thisBool) {
		AIboat.GetComponent<Rigidbody> ().isKinematic = !thisBool;
		Player.GetComponent<Rigidbody> ().isKinematic = !thisBool;
		MotorBoat.GetComponent<Rigidbody> ().isKinematic = !thisBool;

	}

	//TODO make boats turn after X seconds on each module
	void ShowGraphics() {
		
		switch (scenario) {
		case 0:
			{
				currentGraphicCG = windward_LeewardGiveWayCG;
				break;
			}
		case 1:
			{
				currentGraphicCG = windward_LeewardStandOnCG;
				break;
			}
		case 2:
			{
				currentGraphicCG = port_StarboardStandOnCG;
				break;
			}
		case 3:
			{
				currentGraphicCG = port_StarboardGiveWayCG;
				break;
			}
		case 4:
			{
				currentGraphicCG = powerboat_SailboatCG;
				break;
			}
		}
		gamePlayUI.SetActive( false );
		currentGraphicCG.alpha = 1f;
	}

	void SetPositions() {
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.bell);

		print ("SET POS" + scenario);
		switch (scenario) {
		case 0:
			{
				hintText.text = "Remember: You give way to them.";
				boomSlider.value = 40f;
				AIboat.GetComponent<AIBoat> ().SetTack (false);
				Player.transform.position = windward.position;
				AIboat.transform.position = leeward.position;
				Player.transform.rotation = windward.rotation;
				AIboat.transform.rotation = leeward.rotation;

				currentGraphicCG = windward_LeewardGiveWayCG;
				break;
			}
		case 1:
			{
				hintText.text = "Remember: They give way to you.";
				StartCoroutine ("level2");
				boomSlider.value = 15f;
				Player.transform.rotation = leeward.rotation;
				Player.transform.position = leeward.position;
				AIboat.transform.rotation = windward.rotation;
				AIboat.transform.position = windward.position;

				currentGraphicCG = windward_LeewardStandOnCG;
				break;
			}
		case 2:
			{
				hintText.text = "Remember: They give way to you.";
				StartCoroutine ("level3");

				boomSlider.value = 20f;
				Player.transform.position = starboard.position;
				AIboat.transform.position = port.position;
				Player.transform.rotation = starboard.rotation;
				AIboat.transform.rotation = port.rotation;

				currentGraphicCG = port_StarboardStandOnCG;
				break;
			}
		case 3:
			{
				hintText.text = "Remember: You give way to them.";
				boomSlider.value = 14.5f;
				Player.transform.position = port.position;
				Player.transform.rotation = port.rotation;
				AIboat.transform.position = starboard.position;
				AIboat.transform.rotation = starboard.rotation;

				currentGraphicCG = port_StarboardGiveWayCG;
				break;
			}
		case 4:
			{
				hintText.text = "Get the heck out of the way!";
				AIboat.SetActive (false);
				Player.transform.position = starboard.position;
				MotorBoat.transform.position = overrun.position;
				Player.transform.rotation = starboard.rotation;
				MotorBoat.transform.rotation = overrun.rotation;
				MotorBoat.GetComponent<EvilYacht> ().isMoving = true;

				currentGraphicCG = powerboat_SailboatCG;
				break;
			}
		}

	}

	IEnumerator level2 () {
		print ("LEVEL2.1");
		yield return new WaitForSeconds (3f);
		print ("LEVEL2.2");

		AIboat.GetComponent<AIBoat> ().SetSteering (true, true);
		yield return new WaitForSeconds (1.5f);
		print ("LEVEL2.3");

		AIboat.GetComponent<AIBoat> ().SetSteering (false, false);
	}
	IEnumerator level3 () {
		yield return new WaitForSeconds (5.5f);
		AIboat.GetComponent<AIBoat> ().SetSteering (true, false);
		yield return new WaitForSeconds (3f);
		AIboat.GetComponent<AIBoat> ().SetSteering (false, false);
	}

	/// <summary>
	/// Fades the current CG alpha.
	/// </summary>
	/// <returns>The current CG alpha.</returns>
	/// <param name="fadeOut">If set to <c>true</c> fade out. If false, will fade in.</param>
	IEnumerator FadeCurrentCGAlpha( bool fadeOut = true ) {
		float lerpTime = 0.05f;
		float startTime = Time.time;
		float startAlpha = ( fadeOut ) ? 1f : 0f;
		float endAlpha = ( fadeOut ) ? 0f : 1f;

		while( lerpTime >= Time.time - startTime ) {
			currentGraphicCG.alpha = Mathf.Lerp( startAlpha, endAlpha, ( Time.time - startTime )/lerpTime );
			yield return null;
		}

		currentGraphicCG.alpha = endAlpha;
	}


	void WinModule() {
		hintText.text = "";
		gamePlayUI.SetActive (false);
		congratsText.SetActive (true);
		CongratulationsPopUp.s_instance.InitializeCongratulationsPanel( "Right of Way" );
	}

	public void WinScenario() {

		if (!isFailing) {
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.correct);

			isFailing = true;
			success.StartFadeOut ();
			scenario++;
			if (scenario < AIboat.GetComponent<AIBoat> ().scenarioTriggers.Count) {
				AIboat.GetComponent<AIBoat> ().scenarioTriggers [scenario-1].SetActive (false);
				AIboat.GetComponent<AIBoat> ().scenarioTriggers [scenario].SetActive (true);
				switchToReset = true;
			}
			else if (scenario == AIboat.GetComponent<AIBoat> ().scenarioTriggers.Count) {
				switchToReset = true;
			}
			else {
				WinModule ();
			}

		}
	}

	public void Fail () {

		if (!isFailing) {
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.wrong);
			isFailing = true;
			failure.StartFadeOut ();
			switchToReset = true;
		}
	}

	public void ClosedInstructionsPanel() {
		switch( curState ) {
		case ROWState.briefing:
			instructionsUI.SetActive( true );
			curState = ROWState.instructions;
			break;
		case ROWState.instructions:
			StartGame();
			break;
		}
	}

	private IEnumerator WaitForSecondsOrTap( float newWaitTime ) {
		scenarioWaitTime = newWaitTime;

		while( scenarioWaitTime > 0f ) {
			scenarioWaitTime -= Time.deltaTime;
			yield return null;
		}
	}

	/// <summary>
	/// Method called when the player taps to exit the 
	/// </summary>
	public void SkippedScenarioIntro() {
		scenarioWaitTime = 0f;
		SoundtrackManager.s_instance.beep.Play();
	}
}
