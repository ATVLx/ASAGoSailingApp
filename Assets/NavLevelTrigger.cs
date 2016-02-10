using UnityEngine;
using System.Collections;

public class NavLevelTrigger : MonoBehaviour {

	public int triggerNumber;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			if (triggerNumber == 0) {
				NavManager.s_instance.HasReachedHarbor ();
				if (SoundtrackManager.s_instance != null)
					SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.bell);
			} else if (triggerNumber == 1) {
				NavManager.s_instance.WinNavModule ();
				if (SoundtrackManager.s_instance != null)
					SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.bell);

			}
		}
	}
}
