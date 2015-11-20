using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

	public void LoadLevel1() {
		Application.LoadLevel("POSModule");

	}

	public void LoadLevel2() {
		Application.LoadLevel("NavModule");
	}
}
