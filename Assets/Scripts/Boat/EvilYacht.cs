using UnityEngine;
using System.Collections;

public class EvilYacht : MonoBehaviour {
	public bool isMoving;
	public float speed = 100000f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isMoving) {
			GetComponent<Rigidbody> ().AddForce (transform.forward * speed);
		}
	}

	void OnCollisionEnter() {
		SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.crash);

	}
}
