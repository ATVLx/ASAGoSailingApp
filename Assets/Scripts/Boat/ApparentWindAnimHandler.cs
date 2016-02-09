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
	public Text tackDescriptor, auxInfo; 

	void Start() {
		//TODO Make this class check for the overarching GameManager's current state and then get the corresponding module's s_instance
		if( apparentWindModuleManager == null ) {
			Debug.LogWarning( gameObject.name + "'s ApparentWIndAnimHandler component is missing a reference for moduleManager.\n<color=#FFFFFFFF>Note: This is only a problem if we are in the ApparentWind module scene.</color>");
		}
		if( pOSModuleManager == null ) {
			Debug.LogWarning( gameObject.name + "'s ApparentWIndAnimHandler component is missing a reference for POSmoduleManager.\n<color=#FFFFFFFF>Note: This is only a problem if we are in the POS module scene.</color>");
		}
	}

	void Update () {
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
						
						ApparentWindModuleManager.s_instance.hasClickedRun = false;

						//checks which point of sail object was clicked for animation states
						switch(hit.collider.gameObject.name) {
						case "Irons":
							boatAnim.SetTrigger("Irons");
							break;

						case "PTCloseHaul":
							boatAnim.SetTrigger("PTCloseHaul");
							break;

						case "PTCloseReach":
							boatAnim.SetTrigger("PTCloseReach");
							break;

						case "PTBeamReach":
							boatAnim.SetTrigger ("PTBeamReach");
							break;

						case "PTBroadReach":
							boatAnim.SetTrigger("PTBroadReach");
							break;

						case "Run":
							ApparentWindModuleManager.s_instance.hasClickedRun = true;
							boatAnim.SetTrigger("Run");
							break;

						case "STBroadReach":
							boatAnim.SetTrigger("STBroadReach");
							break;
						
						case "STBeamReach":
							boatAnim.SetTrigger("STBeamReach");
							break;

						case "STCloseReach":
							boatAnim.SetTrigger("STCloseReach");
							break;

						case "STCloseHaul":
							boatAnim.SetTrigger("STCloseHaul");
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
							tackDescriptor.text = "In Irons - the boat is head to wind (0 degrees)";
							auxInfo.text = "";

							break;
						case "PTCloseHaul":
							boatAnim.SetTrigger("PTCloseHaul");
							pOSModuleManager.currAnimState = "Close Hauled Port Tack";
							tackDescriptor.text = "Close Hauled on a " + "<color=#00FF00>Port tack </color>";
							auxInfo.text = "The boat is sailing as close to the wind as possible (~45 degrees)";

							break;
						case "PTCloseReach":
							boatAnim.SetTrigger("PTCloseReach");
							pOSModuleManager.currAnimState = "Close Reach Port Tack";
							tackDescriptor.text = "Close Reach on a " + "<color=#00FF00>Port tack </color>";
							auxInfo.text = "The point of sail between close-hauled and beam reach. (~60 degrees";

							break;

						case "PTBeamReach":
							boatAnim.SetTrigger ("PTBeamReach");
							pOSModuleManager.currAnimState = "Beam Reach Port Tack";
							tackDescriptor.text = "Beam Reach on a " + "<color=#00FF00>Port tack </color>"; 
							auxInfo.text = "The wind is abeam of the boat (~90 degrees)";

							break;

						case "PTBroadReach":
							boatAnim.SetTrigger("PTBroadReach");
							pOSModuleManager.currAnimState = "Broad Reach Port Tack";
							tackDescriptor.text = "Broad Reach on a " + "<color=#00FF00>Port tack </color>"; 
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
							tackDescriptor.text = "Broad Reach on a " + "<color=#FF5C49FF>Starboard tack </color>";
							auxInfo.text = "The point of sail between a beam reach and a run (~135 degrees)";
							break;
						case "STBeamReach":
							boatAnim.SetTrigger("STBeamReach");
							pOSModuleManager.currAnimState = "Beam Reach Starboard Tack";
							tackDescriptor.text = "Beam Reach on a " + "<color=#FF5C49FF>Starboard tack </color>"; 
							auxInfo.text = "The wind is abeam of the boat (~90 degrees)";

							break;
						case "STCloseReach":
							boatAnim.SetTrigger("STCloseReach");
							pOSModuleManager.currAnimState = "Close Reach Starboard Tack";
							tackDescriptor.text = "Close Reach on a " + "<color=#FF5C49FF>Starboard tack </color>";
							auxInfo.text = "The point of sail between close-hauled and beam reach. (~60 degrees)";
							break;
						case "STCloseHaul":
							boatAnim.SetTrigger("STCloseHaul");
							pOSModuleManager.currAnimState = "Close Hauled Starboard Tack";
							tackDescriptor.text = "Close Hauled on a " + "<color=#FF5C49FF>Starboard tack </color>";  
							auxInfo.text = "The boat is sailing as close to the wind as possible (~45 degrees)";
							break;
						}
					}
				}
			}
		} 
	}
}
