using UnityEngine;
using System.Collections;

public class WindManager : MonoBehaviour {

	public static WindManager s_instance;
	public Transform[] spawnPositions;
	float distFromBoatToSpawn = 50f;
	float spawnTime = .5f;
	float elapsedTime = 0f;
	public Vector3 directionOfWind = new Vector3(0,0,1f);
	public GameObject arrowPrefab;

	void Awake() {
		if (s_instance == null) {
			s_instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime > spawnTime) {
			elapsedTime = 0;
			Vector3 tempPosition = GameObject.FindGameObjectWithTag ("Player").transform.position;
			transform.position = new Vector3 (tempPosition.x, 2f, tempPosition.z + distFromBoatToSpawn);
			int rand = Random.Range (0, spawnPositions.Length-1);
			Instantiate (arrowPrefab, spawnPositions [rand].position, Quaternion.Euler(new Vector3(0,180,0)));
		}
	}
}
