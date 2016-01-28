using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

	public void SwitchLevel(string level){
		SceneManager.LoadScene (level);	
	}
}
