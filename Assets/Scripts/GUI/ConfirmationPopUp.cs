using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
	This class uses a delegate to store a function to be executed after a callback if it returns true,
	used for "are you sure you want to quit" type of prompts
*/

public class ConfirmationPopUp : MonoBehaviour {
	public static ConfirmationPopUp s_instance;

	[SerializeField]
	GameObject confimationPanel;
	[SerializeField]
	Text descriptionOfActionText;

	public delegate void ConfirmationFunction (bool confirmationBool);
	public static ConfirmationFunction myConfirmationDelegate = null;

	void Awake () {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy (this.gameObject);
			Debug.LogWarning("Deleted " + gameObject.name + " because it was a duplicate confirmation pop up instance");
		}
	}

	public void InitializeConfirmationPanel (string descriptionOfAction, ConfirmationFunction callBack) {
		confimationPanel.SetActive (true);
		if( descriptionOfActionText != null )
			descriptionOfActionText.text = descriptionOfAction;
		else
			Debug.LogError( gameObject.name +"'s ConfirmationPopUp component is missing a reference for DescriptionOfActionText.");
		myConfirmationDelegate = callBack;
	}

	public void Confirm () {
		myConfirmationDelegate (true);
		myConfirmationDelegate = null;
		confimationPanel.SetActive (false);
	}

	public void Deny () {
		myConfirmationDelegate (false);
		myConfirmationDelegate = null;
		confimationPanel.SetActive (false);

	}
}
