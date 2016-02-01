using UnityEngine;
using System.Collections;

public class DestroyAfterX : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine ("Death");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Death() {
		yield return new WaitForSeconds (3f);
		Destroy (gameObject);
	}
}
