using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class posAnimState : MonoBehaviour {

	public Animator boatAnim;
	public ApparentWindModuleManager gameManager;
	public Text displayPOS, displayPOS2; 
	
	void Update () {

		//Cast ray at point of mouse click
		if (Input.GetButtonDown("Fire1")) 
		{
			boatAnim.enabled = true;
			Ray ray;
			RaycastHit hit;
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 1000.0f)){
				ApparentWindModuleManager.s_instance.hasClickedRun = false;
				//checks which point of sail object was clicked for animation states
				switch(hit.collider.gameObject.name){

				case "Irons":
					boatAnim.SetTrigger("Irons");
					gameManager.currAnimState = "In Irons";
					displayPOS.text = "In Irons - the boat is head to wind (0 degrees)";
					displayPOS2.text = "";

					break;
				case "PTCloseHaul":
					boatAnim.SetTrigger("PTCloseHaul");
					gameManager.currAnimState = "Close Hauled Port Tack";
					displayPOS.text = "Close Hauled on a " + "<color=#FF0000> port tack </color>";
					displayPOS2.text = "the boat is sailing as close to the wind as possible (~45 degrees)";

					break;
				case "PTCloseReach":
					boatAnim.SetTrigger("PTCloseReach");
					gameManager.currAnimState = "Close Reach Port Tack";
					displayPOS.text = "Close Reach on a " + "<color=#FF0000> port tack </color>";
					displayPOS2.text = "the point of sail between close-hauled and beam reach. (~60 degrees";

					break;

				case "PTBeamReach":
					boatAnim.SetTrigger ("PTBeamReach");
					gameManager.currAnimState = "Beam Reach Port Tack";
					displayPOS.text = "Beam Reach on a " + "<color=#FF0000> port tack </color>"; 
					displayPOS2.text = "The wind is abeam of the boat (~90 degrees)";

					break;

				case "PTBroadReach":
					boatAnim.SetTrigger("PTBroadReach");
					gameManager.currAnimState = "Broad Reach Port Tack";
					displayPOS.text = "Broad Reach on a " + "<color=#FF0000> port tack </color>"; 
					displayPOS2.text = "The point of sail between a beam reach and a run (~135 degrees)";
					break;
				case "Run":
					ApparentWindModuleManager.s_instance.hasClickedRun = true;
					boatAnim.SetTrigger("Run");
					gameManager.currAnimState = "Run";
					displayPOS.text = "Run - the point of sail on which the wind is aft (180 degrees)";
					displayPOS2.text = "";

					break;
				case "STBroadReach":
					boatAnim.SetTrigger("STBroadReach");
					gameManager.currAnimState = "Broad Reach Starboard Tack";
					displayPOS.text = "Broad Reach on a " + "<color=#00FF00> starboard tack </color>";
					displayPOS2.text = "The point of sail between a beam reach and a run (~135 degrees)";
					break;
				case "STBeamReach":
					boatAnim.SetTrigger("STBeamReach");
					gameManager.currAnimState = "Beam Reach Starboard Tack";
					displayPOS.text = "Beam Reach on a " + "<color=#00FF00> starboard tack </color>"; 
					displayPOS2.text = "The wind is abeam of the boat (~90 degrees)";

					break;
				case "STCloseReach":
					boatAnim.SetTrigger("STCloseReach");
					gameManager.currAnimState = "Close Reach Starboard Tack";
					displayPOS.text = "Close Reach on a " + "<color=#00FF00> starboard tack </color>";
					displayPOS2.text = "The point of sail between close-hauled and beam reach. (~60 degrees)";
					break;
				case "STCloseHaul":
					boatAnim.SetTrigger("STCloseHaul");
					gameManager.currAnimState = "Close Hauled Starboard Tack";
					displayPOS.text = "Close Hauled on a " + "<color=#00FF00> starboard tack </color>";  
					displayPOS2.text = "The boat is sailing as close to the wind as possible (~45 degrees)";
					break;


				}

			}


		
		}



	}
}
