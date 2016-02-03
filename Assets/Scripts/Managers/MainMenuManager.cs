using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	#region LoadLevel methods
	public void LoadPOSModule() {
		StartCoroutine (SoundtrackManager.s_instance.FadeOutAudioSource (SoundtrackManager.s_instance.music));
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.POS );
	}
	public void LoadApparentWindModule() {
		StartCoroutine (SoundtrackManager.s_instance.FadeOutAudioSource (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.ApparentWind );
	}
	public void LoadTrimmingModule() {
		StartCoroutine (SoundtrackManager.s_instance.FadeOutAudioSource (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.SailTrim );
	}
	public void LoadMOBModule() {
		StartCoroutine (SoundtrackManager.s_instance.FadeOutAudioSource (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.ManOverboard );
	}
	public void LoadDockingModule() {
		StartCoroutine (SoundtrackManager.s_instance.FadeOutAudioSource (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.Docking );
	}
	public void LoadROWModule() {
		StartCoroutine (SoundtrackManager.s_instance.FadeOutAudioSource (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.RightOfWay );
	}
	public void LoadTackingModule() {
		StartCoroutine (SoundtrackManager.s_instance.FadeOutAudioSource (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.LearnToTack );
	}
	public void LoadNavigationModule() {
		StartCoroutine (SoundtrackManager.s_instance.FadeOutAudioSource (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.Navigation );
	}
	#endregion
}
