using UnityEngine;
using System.Collections;

[RequireComponent (typeof (DestroyableObject))]
/// <summary>
/// Controls the arrows that follow the wind lines in the ApparentWind module.
/// </summary>
public class ApparentWindLineArrow : MonoBehaviour {

	private Transform myTransform;
	private Transform origin;
	private Transform destination;
	private float speed;
	private float percentageTraveled = 0f;

	void OnEnable() {
		ApparentWindModuleManager.UpdateWindLineArrows += UpdatePosition;
	}

	void OnDisable() {
		ApparentWindModuleManager.UpdateWindLineArrows -= UpdatePosition;
	}
	
	public void Initialize( Transform _origin, Transform _destination, float _speed, Color color ) {
		myTransform = transform;
		origin = _origin;
		destination = _destination;
		speed = _speed;
		GetComponentInChildren<SpriteRenderer>().color = color;
		
		myTransform.LookAt( destination );
	}
		
	void UpdatePosition () {
		if( float.IsNaN( percentageTraveled) ) {
			Destroy( gameObject );
			return;
		}
		myTransform.position = Vector3.Lerp( origin.position, destination.position, Mathf.Clamp( percentageTraveled, 0f, 1f ) );
		myTransform.LookAt( destination );

		Vector3 lerpPath = destination.position - origin.position;
		myTransform.position += (destination.position - myTransform.position).normalized * speed * Time.deltaTime;
			
		float totalDistance = lerpPath.magnitude;
		float myDistance = Vector3.Distance( myTransform.position, destination.position );

		percentageTraveled = (totalDistance-myDistance)/totalDistance;
	}
}
