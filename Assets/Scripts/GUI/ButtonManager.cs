﻿using UnityEngine;
using System.Collections;

public class ButtonManager : MonoBehaviour {


	public void ConfirmationButton (string confirmationQuestion) {
		ConfirmationPopUp.InitializeConfirmationPanel(confirmationQuestion, (bool answer) => {

			if (answer) {
//				GameManager.s_instance.LoadLevel(levelValue);
			}
			else {
//				GameManager.s_instance.LoadLevel(levelValue);
			}
		});
	}
}