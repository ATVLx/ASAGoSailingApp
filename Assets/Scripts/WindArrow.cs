using UnityEngine;
using System.Collections;

public class WindArrow : MonoBehaviour {
	float spawnTimer;
	// Use this for initialization
	float spawnFrequency = 40f;
	// Use this for initialization
	void Start () {
		spawnTimer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(transform.forward*-.2f);
		if (Time.time - spawnTimer > spawnFrequency) {
			Destroy(gameObject);
		}
	}
}
