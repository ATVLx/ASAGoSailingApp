using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the arrows that follow the wind lines in the ApparentWind module.
/// </summary>
public class ApparentWindLineArrow : MonoBehaviour {

	private Transform myTransform;
	private Transform origin;
	private Transform destination;
	private float speed;
	private float percentageTraveled = 0f;

	void Start() {
		myTransform = transform;
	}
	
	public void Initialize( Transform _origin, Transform _destination, float _speed, Color color ) {
		origin = _origin;
		destination = _destination;
		speed = _speed;
		GetComponentInChildren<SpriteRenderer>().color = color;
		
		myTransform.LookAt( destination );
	}
		
	void Update () {
		float totalDistance = Vector3.Distance( origin.position, destination.position );

		myTransform.position = Vector3.Lerp( origin.position, destination.position, percentageTraveled );
		myTransform.LookAt( destination );
	}
}
