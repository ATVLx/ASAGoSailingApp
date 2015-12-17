using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class pointOfSail{
	public string sailTitle;
	public float angle;
	public pointOfSail(string sailTitleToSet, float angleToSet){
		angle = angleToSet;
		sailTitle = sailTitleToSet;
	}
};
public class BoatControl : BoatBase {
	public TextAsset pointsOfSailTxt;
	bool isLerpingAngle = false;
	float POTlerpTime = .3f, POTlerpTimer, currAngle, endAngle;
	public int indexPOS = 0;
	int maxIndex;
	public Text showCurrentPOS;
	public AudioSource paddleWater;
	public static BoatControl s_instance;
	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	public string ReturnCurrPointOfSail(){
		return allPoints[indexPOS].sailTitle;
	}

	public List<pointOfSail> allPoints = new List<pointOfSail>();
	
	bool playerHasControl, pointsOfSailMode, allRangeMode;
	// Use this for initialization
	void Start () {
		allPoints = TextParser.Parse(pointsOfSailTxt);
		playerHasControl = true;
		maxIndex = allPoints.Count - 1;
	}

	void StartLerp () {
		//check if changing direction
		isLerpingAngle = true;
		POTlerpTimer = Time.time;
	}
	// Update is called once per frame
	void Update () {

		//HANDLES ROTATING THE BOAT AROUND THE POINTS OF SAIL___________________________________________________________________________________________
		if (isLerpingAngle) {
			float fracJourney = (Time.time - POTlerpTimer)/POTlerpTime;
			transform.rotation = Quaternion.Lerp (Quaternion.Euler(0, currAngle,0),Quaternion.Euler(0, allPoints[indexPOS].angle,0), fracJourney);
			if (fracJourney > .99f) {
				fracJourney = 1f;
				transform.rotation = Quaternion.Lerp (Quaternion.Euler(0, currAngle,0),Quaternion.Euler(0, allPoints[indexPOS].angle,0), fracJourney);
				isLerpingAngle = false;
				currAngle = allPoints[indexPOS].angle;
			}
		}

		if (!isLerpingAngle && playerHasControl) {
			if(Input.GetKeyDown(KeyCode.LeftArrow)) {
				paddleWater.Play ();
				if(indexPOS == maxIndex) {
					indexPOS = 0;
				}
				else {
					indexPOS++;
				}
				StartLerp();
			}	
			if(Input.GetKeyDown(KeyCode.RightArrow)) {
				paddleWater.Play ();
				if(indexPOS == 0) {
					indexPOS = maxIndex;
				}
				else {
					indexPOS--;
				}

				StartLerp();
			}

		}
		//______________________________________________________________________________________________________________________________________________
	}
}
