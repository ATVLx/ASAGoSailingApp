using UnityEngine;
using System.Collections;

public class DieOnHitBoat : MonoBehaviour {
//	[SerializeField] GameObject explosion;
	// Use this for initialization
	private bool hit = false;
	private float speed = 0;
	void Start () {
	
	}
	
	void OnTriggerEnter( Collider col ) {
		if (col.tag == "Player") {
			StartCoroutine ("Die");
			hit = true;
//			Instantiate (explosion,transform.position, Quaternion.identity);
			if (SoundtrackManager.s_instance != null)
				SoundtrackManager.s_instance.PlayAudioSource (SoundtrackManager.s_instance.laser);
		}
			
	}
	void Update () {
		if (hit) {
			speed += Time.smoothDeltaTime * 10;
			transform.Translate(Vector3.up *speed);
		}
	}

	IEnumerator Die () {
		yield return new WaitForSeconds (8f);
		Destroy (gameObject);

	}

}
