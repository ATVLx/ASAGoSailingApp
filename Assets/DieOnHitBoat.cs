using UnityEngine;
using System.Collections;

public class DieOnHitBoat : MonoBehaviour {
//	[SerializeField] GameObject explosion;
	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter( Collider col ) {
		if (col.tag == "Player") {
//			Instantiate (explosion,transform.position, Quaternion.identity);
			Destroy (gameObject);
			if (SoundtrackManager.s_instance != null)
				SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.laser);
		}
			
	}
}
