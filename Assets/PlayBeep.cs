using UnityEngine;
using System.Collections;

public class PlayBeep : MonoBehaviour {

	public void Beep () {
		if (SoundtrackManager.s_instance != null)
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.beep);
	}
}