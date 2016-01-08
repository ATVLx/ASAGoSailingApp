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
	public ApparentWindModuleManager moduleManager;
	public POSModuleManager POSmoduleManager;
	public Text tackDescriptor, auxInfo; 

	void Start() {
		if( moduleManager == null ) {
			Debug.LogWarning( gameObject.name + "'s ApparentWIndAnimHandler component is missing a reference for moduleManager.\nNote: This is only a problem if we are in the ApparentWind module scene.");
		}
		if( POSmoduleManager == null ) {
			Debug.LogWarning( gameObject.name + "'s ApparentWIndAnimHandler component is missing a reference for POSmoduleManager.\nNote: This is only a problem if we are in the POS module scene.");
		}
	}

	void Update () {
		if( moduleManager != null ) {
			if( moduleManager.gameState == ApparentWindModuleManager.GameState.Playing ) {
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
							tackDescriptor.text = "In Irons - the boat is head to wind (0 degrees)";
							auxInfo.text = "";

							break;
						case "PTCloseHaul":
							boatAnim.SetTrigger("PTCloseHaul");
							moduleManager.currAnimState = "Close Hauled Port Tack";
							tackDescriptor.text = "Close Hauled on a " + "<color=#FF0000> port tack </color>";
							auxInfo.text = "the boat is sailing as close to the wind as possible (~45 degrees)";

							break;
						case "PTCloseReach":
							boatAnim.SetTrigger("PTCloseReach");
							moduleManager.currAnimState = "Close Reach Port Tack";
							tackDescriptor.text = "Close Reach on a " + "<color=#FF0000> port tack </color>";
							auxInfo.text = "the point of sail between close-hauled and beam reach. (~60 degrees";

							break;

						case "PTBeamReach":
							boatAnim.SetTrigger ("PTBeamReach");
							moduleManager.currAnimState = "Beam Reach Port Tack";
							tackDescriptor.text = "Beam Reach on a " + "<color=#FF0000> port tack </color>"; 
							auxInfo.text = "The wind is abeam of the boat (~90 degrees)";

							break;

						case "PTBroadReach":
							boatAnim.SetTrigger("PTBroadReach");
							moduleManager.currAnimState = "Broad Reach Port Tack";
							tackDescriptor.text = "Broad Reach on a " + "<color=#FF0000> port tack </color>"; 
							auxInfo.text = "The point of sail between a beam reach and a run (~135 degrees)";
							break;
						case "Run":
							ApparentWindModuleManager.s_instance.hasClickedRun = true;
							boatAnim.SetTrigger("Run");
							moduleManager.currAnimState = "Run";
							tackDescriptor.text = "Run - the point of sail on which the wind is aft (180 degrees)";
							auxInfo.text = "";

							break;
						case "STBroadReach":
							boatAnim.SetTrigger("STBroadReach");
							moduleManager.currAnimState = "Broad Reach Starboard Tack";
							tackDescriptor.text = "Broad Reach on a " + "<color=#00FF00> starboard tack </color>";
							auxInfo.text = "The point of sail between a beam reach and a run (~135 degrees)";
							break;
						case "STBeamReach":
							boatAnim.SetTrigger("STBeamReach");
							moduleManager.currAnimState = "Beam Reach Starboard Tack";
							tackDescriptor.text = "Beam Reach on a " + "<color=#00FF00> starboard tack </color>"; 
							auxInfo.text = "The wind is abeam of the boat (~90 degrees)";

							break;
						case "STCloseReach":
							boatAnim.SetTrigger("STCloseReach");
							moduleManager.currAnimState = "Close Reach Starboard Tack";
							tackDescriptor.text = "Close Reach on a " + "<color=#00FF00> starboard tack </color>";
							auxInfo.text = "The point of sail between close-hauled and beam reach. (~60 degrees)";
							break;
						case "STCloseHaul":
							boatAnim.SetTrigger("STCloseHaul");
							moduleManager.currAnimState = "Close Hauled Starboard Tack";
							tackDescriptor.text = "Close Hauled on a " + "<color=#00FF00> starboard tack </color>";  
							auxInfo.text = "The boat is sailing as close to the wind as possible (~45 degrees)";
							break;
						}
					}
				}
			} 
		}else if( POSmoduleManager != null ) { 
			if( POSmoduleManager.gameState == POSModuleManager.GameState.Playing ) {
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
							tackDescriptor.text = "In Irons - the boat is head to wind (0 degrees)";
							auxInfo.text = "";

							break;
						case "PTCloseHaul":
							boatAnim.SetTrigger("PTCloseHaul");
							moduleManager.currAnimState = "Close Hauled Port Tack";
							tackDescriptor.text = "Close Hauled on a " + "<color=#FF0000> port tack </color>";
							auxInfo.text = "the boat is sailing as close to the wind as possible (~45 degrees)";

							break;
						case "PTCloseReach":
							boatAnim.SetTrigger("PTCloseReach");
							moduleManager.currAnimState = "Close Reach Port Tack";
							tackDescriptor.text = "Close Reach on a " + "<color=#FF0000> port tack </color>";
							auxInfo.text = "the point of sail between close-hauled and beam reach. (~60 degrees";

							break;

						case "PTBeamReach":
							boatAnim.SetTrigger ("PTBeamReach");
							moduleManager.currAnimState = "Beam Reach Port Tack";
							tackDescriptor.text = "Beam Reach on a " + "<color=#FF0000> port tack </color>"; 
							auxInfo.text = "The wind is abeam of the boat (~90 degrees)";

							break;

						case "PTBroadReach":
							boatAnim.SetTrigger("PTBroadReach");
							moduleManager.currAnimState = "Broad Reach Port Tack";
							tackDescriptor.text = "Broad Reach on a " + "<color=#FF0000> port tack </color>"; 
							auxInfo.text = "The point of sail between a beam reach and a run (~135 degrees)";
							break;
						case "Run":
							ApparentWindModuleManager.s_instance.hasClickedRun = true;
							boatAnim.SetTrigger("Run");
							moduleManager.currAnimState = "Run";
							tackDescriptor.text = "Run - the point of sail on which the wind is aft (180 degrees)";
							auxInfo.text = "";

							break;
						case "STBroadReach":
							boatAnim.SetTrigger("STBroadReach");
							moduleManager.currAnimState = "Broad Reach Starboard Tack";
							tackDescriptor.text = "Broad Reach on a " + "<color=#00FF00> starboard tack </color>";
							auxInfo.text = "The point of sail between a beam reach and a run (~135 degrees)";
							break;
						case "STBeamReach":
							boatAnim.SetTrigger("STBeamReach");
							moduleManager.currAnimState = "Beam Reach Starboard Tack";
							tackDescriptor.text = "Beam Reach on a " + "<color=#00FF00> starboard tack </color>"; 
							auxInfo.text = "The wind is abeam of the boat (~90 degrees)";

							break;
						case "STCloseReach":
							boatAnim.SetTrigger("STCloseReach");
							moduleManager.currAnimState = "Close Reach Starboard Tack";
							tackDescriptor.text = "Close Reach on a " + "<color=#00FF00> starboard tack </color>";
							auxInfo.text = "The point of sail between close-hauled and beam reach. (~60 degrees)";
							break;
						case "STCloseHaul":
							boatAnim.SetTrigger("STCloseHaul");
							moduleManager.currAnimState = "Close Hauled Starboard Tack";
							tackDescriptor.text = "Close Hauled on a " + "<color=#00FF00> starboard tack </color>";  
							auxInfo.text = "The boat is sailing as close to the wind as possible (~45 degrees)";
							break;
						}
					}
				}
			}
		} 
	}
}
