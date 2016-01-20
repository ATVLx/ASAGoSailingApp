using UnityEngine;
using System.Collections;

public class DestroyableObject : MonoBehaviour {

	/// <summary>
	/// Index used to destroy object. DestroyTrigger must have this layer index in it's list to be destroyed.
	/// </summary>
	public int destroyableLayerIndex = 0;
}
