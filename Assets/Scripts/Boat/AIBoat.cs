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

	public Transform mast;

	void Start()
	{
		m_body = GetComponent<Rigidbody>();
		sail.SetFloat ("sailtrim", 1f);
		accelerate = true;
	}

	public void SetMast(int scenario) {
		if (scenario == 0) {

		} else if (scenario == 1) {
			sail.SetFloat ("sailtrim", -1f);
			mast.localRotation = Quaternion.Euler (new Vector3 (0, 45f, 0));

		} else if (scenario == 2) {
			sail.SetFloat ("sailtrim", -1f);
			mast.localRotation = Quaternion.Euler (new Vector3 (0, 35f, 0));
		} else if (scenario == 3) {
			mast.localRotation = Quaternion.Euler (new Vector3 (0, 60f, 0));

		}
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

	public void SetTack (bool isPort) {
		if (isPort) {
			sail.SetFloat ("sailtrim", 0.8f);
			mast.transform.localRotation = Quaternion.Euler (0, -50f, 0);
		} else {
			sail.SetFloat ("sailtrim", -0.8f);
			mast.transform.localRotation = Quaternion.Euler (0, 50f, 0);

		}
	}
}
