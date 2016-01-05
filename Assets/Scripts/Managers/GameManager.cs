using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/*
	This class keeps track of state the game is in wrt to levels and scenes 
*/

public class GameManager : MonoBehaviour {

	public enum LevelState {MainMenu, POS, SailTrim, ApparentWind, LearnToTack, Docking, Navigation};
	LevelState thisLevelState;

	public static GameManager s_instance;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
			DontDestroyOnLoad( this.gameObject );
		}
		else {
			Destroy(gameObject);
		}
	}
		
	public void LoadLevel(int levelIndex) {
		thisLevelState = (LevelState)levelIndex;
		SceneManager.LoadScene (levelIndex);
	}

}

