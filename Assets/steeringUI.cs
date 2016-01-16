using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class steeringUI : MonoBehaviour {

	public Slider steeringSlider;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 curRot = new Vector3 (0, 0, transform.rotation.z);
		transform.rotation = Quaternion.Euler(0,0,steeringSlider.value * -120);
	}
}
