using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Keeps track of state the game is in wrt to levels and scenes 
/// </summary>
public class GameManager : MonoBehaviour {
	public static GameManager s_instance;

	public enum LevelState {MainMenu, POS, SailTrim, ApparentWind, LearnToTack, Docking, Navigation};
	public LevelState thisLevelState;

	// Game pausing event
	public delegate void GamePause( bool toggle );
	public static event GamePause PauseEvent;

	public bool isPaused = false;

	[Header("UI")]
	public CanvasGroup pauseMenu;

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

	public void PressedPause() {
		if( PauseEvent != null ) {
			PauseEvent( true );
		}

		isPaused = true;
		pauseMenu.alpha = 1f;
		pauseMenu.interactable = true;
		pauseMenu.blocksRaycasts = true;
	}

	public void ResumeGame() {
		isPaused = false;
		pauseMenu.alpha = 0f;
		pauseMenu.interactable = false;
		pauseMenu.blocksRaycasts = false;

		if( PauseEvent != null ) {
			PauseEvent( false );
		}
	}

	public void RestartModule() {
		LoadLevel( (int)thisLevelState );
	}

	public void ReturnToMainMenu() {
		LoadLevel( (int)LevelState.MainMenu );
	}
}

