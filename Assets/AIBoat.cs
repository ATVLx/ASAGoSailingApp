using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AIBoat : MonoBehaviour {

	Rigidbody m_body;
	public float accelerateStrength = 30000f;
	public float turnStrength = 5000f;
	float horizontalAxisDirection;
	[SerializeField]
	Animator sail;
	//FX
	public List<GameObject> scenarioTriggers;
	public Transform leeward, windward, port, starboard, overtaker;
	bool accelerate, steer, strafe;


	void Start()
	{
		m_body = GetComponent<Rigidbody>();
		sail.SetFloat ("sailtrim", .8f);
		accelerate = true;
	}

	void FixedUpdate()
	{
		if (accelerate) {
			Accelerate();
		}
		if (steer) {
			Steer ();
		}
	}

	public void Accelerate() {
		m_body.AddForce(transform.forward * (accelerateStrength));
	}


	public void Steer() {
		m_body.AddRelativeTorque(Vector3.up * horizontalAxisDirection * turnStrength);
	}
		

	public void SetSteering (bool on, bool directionRight) {
		if (on == false) {
			steer = false;
		}
		else {
			steer = true;
		}
		if (directionRight) {
			horizontalAxisDirection = -1f;
		}
		else {
			horizontalAxisDirection = 1f;
		}
	}
		







}
