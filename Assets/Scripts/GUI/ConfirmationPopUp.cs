using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConfirmationPopUp : MonoBehaviour {

	[SerializeField]
	GameObject confimationPanel;
	[SerializeField]
	Text descriptionOfActionText;

	public static ConfirmationPopUp s_instance;

	void Awake () {
		if (s_instance == null) {
			s_instance = this;
		} else {
			Destroy (this.gameObject);
			Debug.LogWarning("Deleted " + gameObject.name + " because it was a duplicate confirmation pop up instance");
		}
	}

	public delegate void ConfirmationFunction (bool confirmationBool);
	public ConfirmationFunction myConfirmationDelegate = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void InitializeConfirmationPanel (string descriptionOfAction, ConfirmationFunction thisFunction) {
		confimationPanel.SetActive (true);
		descriptionOfActionText.text = descriptionOfAction;
		if (descriptionOfAction == null) {
			Debug.LogError("Assign string in ConfirmationPanel Initialization");
		}
		myConfirmationDelegate = thisFunction;
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
