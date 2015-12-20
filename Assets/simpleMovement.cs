using UnityEngine;
using System.Collections;

public class simpleMovement : MonoBehaviour {
	public float thrust;
	public float turnSpeed;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W))
			this.gameObject.GetComponent<Rigidbody> ().AddForce (transform.forward * thrust);
		if (Input.GetKey (KeyCode.S))
			this.gameObject.GetComponent<Rigidbody> ().AddForce (-transform.forward * thrust);
		if (Input.GetKey (KeyCode.A))
			this.gameObject.GetComponent<Rigidbody> ().AddTorque (transform.up * turnSpeed * -1);
		if (Input.GetKey (KeyCode.D))
			this.gameObject.GetComponent<Rigidbody> ().AddTorque (transform.up * turnSpeed);

	}
}
