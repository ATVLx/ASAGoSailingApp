using UnityEngine;
using System.Collections;
using System;

public class AIBoat : MonoBehaviour {

	Rigidbody m_body;
	float m_deadZone = 0.1f;
	public float bullDozeStrength = 1000f;
	public float accelerateStrength = 30000f;
	public float turnStrength = 5000f;
	public float strafeStrength = 10000f;
	public float verticalAxisDirection = 1f;
	public float horizontalAxisDirection = 1f;
	public float strafeAxisDirection = 1f;
	public float m_hoverForce = 9.0f;
	public float m_hoverHeight = 2.0f;
	public GameObject[] m_hoverPoints;
	public float faceObjectBuffer = 50f;
	float accelerationModifier;

	//FX

	bool accelerate, steer, strafe;

	int m_layerMask;
	int m_wallMask;
	bool isTouchingGround;

	void Start()
	{
		m_body = GetComponent<Rigidbody>();
		m_layerMask = 1 << LayerMask.NameToLayer("Characters");
		m_layerMask = ~m_layerMask;

		m_wallMask = 1 << LayerMask.NameToLayer("Walls");
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
		m_body.AddForce(transform.forward * (accelerateStrength+accelerationModifier) * verticalAxisDirection);
	}

	IEnumerator SlowDown (float time) {
		accelerationModifier = -3 * accelerateStrength/4f;
		yield return new WaitForSeconds(time);
		accelerationModifier = 0;
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
		

	public bool IsTargetInFront (Vector3 target) {
		Vector3 targetDirection = target - transform.position;
		if (Vector3.Dot (transform.forward, targetDirection) > 0) {
			return true;
		}
		else {
			return false;
		}
	}

	public float IsTargetOnRightOrLeft (Vector3 target) {
		Vector3 relativePoint = transform.InverseTransformPoint(target);
		return relativePoint.x;
		//where positive means on right negative means on left
	}

	public float GetAngleToTarget(Vector3 target) {
		Vector3 targetDirection = target - transform.position;
		targetDirection = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;
		return Mathf.Acos(Vector3.Dot (transform.forward,targetDirection));
		//always returns positive angle value in radians
	}

	public void FaceTarget (Vector3 target) {
		//face target 
		if (IsTargetOnRightOrLeft(target) < faceObjectBuffer && GetAngleToTarget(target) > 0.3f){
			SetSteering(true,true);
		}
		else if (IsTargetOnRightOrLeft(target) > faceObjectBuffer && GetAngleToTarget(target) > 0.3f){
			SetSteering(true,false);
		}
		else {
			SetSteering(false,false);
		}
	}


	public void FaceAwayFromTarget(Vector3 target) {
		if (IsTargetOnRightOrLeft(target) < 0 && GetAngleToTarget(target) < 3f){
			SetSteering(true,false);
		}
		else if (IsTargetOnRightOrLeft(target) > 0 && GetAngleToTarget(target) < 3f){
			SetSteering(true,true);
		}
		else {
			SetSteering (false,false);
		}
	}


}
