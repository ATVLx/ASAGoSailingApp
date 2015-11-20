﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestLerp : MonoBehaviour {

	public bool findAttributes = false;

	private List<Vector3> positions;
	private List<Quaternion> rotations;
	private Vector3 currentStartPos, currentEndPos;
	private Quaternion currentStartRot, currentEndRot;
	private int currentIndex = 0;
	private float sampleRate;
	private float timer = 0f;
	private bool isMoving = false;

	private Transform thisTransform;

	void Start() {
		thisTransform = GetComponent<Transform>();
	}

	void Update () {
		if( findAttributes ) {
			FindAttributes();
		}
		if( isMoving ) {
			// Check if we should go to next segment
			if( timer >= sampleRate ) {
				if( currentIndex+2 < positions.Count ) {
					IterateToNextSegment();
				} else {
					isMoving = false;
				}
			}
			// Lerp Positions
			thisTransform.position = Vector3.Lerp( currentStartPos, currentEndPos, timer/sampleRate );
			thisTransform.rotation = Quaternion.Lerp( currentStartRot, currentEndRot, timer/sampleRate );
		}
	}

	void FindAttributes() {
		GhostPathRecorder temp = GameObject.FindObjectOfType<GhostPathRecorder>();
		if( temp == null )
			Debug.LogError( "TestLerp coulnd't find a GhostPathRecorder in he scene." );

		positions = temp.recordedPositions;
		rotations = temp.recordedRotations;
		sampleRate = temp.sampleRate;

		thisTransform.position = positions[0];
		thisTransform.rotation = rotations[0];

		isMoving = true;
		findAttributes = false;
	}

	void IterateToNextSegment() {
		currentIndex++;
		currentStartPos = positions[currentIndex];
		currentEndPos = positions[currentIndex+1];
		currentStartRot = rotations[currentIndex];
		currentEndRot = rotations[currentIndex+1];

		timer = timer%sampleRate;
	}
}
