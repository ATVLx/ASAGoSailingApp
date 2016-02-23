using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	public GameObject startPanel;
	public GameObject modulesMenu;

	private Animator modulesMenuAnimator;
	private bool restartTimerHasStarterd = false;

	void Awake() {
		modulesMenuAnimator = modulesMenu.GetComponent<Animator>();
	}

	void OnLevelWasLoaded() {
		if (GameManager.s_instance.isReturningFromModule) {
			GameManager.s_instance.isReturningFromModule = false;

			//modulesMenuAnimator = modulesMenu.GetComponent<Animator>();
			modulesMenuAnimator.SetTrigger ("menuTrigger");

			InstructionsPanel modulesMenuInstructionsPanel = modulesMenu.GetComponent<InstructionsPanel> ();

			/* Order of levels as slides: 
			 * 0 POS 
			 * 1 Apparent Wind
			 * 2 Trim
			 * 3 Tack
			 * 4 RoW
			 * 5 Dock
			 * 6 MOB
			 * 7 Nav */
			int goToPanelIndex = 0;
			switch( GameManager.s_instance.lastLevelState ) {
			case GameManager.LevelState.POS:
				goToPanelIndex = 0;
				break;
			case GameManager.LevelState.SailTrim:
				goToPanelIndex = 2;
				break;
			case GameManager.LevelState.ApparentWind:
				goToPanelIndex = 1;
				break;
			case GameManager.LevelState.LearnToTack:
				goToPanelIndex = 3;
				break;
			case GameManager.LevelState.Docking:
				goToPanelIndex = 5;
				break;
			case GameManager.LevelState.Navigation:
				goToPanelIndex = 7;
				break;
			case GameManager.LevelState.ManOverboard:
				goToPanelIndex = 6;
				break;
			case GameManager.LevelState.RightOfWay:
				goToPanelIndex = 4;
				break;
			}
			GameObject goToPanel = modulesMenu.transform.GetChild(0).GetChild( goToPanelIndex ).gameObject;
			modulesMenuInstructionsPanel.GoToPanel (goToPanel);

			startPanel.SetActive (false);
		}
	}

	void Update () {
		// TODO In the case that this is ever ported to something that doens't use touch for input, add a case for keys/mouse
		if (Input.touches.Length > 0 && restartTimerHasStarterd == true) {
			restartTimerHasStarterd = false;
		}

		if ( restartTimerHasStarterd == false && modulesMenuAnimator.GetCurrentAnimatorStateInfo (0).IsName ("onScreen")) {
			StopAllCoroutines ();
			restartTimerHasStarterd = true;
			StartCoroutine (GoBackToStartScreen ());
		}
	}

	private IEnumerator GoBackToStartScreen() {
		yield return new WaitForSeconds (60f);
		modulesMenuAnimator.SetTrigger ("menuTrigger");
		startPanel.SetActive (true);

		yield return new WaitForSeconds (2f);
		restartTimerHasStarterd = false;
		StopCoroutine ("GoBackToStartScreen");
	}

	#region LoadLevel methods
	public void LoadPOSModule() {
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.oceanBreeze);

		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.POS );
	}
	public void LoadApparentWindModule() {
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.oceanBreeze);

		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.ApparentWind );
	}
	public void LoadTrimmingModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.oceanBreeze);

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.SailTrim );
	}
	public void LoadMOBModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.oceanBreeze);

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.ManOverboard );
	}
	public void LoadDockingModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.oceanBreeze);

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.Docking );
	}
	public void LoadROWModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.oceanBreeze);

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.RightOfWay );
	}
	public void LoadTackingModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.oceanBreeze);

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.LearnToTack );
	}
	public void LoadNavigationModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.oceanBreeze);

		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.Navigation );
	}
	#endregion
}
