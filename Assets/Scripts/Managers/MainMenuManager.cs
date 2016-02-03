using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	#region LoadLevel methods
	public void LoadPOSModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.POS );
	}
	public void LoadApparentWindModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.ApparentWind );
	}
	public void LoadTrimmingModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.SailTrim );
	}
	public void LoadMOBModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.ManOverboard );
	}
	public void LoadDockingModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.Docking );
	}
	public void LoadROWModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.RightOfWay );
	}
	public void LoadTackingModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.LearnToTack );
	}
	public void LoadNavigationModule() {
		SoundtrackManager.s_instance.StartCoroutine ("FadeOutAudioSource", (SoundtrackManager.s_instance.music));

		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.Navigation );
	}
	#endregion
}
