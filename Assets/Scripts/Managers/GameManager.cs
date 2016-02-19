using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Keeps track of state the game is in wrt to levels and scenes 
/// </summary>
public class GameManager : MonoBehaviour {
	public static GameManager s_instance;

	public enum LevelState {OpeningCredits, MainMenu, POS, SailTrim, ApparentWind, LearnToTack, Docking, Navigation, ManOverboard, RightOfWay};
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
	public CanvasGroup loadingBarScreen;
	public Slider musicVolumeSlider;
	public Slider soundsVolumeSlider;
	public Slider loadingSlider;

	private AsyncOperation async = null;

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
		if( thisLevelState != LevelState.OpeningCredits )
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		loadingBarScreen.alpha = 1;
		StartCoroutine (LevelLoader (levelIndex));
		thisLevelState = (LevelState)levelIndex;
	

	}
	IEnumerator LevelLoader (int level) {
		async = SceneManager.LoadSceneAsync (level);
		yield return async;
		if (level == 1) {
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.music);
			SoundtrackManager.s_instance.StartCoroutine( "FadeOutOceanAudioSource" );
		}
		loadingBarScreen.alpha = 0;

	}

	#region UI Logic
	public void PressedPause() {
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		if( thisLevelState != LevelState.POS )
			Time.timeScale = 0f;

		isPaused = true;
		ToggleSlide( pauseMenu, true );
	}

	public void ResumeGame() {
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
		isPaused = false;
		ToggleSlide( pauseMenu, false );
		Time.timeScale = 1f;
	}

	public void RestartModule() {
		ConfirmationPopUp.s_instance.InitializeConfirmationPanel( "Restart Module?", ( bool confirmed ) => {
			if( confirmed ) {
				ToggleSlide( pauseMenu, false );
				LoadLevel( (int)thisLevelState );
				SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
				Time.timeScale = 1f;
			}
		});
	}

	public void ReturnToMainMenu() {
		ConfirmationPopUp.s_instance.InitializeConfirmationPanel( "Return to Main Menu?", ( bool confirmed ) => {
			if( confirmed ) {
				ToggleSlide( pauseMenu, false );
				LoadLevel( (int)LevelState.MainMenu );
				SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
				Time.timeScale = 1f;
			}
		});
	}

	public void PressedOptionsButton() {
		ToggleSlide( pauseMenu, false );
		ToggleSlide( optionsMenu, true );
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);

	}

	/// <summary>
	/// Returns to pause screen. Used when pressing the 'Close' button in the options menu.
	/// </summary>
	public void ReturnToPauseScreen() {
		ToggleSlide( optionsMenu, false );
		ToggleSlide( pauseMenu, true );
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);

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

		if (async != null && !async.isDone) {
			loadingSlider.value = async.progress;
		}

		if( Input.GetKeyDown( KeyCode.Space ) )
			PressedPause();
	}
}

