﻿using UnityEngine;
using System.Collections;

public class CamPosModifier : MonoBehaviour {

	public float yoffset,zoffset;
	GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (player.transform.position.x, player.transform.position.y + yoffset, player.transform.position.z + zoffset);
	}
}
