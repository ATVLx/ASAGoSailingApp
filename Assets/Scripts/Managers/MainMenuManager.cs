using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	#region LoadLevel methods
	public void LoadPOSModule() {
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.POS );
	}
	public void LoadApparentWindModule() {
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.ApparentWind );
	}
	public void LoadTrimmingModule() {
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.SailTrim );
	}
	public void LoadMOBModule() {
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.ManOverboard );
	}
	public void LoadDockingModule() {
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.Docking );
	}
	public void LoadROWModule() {
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.RightOfWay );
	}
	public void LoadTackingModule() {
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.LearnToTack );
	}
	public void LoadNavigationModule() {
		GameManager.s_instance.LoadLevel( (int)GameManager.LevelState.Navigation );
	}
	#endregion
}
