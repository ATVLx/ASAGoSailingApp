using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class GhostPathRecorder : MonoBehaviour {

	public float sampleRate = 0.25f;
	public bool isRecording = false;
	public bool exportJSON = false;

	public List<Vector3> recordedPositions;
	public List<Quaternion> recordedRotations;
	private Transform thisTransform;
	private float timer;

	void Start () {
		recordedPositions = new List<Vector3>();
		recordedRotations = new List<Quaternion>();
		thisTransform = GetComponent<Transform>();

		// TODO Get sampleRate from external script
	}

	void Update () {
		if( exportJSON ) {
			exportJSON = false;
			if( recordedPositions.Count < 2 || recordedRotations.Count < 2 ) {
				Debug.LogError( "GhostPathRecorder hasn't recorded anything yet so there is nothing to export." );
			} else {
				ExportRecordedDataToJSON();
			}
		}

		if( isRecording ) {
			if( timer >= sampleRate ) {
				// Record position and rotation
				recordedPositions.Add( thisTransform.position );
				recordedRotations.Add( thisTransform.rotation );

				// Add the left over faction of time to the timer after reset
				timer = timer % sampleRate;
			}

			timer += Time.deltaTime;
		}
	}

	public void CheckIfUploadData() {
		// TODO Get data from database
	}

	private void ExportRecordedDataToJSON() {
		JsonObject recordedData = new JsonObject();
		JsonArray posRotArray = new JsonArray();

		for( int i = 0; i < recordedPositions.Count; i++ ) {
			// Get reference to current position/rotation in our recorded data
			Vector3 currentPos = recordedPositions[i];
			Quaternion currentRot = recordedRotations[i];

			// Build out position and rotation objects
			JsonObject posV = new JsonObject();
			posV.Add( "x", currentPos.x );
			posV.Add( "y", currentPos.y );
			posV.Add( "z", currentPos.z );
			JsonObject rotQ = new JsonObject();
			rotQ.Add( "w", currentRot.w );
			rotQ.Add( "x", currentRot.x );
			rotQ.Add( "y", currentRot.y );
			rotQ.Add( "z", currentRot.z );

			// Create and populate object that will encapsulate position and rotation at this index
			JsonObject posRotPair = new JsonObject();
			posRotPair.Add( "posV", posV );
			posRotPair.Add( "rotQ", rotQ );

			// Add new object to array of positions
			posRotArray.Add( posRotPair );
		}
		// Add array of positions to recorded data
		recordedData.Add( "frames", posRotArray );
		Debug.LogWarning( "Exporting complete." );
	}

	public void StartRecording() {
		isRecording = true;
		recordedPositions.Add( thisTransform.position );
		recordedRotations.Add( thisTransform.rotation );
	}

	public void StopRecording() {
		isRecording = false;
	}
}
