using UnityEngine;
using System.Collections;

[RequireComponent (typeof (LineRenderer))]
/// <summary>
/// Connects the wind line renderer and spawns wind line arrows.
/// </summary>
public class ConnectLineRenderer : MonoBehaviour {

	[Header( "Line Renderer:" )]
	public Transform startPos;
	public Transform endPos;
	
	[Header( "Arrow Spawning" )]
	public GameObject arrowPrefab;
	public float arrowSpeed = 1f;
	public Color arrowColor = Color.white;
	public int arrowDestroyableLayerIndex = 0;

	private LineRenderer myLineRenderer;
	private float spawnFrequency = 3f;
	private float spawnTimer = 0f;

	void Start () {
		myLineRenderer = GetComponent<LineRenderer>();
		spawnTimer = spawnFrequency;

		if( arrowPrefab == null )
			Debug.LogError( gameObject.name + "'s " + this.name + " component is missing a reference for it's arrowPrefab." );
	}

	void Update() {
		if( spawnTimer >= spawnFrequency ) {
			spawnTimer = 0f;
			SpawnNewArrow();
		}
		spawnTimer += Time.deltaTime;
	}

	private void SpawnNewArrow() {
		ApparentWindLineArrow temp = GameObject.Instantiate( arrowPrefab ).GetComponent<ApparentWindLineArrow>();
		temp.Initialize( startPos, endPos, arrowSpeed, arrowColor );
		temp.GetComponent<DestroyableObject>().destroyableLayerIndex = arrowDestroyableLayerIndex;
	}

	public void UpdatePosition () {
		Vector3[] positions = new Vector3[2] {startPos.position, endPos.position};
		myLineRenderer.SetPositions( positions );
	}
}
