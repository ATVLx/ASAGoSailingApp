using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Action taken when the GoToXModule button is pressed in the scene.
	/// </summary>
	public void PressedLoadModule( GameManager.LevelState selectedModule ) {
		GameManager.s_instance.LoadLevel( (int)selectedModule );
	}
}
