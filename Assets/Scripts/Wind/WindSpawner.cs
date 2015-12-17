using UnityEngine;
using System.Collections;

public class WindSpawner : MonoBehaviour {
	float spawnTimer;
	// Use this for initialization
	float spawnFrequency = 5f;
	public GameObject windArrow;
	void Start () {
		spawnTimer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - spawnTimer > spawnFrequency) {
			SpawnArrow();
		}
	}

	void SpawnArrow() {
		Instantiate(windArrow, transform.position, Quaternion.Euler(0,180f,0));
		spawnTimer = Time.time;
	}
}
