using UnityEngine;
using System.Collections;

public class SoundtrackManager : MonoBehaviour {
	
	public AudioSource oceanBreeze; //soundtrack files
	public static SoundtrackManager s_instance;

	public AudioSource correct, wrong, gybe, bell, crash, beep, laser, waterWoosh;
	void Awake () {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy(gameObject);
		}		
		DontDestroyOnLoad (gameObject); //persist through scenes
	}
	
	IEnumerator FadeOutAudioSource(AudioSource x) { //call from elsewhere
		while (x.volume > 0.0f) {					//where x is sound track file
			x.volume -= 0.01f;
			yield return new WaitForSeconds(0.03f);
		}
		x.Stop ();
	}
	
	public void PlayAudioSource(AudioSource x, float volume = 1) { //call from elsewhere
		x.volume = volume;
		x.Play ();
	}
}