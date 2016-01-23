using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
	Handles GUI consistent through all levels and on main menu
*/


public class GUIManager : MonoBehaviour {
	public static GUIManager s_instance;
//	public Text currTarget;
//	public Text timeText;
	public GameObject gamePlayPage;

	void Awake() {
		if( s_instance == null )
			s_instance = this;
		else
			Destroy(this.gameObject);
	}

	void Start () {	
	}

}
