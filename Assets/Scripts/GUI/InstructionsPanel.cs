using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstructionsPanel : MonoBehaviour {

	public RectTransform slidesParent;
	public RectTransform finalSlide;
	public Button prevButton, nextButton;
	public Text pageNumberText;

	private CanvasGroup[] slides;
	private int numSlides;
	private int currentSlide = 0;

	void Start () {
		finalSlide.SetAsLastSibling();
		slides = slidesParent.GetComponentsInChildren<CanvasGroup>();
		numSlides = slides.Length;

		foreach( CanvasGroup slide in slides ) {
			ToggleSlide( slide, false );
		}
		ToggleSlide( slides[0], true );

		UpdatePageNumber();
		UpdateButtons();
	}

	/// <summary>
	/// Toggles the slide on or off.
	/// </summary>
	/// <param name="cG">Reference to CanvasGroup instance.</param>
	/// <param name="turnOn">If set to <c>true</c> toggles CanvasGroup on.</param>
	private void ToggleSlide( CanvasGroup cG, bool turnOn ) {
		cG.gameObject.SetActive( turnOn );
//		if( turnOn ) {
//			cG.alpha = 1f;
//			cG.interactable = true;
//			cG.blocksRaycasts = true;
//		} else {
//			cG.alpha = 0f;
//			cG.interactable = false;
//			cG.blocksRaycasts = false;
//		}
	}

	private void UpdatePageNumber() {
		pageNumberText.text = (currentSlide+1).ToString() + " / " + slides.Length;
	}

	public void ClickedNextPage() {
		ToggleSlide( slides[currentSlide], false );
		currentSlide++;
		ToggleSlide( slides[currentSlide], true );
		UpdatePageNumber();
		UpdateButtons();
		if (SoundtrackManager.s_instance != null)SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
	}

	public void ClickedPreviousPage() {
		ToggleSlide( slides[currentSlide], false );
		currentSlide--;
		ToggleSlide( slides[currentSlide], true );
		UpdatePageNumber();
		UpdateButtons();
		if (SoundtrackManager.s_instance != null)SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);

	}

	public void ClickedDoneButton() {
		if (SoundtrackManager.s_instance != null)SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);

		// ~NOTE~ We should create a generic way for this to get back to the game. Perhaps a ModuleManager base class?
		if( GameManager.s_instance != null ) {
			switch( GameManager.s_instance.thisLevelState ) {
			case GameManager.LevelState.ApparentWind:
				ApparentWindModuleManager.s_instance.ChangeState( ApparentWindModuleManager.GameState.Playing );
				break;

			case GameManager.LevelState.Docking:
				DockingManager.s_instance.ClosedInstructionsPanel();
				break;

			case GameManager.LevelState.LearnToTack:
				TackManager.s_instance.StartGame();
				break;

			case GameManager.LevelState.ManOverboard:
				MOBManager.s_instance.ClosedInstructionsPanel();
				break;

			case GameManager.LevelState.Navigation:
				NavManager.s_instance.ChangeState( NavManager.GameState.Gameplay );
				break;

			case GameManager.LevelState.POS:
				POSModuleManager.s_instance.ClickStart();
				break;

			case GameManager.LevelState.RightOfWay:
				RightOfWayManager.s_instance.StartGame();
				break;

			case GameManager.LevelState.SailTrim:
				TrimManager.s_instance.BeginTutorial();
				break;
			}
		}
		gameObject.SetActive( false );
	}

	/// <summary>
	/// Updates buttons based on what page we are on.
	/// </summary>
	public void UpdateButtons() {
		// Check if we're on the first slide
		if( currentSlide == 0 )
			prevButton.gameObject.SetActive( false );
		else
			prevButton.gameObject.SetActive( true );

		// Check if we are on the last slide
		if( currentSlide == slides.Length-1 ) {
			nextButton.gameObject.SetActive( false );
			pageNumberText.enabled = false;
		} else {
			nextButton.gameObject.SetActive( true );
			pageNumberText.enabled = true;
		}
	}
}
