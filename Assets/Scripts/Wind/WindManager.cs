using UnityEngine;
using System.Collections;

/*
	This class handles a direction of wind variable and instantiates wind arrows
*/

public class WindManager : MonoBehaviour {

	public static WindManager s_instance;
	Transform[] spawnPositions;
	public float distFromBoatToSpawn = 50f;
	public float spawnTime = .4f;
	float elapsedTime = 0f;
	public Vector3 directionOfWind = new Vector3(0,0,1f);
	public GameObject arrowPrefab;
	int lastRand;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Debug.LogWarning( "Deleting gameobject \"" + gameObject.name + "\" because it has a duplicate WindManager." );
			Destroy(gameObject);
		}
	}

	void Start() {
		spawnPositions = GetComponentsInChildren<Transform> ();
	}

	void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime > spawnTime) {
			elapsedTime = 0;
			Vector3 tempPosition = GameObject.FindGameObjectWithTag ("Player").transform.position;
			transform.position = new Vector3 (tempPosition.x, 2f, tempPosition.z + distFromBoatToSpawn);
			int rand = Random.Range (0, spawnPositions.Length-1);
			//doesnt let rand be what is was last spawn cycle quick fix and dirty too
			if (lastRand == rand) {
				if (rand == 0) {
					rand++;
				} else {
					rand--;
				}
			}
			lastRand = rand;
			float xDisplace = Random.Range (-4f, 4f);
			Instantiate (arrowPrefab, new Vector3(spawnPositions [rand].position.x + xDisplace, spawnPositions [rand].position.y, spawnPositions [rand].position.z), Quaternion.Euler(new Vector3(0,180,0)));
		}
	}
}
