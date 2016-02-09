using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundtrackManager : MonoBehaviour {
	
	public AudioSource oceanBreeze; //soundtrack files
	public static SoundtrackManager s_instance;
	float currentVolume =1f, musicVolume=1f;
	[SerializeField] Slider volumeSlider, musicSlider;
	public AudioSource correct, wrong, gybe, bell, crash, beep, laser, waterWoosh, music;
	void Awake () {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy(gameObject);
		}		
	}
	
	IEnumerator FadeOutAudioSource(AudioSource x) { //call from elsewhere

		while (x.volume > 0.0f) {					//where x is sound track file
			x.volume -= 0.01f;
			yield return new WaitForSeconds (.002f);

		}
		x.Stop ();
	}

	public void SetVolume() {
		currentVolume = volumeSlider.value;
	}

	public void SetMusicVolume() {
		musicVolume = musicSlider.value;
		oceanBreeze.volume = musicVolume;
		music.volume = musicVolume;
	}
	
	public void PlayAudioSource(AudioSource x) { //call from elsewhere
		x.volume = currentVolume;
		x.Play ();
	}
}