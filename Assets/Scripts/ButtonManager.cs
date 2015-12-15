using UnityEngine;
using System.Collections;

public class ButtonManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void PointsOfSailButton () {
		print ("POINTS OF SAIL CLICKED");
		ConfirmationPopUp.s_instance.InitializeConfirmationPanel( (bool answer) => {
			if (answer) {
				print("YES!!!!!");
			}
			else {
				print("NOOOOOO");
			}
		});
	}

	// Update is called once per frame
	void Update () {
	
	}
}
