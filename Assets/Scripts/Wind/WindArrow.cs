﻿using UnityEngine;
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
		transform.SetParent(player.transform);

	}
	
	// Update is called once per frame
	void Update () {
		
		if (Vector3.Distance(transform.position, player.transform.position) > deathDistance) {
			Destroy(gameObject);
		}
	}

	void FixedUpdate() {
	}
}
