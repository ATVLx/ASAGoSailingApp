using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraDepthMode : MonoBehaviour {	

	void Start () {
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
	}

	void OnEnable(){
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
	}

	void OnDisable(){
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
	}
}
