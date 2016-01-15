using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RightOfWayManager : MonoBehaviour {

	enum ROWState {intro, gameplay, reset};
	ROWState curState = ROWState.intro;
	[SerializeField]
	GameObject AIboat, Player, MotorBoat;
	[SerializeField]
	Slider sailtrim;
	[SerializeField]
	Transform windward, leeward, port, starboard, overrun;
	[SerializeField]
	int scenario = 0;
	[SerializeField]
	Fader failure;
	[SerializeField]
	Fader success;

	bool isFailing;
	public static RightOfWayManager s_instance;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	public bool switchToReset, switchToGamePlay;
	void Start () {
		ToggleBoatMovement (false);
	}

	// Update is called once per frame
	void Update () {
		switch (curState) {
		case ROWState.intro:
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
				StartCoroutine ("PauseBoats");
				curState = ROWState.gameplay;
				break;
			}
		}
	}
	public void StartGame() {
		switchToGamePlay = true;
		ToggleBoatMovement (true);
		SetPositions ();
	}

	void ToggleBoatMovement (bool thisBool) {
		AIboat.GetComponent<Rigidbody> ().isKinematic = !thisBool;
		Player.GetComponent<Rigidbody> ().isKinematic = !thisBool;
		MotorBoat.GetComponent<Rigidbody> ().isKinematic = !thisBool;

	}

	//TODO make boats turn after X seconds on each module
	void SetPositions() {
		switch (scenario) {
		case 0:
			{
				sailtrim.value = 40f;
				AIboat.GetComponent<AIBoat> ().SetTack (false);
				Player.transform.position = windward.position;
				AIboat.transform.position = leeward.position;
				Player.transform.rotation = windward.rotation;
				AIboat.transform.rotation = leeward.rotation;
				break;
			}
		case 1:
			{
				StartCoroutine ("level2");
				sailtrim.value = 15f;
				Player.transform.rotation = leeward.rotation;
				Player.transform.position = leeward.position;
				AIboat.transform.rotation = windward.rotation;
				AIboat.transform.position = windward.position;
				break;
			}
		case 2:
			{
				StartCoroutine ("level3");

				sailtrim.value = 20f;
				Player.transform.position = starboard.position;
				AIboat.transform.position = port.position;
				Player.transform.rotation = starboard.rotation;
				AIboat.transform.rotation = port.rotation;
				break;
			}
		case 3:
			{
				sailtrim.value = 14.5f;
				Player.transform.position = port.position;
				Player.transform.rotation = port.rotation;
				AIboat.transform.position = starboard.position;
				AIboat.transform.rotation = starboard.rotation;
				break;
			}
		case 4:
			{
				AIboat.SetActive (false);
				Player.transform.position = starboard.position;
				MotorBoat.transform.position = overrun.position;
				Player.transform.rotation = starboard.rotation;
				MotorBoat.transform.rotation = overrun.rotation;
				MotorBoat.GetComponent<EvilYacht> ().isMoving = true;
				break;
			}
		
		}
	}

	IEnumerator level2 () {
		yield return new WaitForSeconds (3f);
		AIboat.GetComponent<AIBoat> ().SetSteering (true, true);
		yield return new WaitForSeconds (1.5f);
		AIboat.GetComponent<AIBoat> ().SetSteering (false, false);
	}
	IEnumerator level3 () {
		yield return new WaitForSeconds (5.5f);
		AIboat.GetComponent<AIBoat> ().SetSteering (true, false);
		yield return new WaitForSeconds (3f);
		AIboat.GetComponent<AIBoat> ().SetSteering (false, false);
	}

	IEnumerator PauseBoats () {

		yield return new WaitForSeconds (2f);
		ToggleBoatMovement (false);
		yield return new WaitForSeconds (.0001f);
		ToggleBoatMovement (true);
		SetPositions ();
		isFailing = false;

	}

	void WinModule() {

	}

	public void WinScenario() {
		if (!isFailing) {
			isFailing = true;
			success.StartFadeOut ();
			AIboat.GetComponent<AIBoat> ().scenarioTriggers [scenario].SetActive (false);
			scenario++;
			if (scenario < AIboat.GetComponent<AIBoat> ().scenarioTriggers.Count) {
				AIboat.GetComponent<AIBoat> ().scenarioTriggers [scenario].SetActive (true);
				switchToReset = true;
			} else {
				WinModule ();
			}

		}
	}

	public void Fail () {
		if (!isFailing) {
			isFailing = true;
			failure.StartFadeOut ();
			switchToReset = true;
		}
	}


}
