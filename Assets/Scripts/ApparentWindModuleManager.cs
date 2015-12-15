using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Manages the Apparent Wind module.
/// </summary>
public class ApparentWindModuleManager : MonoBehaviour {
	public static ApparentWindModuleManager s_instance;
	public enum GameState { Intro, Playing, Complete };

	public GameState gameState;
	public List<Term> listOfPOSTerms,tempListPointTerms,randomListPoints;
	public bool hasClickedRun;
	public Vector3 directionOfWind = new Vector3 (1f,0,1f);
	[System.NonSerialized]
	public string currAnimState;

	// For line renderer
	/// <summary>
	/// The origin of the lineRenderers for true and apparent wind.
	/// </summary>
//	public Transform lineOriginWind;
//	public Transform linePositionBow;
//	public Transform linePositionSail;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
			Debug.LogWarning( "Deleting "+ gameObject.name +" because it is a duplicate ApparentWindModuleManager." );
		}
	}
}
