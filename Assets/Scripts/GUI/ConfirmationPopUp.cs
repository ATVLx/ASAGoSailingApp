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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void InitializeConfirmationPanel (string descriptionOfAction, ConfirmationFunction callBack) {
		s_instance.confimationPanel.SetActive (true);
		if( s_instance.descriptionOfActionText != null )
			s_instance.descriptionOfActionText.text = descriptionOfAction;
		else
			Debug.LogError( s_instance.gameObject.name +"'s ConfirmationPopUp component is missing a reference for DescriptionOfActionText.");
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
