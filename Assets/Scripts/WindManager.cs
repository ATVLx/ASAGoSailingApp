using UnityEngine;
using System.Collections;

public class WindManager : MonoBehaviour {

	public static WindManager s_instance;

	public Vector3 directionOfWind = new Vector3(0,0,1f);

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}
}
