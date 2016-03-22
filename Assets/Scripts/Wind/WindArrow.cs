using UnityEngine;
using System.Collections;

/// <summary>
/// This class handles the functionality of the flying wind arrows.
/// </summary>
public class WindArrow : MonoBehaviour {
//	float spawnTimer;
	Transform player;
	float deathDistance = 80f;
	// Use this for initialization
//	float spawnFrequency = 40f;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
//		if (NavBoatControl.s_instance != null) {
//			transform.SetParent (player.transform);
//
//		}

	}
	
	// Update is called once per frame
	void Update () {
//		transform.rotation = Quaternion.Euler (new Vector3 (0, 180f, 0));
		if (Vector3.Distance(transform.position, player.transform.position) > deathDistance) {
			Destroy(gameObject);
		}

//		if (NavBoatControl.s_instance == null || TrimManager.s_instance!=null) {
			transform.Translate (Vector3.forward * .2f);
//		}
	}

	void FixedUpdate() {
	}
}
