using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WindDirection : MonoBehaviour {

	public GameObject windObj;
	public Slider windSlider;
	public Slider boatMassSlider;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		windObj.transform.rotation = Quaternion.Euler (windObj.transform.rotation.x, 0, windObj.transform.rotation.z);
	}
}
