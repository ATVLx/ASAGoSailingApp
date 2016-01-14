using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ApparentWindModuleGuiManager : MonoBehaviour {
	public static ApparentWindModuleGuiManager s_instance;

	void Awake() {
		if( s_instance == null )
			s_instance = this;
		else {
			Debug.LogWarning( "Destroying " + this.gameObject + " because it is a duplicate ApparentWindModuleGuiManager." );
			Destroy( this.gameObject );
		}
	}
}
