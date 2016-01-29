using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {

	public void PressedPause() {
		if( GameManager.s_instance != null ) {
			GameManager.s_instance.PressedPause();
		} else {
			Debug.LogError( "Pressing Pause requirees an instance of GameManager in the scene. No GameManager exists in this scene." );
		}
	}
}
