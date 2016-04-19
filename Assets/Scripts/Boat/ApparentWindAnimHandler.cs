using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This class handles the animations and text that are set during the apparent wind and POS module
/// </summary>
public class ApparentWindAnimHandler : MonoBehaviour {

	public Animator boatAnim;
	/// <summary>
	/// Reference to the Apparent Wind Module Manager in the scene.
	/// </summary>
	public ApparentWindModuleManager apparentWindModuleManager;
	public POSModuleManager pOSModuleManager;
	public Text tackDescriptor, auxInfo, POS; 
	string lastPOSHit = "Irons";
	void Start() {
		//TODO Make this class check for the overarching GameManager's current state and then get the corresponding module's s_instance
		if( apparentWindModuleManager == null ) {
			Debug.LogWarning( gameObject.name + "'s ApparentWIndAnimHandler component is missing a reference for moduleManager.\n<color=#FFFFFFFF>Note: This is only a problem if we are in the ApparentWind module scene.</color>");
		}
		if( pOSModuleManager == null ) {
			Debug.LogWarning( gameObject.name + "'s ApparentWIndAnimHandler component is missing a reference for POSmoduleManager.\n<color=#FFFFFFFF>Note: This is only a problem if we are in the POS module scene.</color>");
		}
	}
	void SetTextPOSTextValues () {
		print ("SETTING");
		switch (lastPOSHit) {

		case "Irons":

			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (18f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (0);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (6f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (0);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}




			break;
		case "PTCloseHaul":

			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (23.1f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (6.5f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (9.7f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (4.5f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}

			break;
		case "PTCloseReach":

			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (22.3f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (7f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (9.6f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (5.1f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}
			break;

		case "PTBeamReach":

			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (19.5f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (7.6f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (8.2f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (5.6f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}
			break;

		case "PTBroadReach":

			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (13.8f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (7.5f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (4.2f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (4f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}
			break;
		case "Run":
			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (10.8f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (7.2f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (2.8f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (3.2f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}
			break;
		case "STBroadReach":
			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (13.8f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (7.5f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (4.2f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (4f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}
			break;
		case "STBeamReach":
			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (19.5f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (7.6f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (8.2f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (5.6f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}
			break;
		case "STCloseReach":
			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (22.3f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (7f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (9.6f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (5.1f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}
			break;
		case "STCloseHaul":
			if (ApparentWindModuleManager.s_instance.isWindSpeedSetToHigh) {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (23.1f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (6.5f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (18f);
			} else {
				ApparentWindModuleGuiManager.s_instance.UpdateApparentWindSpeed (9.7f);
				ApparentWindModuleGuiManager.s_instance.UpdateBoatSpeed (4.5f);
				ApparentWindModuleGuiManager.s_instance.UpdateTrueWindSpeed (6f);
			}
			break;
		}
	}
	void Update () {
		if (apparentWindModuleManager != null) {
			SetTextPOSTextValues ();
		}
		if( apparentWindModuleManager != null ) {
			if( apparentWindModuleManager.gameState == ApparentWindModuleManager.GameState.Playing ) {
				
				//Cast ray at point of mouse click
				if (Input.GetButtonDown("Fire1")) 
				{
					boatAnim.enabled = true;
					Ray ray;
					RaycastHit hit;
					ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out hit, 1000.0f)) {
						lastPOSHit = hit.collider.gameObject.name;
						ApparentWindModuleManager.s_instance.hasClickedRun = false;

						//checks which point of sail object was clicked for animation states
						switch(hit.collider.gameObject.name) {
						case "Irons":
							POS.text = "No Sail Zone";
							boatAnim.SetTrigger("Irons");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.InIrons;
							break;

						case "PTCloseHaul":
							POS.text = "Close Hauled Port Tack";

							boatAnim.SetTrigger("PTCloseHaul");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.PTCloseHaul;
							break;

						case "PTCloseReach":
							POS.text = "Close Reach Port Tack";
							boatAnim.SetTrigger("PTCloseReach");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.PTCloseReach;
							break;

						case "PTBeamReach":
							POS.text = "Beam Reach Port Tack";
							boatAnim.SetTrigger ("PTBeamReach");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.PTBeamReach;
							break;

						case "PTBroadReach":
							POS.text = "Broad Reach Port Tack";
							boatAnim.SetTrigger("PTBroadReach");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.PTBroadReach;
							break;

						case "Run":
							POS.text = "Run";

							ApparentWindModuleManager.s_instance.hasClickedRun = true;
							boatAnim.SetTrigger("Run");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.Run;
							break;

						case "STBroadReach":
							POS.text = "Broad Reach Starbord Tack";

							boatAnim.SetTrigger("STBroadReach");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.STBroadReach;
							break;
						
						case "STBeamReach":
							POS.text = "Beam Reach Starbord Tack";
							boatAnim.SetTrigger("STBeamReach");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.STBeamReach;
							break;

						case "STCloseReach":
							POS.text = "Close Reach Starbord Tack";

							boatAnim.SetTrigger("STCloseReach");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.STCloseReach;
							break;

						case "STCloseHaul":
							POS.text = "Close Hauled Starbord Tack";

							boatAnim.SetTrigger("STCloseHaul");
							ApparentWindBoatControl.s_instance.currentPOS = ApparentWindBoatControl.BoatPointOfSail.STCloseHaul;
							break;
						}
					}
				}
			} 
		}

		if( pOSModuleManager != null ) { 
			if( pOSModuleManager.gameState == POSModuleManager.GameState.TestPage || pOSModuleManager.gameState == POSModuleManager.GameState.Playing ) {
				//Cast ray at point of mouse click
				if (Input.GetButtonDown("Fire1")) 
				{
					boatAnim.enabled = true;
					Ray ray;
					RaycastHit hit;
					ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out hit, 1000.0f)){
						
						POSModuleManager.s_instance.hasClickedRun = false;
						//checks which point of sail object was clicked for animation states
						switch(hit.collider.gameObject.name){

						case "Irons":
							boatAnim.SetTrigger("Irons");
							pOSModuleManager.currAnimState = "In Irons";
							tackDescriptor.text = "In Irons - The boat is sailing with the wind coming from directly behind.";
							auxInfo.text = "";

							break;
						case "PTCloseHaul":
							boatAnim.SetTrigger("PTCloseHaul");
							pOSModuleManager.currAnimState = "Close Hauled Port Tack";
							tackDescriptor.text = "Close Hauled on a " + "<color=#FF0000>Port tack </color>";
							auxInfo.text = "The boat is sailing as close to the wind as possible (~45 degrees)";

							break;
						case "PTCloseReach":
							boatAnim.SetTrigger("PTCloseReach");
							pOSModuleManager.currAnimState = "Close Reach Port Tack";
							tackDescriptor.text = "Close Reach on a " + "<color=#FF0000>Port tack </color>";
							auxInfo.text = "The point of sail between close-hauled and beam reach. (~60 degrees";

							break;

						case "PTBeamReach":
							boatAnim.SetTrigger ("PTBeamReach");
							pOSModuleManager.currAnimState = "Beam Reach Port Tack";
							tackDescriptor.text = "Beam Reach on a " + "<color=#FF0000>Port tack </color>"; 
							auxInfo.text = "The wind is abeam of the boat (~90 degrees)";

							break;

						case "PTBroadReach":
							boatAnim.SetTrigger("PTBroadReach");
							pOSModuleManager.currAnimState = "Broad Reach Port Tack";
							tackDescriptor.text = "Broad Reach on a " + "<color=#FF0000>Port tack </color>"; 
							auxInfo.text = "The point of sail between a beam reach and a run (~135 degrees)";
							break;
						case "Run":
							POSModuleManager.s_instance.hasClickedRun = true;
							boatAnim.SetTrigger("Run");
							pOSModuleManager.currAnimState = "Run";
							tackDescriptor.text = "Run - the point of sail on which the wind is aft (180 degrees)";
							auxInfo.text = "";

							break;
						case "STBroadReach":
							boatAnim.SetTrigger("STBroadReach");
							pOSModuleManager.currAnimState = "Broad Reach Starboard Tack";
							tackDescriptor.text = "Broad Reach on a " + "<color=#00FF00>Starboard tack </color>";
							auxInfo.text = "The point of sail between a beam reach and a run (~135 degrees)";
							break;
						case "STBeamReach":
							boatAnim.SetTrigger("STBeamReach");
							pOSModuleManager.currAnimState = "Beam Reach Starboard Tack";
							tackDescriptor.text = "Beam Reach on a " + "<color=#00FF00>Starboard tack </color>"; 
							auxInfo.text = "The wind is abeam of the boat (~90 degrees)";

							break;
						case "STCloseReach":
							boatAnim.SetTrigger("STCloseReach");
							pOSModuleManager.currAnimState = "Close Reach Starboard Tack";
							tackDescriptor.text = "Close Reach on a " + "<color=#00FF00>Starboard tack </color>";
							auxInfo.text = "The point of sail between close-hauled and beam reach. (~60 degrees)";
							break;
						case "STCloseHaul":
							boatAnim.SetTrigger("STCloseHaul");
							pOSModuleManager.currAnimState = "Close Hauled Starboard Tack";
							tackDescriptor.text = "Close Hauled on a " + "<color=#00FF00>Starboard tack </color>";  
							auxInfo.text = "The boat is sailing as close to the wind as possible (~45 degrees)";
							break;
						}
					}
				}
			}
		} 
	}
}
