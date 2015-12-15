using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class posAnimState : MonoBehaviour {

	public Animator boatAnim;
	public ApparentWindModuleManager moduleManager;
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
					moduleManager.currAnimState = "In Irons";
					displayPOS.text = "In Irons - the boat is head to wind (0 degrees)";
					displayPOS2.text = "";

					break;
				case "PTCloseHaul":
					boatAnim.SetTrigger("PTCloseHaul");
					moduleManager.currAnimState = "Close Hauled Port Tack";
					displayPOS.text = "Close Hauled on a " + "<color=#FF0000> port tack </color>";
					displayPOS2.text = "the boat is sailing as close to the wind as possible (~45 degrees)";

					break;
				case "PTCloseReach":
					boatAnim.SetTrigger("PTCloseReach");
					moduleManager.currAnimState = "Close Reach Port Tack";
					displayPOS.text = "Close Reach on a " + "<color=#FF0000> port tack </color>";
					displayPOS2.text = "the point of sail between close-hauled and beam reach. (~60 degrees";

					break;

				case "PTBeamReach":
					boatAnim.SetTrigger ("PTBeamReach");
					moduleManager.currAnimState = "Beam Reach Port Tack";
					displayPOS.text = "Beam Reach on a " + "<color=#FF0000> port tack </color>"; 
					displayPOS2.text = "The wind is abeam of the boat (~90 degrees)";

					break;

				case "PTBroadReach":
					boatAnim.SetTrigger("PTBroadReach");
					moduleManager.currAnimState = "Broad Reach Port Tack";
					displayPOS.text = "Broad Reach on a " + "<color=#FF0000> port tack </color>"; 
					displayPOS2.text = "The point of sail between a beam reach and a run (~135 degrees)";
					break;
				case "Run":
					ApparentWindModuleManager.s_instance.hasClickedRun = true;
					boatAnim.SetTrigger("Run");
					moduleManager.currAnimState = "Run";
					displayPOS.text = "Run - the point of sail on which the wind is aft (180 degrees)";
					displayPOS2.text = "";

					break;
				case "STBroadReach":
					boatAnim.SetTrigger("STBroadReach");
					moduleManager.currAnimState = "Broad Reach Starboard Tack";
					displayPOS.text = "Broad Reach on a " + "<color=#00FF00> starboard tack </color>";
					displayPOS2.text = "The point of sail between a beam reach and a run (~135 degrees)";
					break;
				case "STBeamReach":
					boatAnim.SetTrigger("STBeamReach");
					moduleManager.currAnimState = "Beam Reach Starboard Tack";
					displayPOS.text = "Beam Reach on a " + "<color=#00FF00> starboard tack </color>"; 
					displayPOS2.text = "The wind is abeam of the boat (~90 degrees)";

					break;
				case "STCloseReach":
					boatAnim.SetTrigger("STCloseReach");
					moduleManager.currAnimState = "Close Reach Starboard Tack";
					displayPOS.text = "Close Reach on a " + "<color=#00FF00> starboard tack </color>";
					displayPOS2.text = "The point of sail between close-hauled and beam reach. (~60 degrees)";
					break;
				case "STCloseHaul":
					boatAnim.SetTrigger("STCloseHaul");
					moduleManager.currAnimState = "Close Hauled Starboard Tack";
					displayPOS.text = "Close Hauled on a " + "<color=#00FF00> starboard tack </color>";  
					displayPOS2.text = "The boat is sailing as close to the wind as possible (~45 degrees)";
					break;


				}

			}


		
		}



	}
}
