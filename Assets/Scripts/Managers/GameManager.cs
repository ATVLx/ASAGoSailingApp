using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Keeps track of state the game is in wrt to levels and scenes 
/// </summary>
public class GameManager : MonoBehaviour {
	public static GameManager s_instance;

	public enum LevelState {OpeningCredits, MainMenu, POS, SailTrim, ApparentWind, LearnToTack, Docking, Navigation};
	public LevelState thisLevelState;

	// Game pausing event
	public delegate void PauseEvent( bool toggle );
	public static event PauseEvent TogglePause;

	// Volume change events
	public delegate void MusicVolumeChangeEvent( float value );
	public static event MusicVolumeChangeEvent MusicVolumeChanged;
	public delegate void SoundsVolumeChangeEvent( float value );
	public static event SoundsVolumeChangeEvent SoundsVolumeChanged;

	[System.NonSerialized]
	public bool isPaused = false;

	[Header("UI")]
	public CanvasGroup pauseMenu;
	public CanvasGroup optionsMenu;
	public Slider musicVolumeSlider;
	public Slider soundsVolumeSlider;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
			DontDestroyOnLoad( this.gameObject );
		}
		else {
			Debug.LogWarning( "Deleting " + gameObject.name + " because it is a duplicate GameManager" );
			Destroy(gameObject);
		}
	}
		
	public void LoadLevel(int levelIndex) {
		thisLevelState = (LevelState)levelIndex;
		SceneManager.LoadScene (levelIndex);
	}

	#region UI Logic
	public void PressedPause() {
		if( TogglePause != null ) {
			TogglePause( true );
		}

		isPaused = true;
		ToggleSlide( pauseMenu, true );
	}

	public void ResumeGame() {
		isPaused = false;
		ToggleSlide( pauseMenu, false );

		if( TogglePause != null ) {
			TogglePause( false );
		}
	}

	public void RestartModule() {
		ConfirmationPopUp.s_instance.InitializeConfirmationPanel( "Restart Module?", ( bool confirmed ) => {
			if( confirmed ) {
				ToggleSlide( pauseMenu, false );
				LoadLevel( (int)thisLevelState );
			}
		});
	}

	public void ReturnToMainMenu() {
		ConfirmationPopUp.s_instance.InitializeConfirmationPanel( "Return to Main Menu?", ( bool confirmed ) => {
			if( confirmed ) {
				ToggleSlide( pauseMenu, false );
				LoadLevel( (int)LevelState.MainMenu );
			}
		});
	}

	public void PressedOptionsButton() {
		ToggleSlide( pauseMenu, false );
		ToggleSlide( optionsMenu, true );
	}

	/// <summary>
	/// Returns to pause screen. Used when pressing the 'Close' button in the options menu.
	/// </summary>
	public void ReturnToPauseScreen() {
		ToggleSlide( optionsMenu, false );
		ToggleSlide( pauseMenu, true );
	}

	public void UpdatedMusicVolume() {
		if( MusicVolumeChanged != null ) {
			MusicVolumeChanged( musicVolumeSlider.value );
		}
	}

	public void UpdatedSoundsVolume() {
		if( SoundsVolumeChanged != null ) {
			SoundsVolumeChanged( soundsVolumeSlider.value );
		}
	}

	/// <summary>
	/// Toggles the slide on or off.
	/// </summary>
	/// <param name="cG">Reference to CanvasGroup instance.</param>
	/// <param name="turnOn">If set to <c>true</c> toggles CanvasGroup on.</param>
	private void ToggleSlide( CanvasGroup cG, bool turnOn ) {
		if( turnOn ) {
			cG.alpha = 1f;
			cG.interactable = true;
			cG.blocksRaycasts = true;
		} else {
			cG.alpha = 0f;
			cG.interactable = false;
			cG.blocksRaycasts = false;
		}
	}
	#endregion

	void Update() {
		if( Input.GetKeyDown( KeyCode.Space ) )
			PressedPause();
	}
}

