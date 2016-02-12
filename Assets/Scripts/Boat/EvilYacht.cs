using UnityEngine;
using System.Collections;

public class EvilYacht : MonoBehaviour {
	public bool isMoving;
	public float speed = 100000f;
	public bool isDying, canDie = false;
	public bool isGoingInCircles;
	[SerializeField] GameObject explosion;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isMoving) {
			GetComponent<Rigidbody> ().AddForce (transform.forward * speed);
			if (isGoingInCircles) {
				GetComponent<Rigidbody> ().AddTorque (Vector3.up * 100000f);
			}
		}
	}

	void OnCollisionEnter(Collision thisCol) {
		if (thisCol.gameObject.tag == "Player") {
			SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.crash);
		}
		Kill ();

	}
	public void Kill() {
		if (!isDying && canDie) {
			isDying = true;
			StartCoroutine ("Death");
			if (explosion != null)
				SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.explode);
			
				Instantiate (explosion, transform.position, Quaternion.identity);
		}
	}

	IEnumerator Death () {

		GetComponent<Rigidbody> ().useGravity = true;
		yield return new WaitForSeconds (5f);
		Destroy (gameObject);
	}

}
