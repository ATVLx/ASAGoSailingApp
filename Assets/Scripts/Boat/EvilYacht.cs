using UnityEngine;
using System.Collections;

public class EvilYacht : MonoBehaviour {
	public bool isMoving;
	public float speed = 100000f;
	public bool isDying;
	[SerializeField] GameObject explosion;
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
	public void Kill() {
		if (!isDying) {
			isDying = true;
			StartCoroutine ("Death");
			if (explosion != null)
				Instantiate (explosion, transform.position, Quaternion.identity);
		}
	}

	IEnumerator Death () {

		GetComponent<Rigidbody> ().useGravity = true;
		yield return new WaitForSeconds (5f);
		Destroy (gameObject);
	}

}
