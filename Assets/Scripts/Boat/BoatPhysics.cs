using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatPhysics : MonoBehaviour
{

//  //commented out to integrate with Pablo's script
//	public Rigidbody boomRG;
//	public Slider heelSlider;
//	public Slider boomSlider;
	public bool isMenu;
	public GameObject waterObj;
	public float density = 500;
	public int slicesPerAxis = 2;
	public bool isConcave = false;
	public int voxelsLimit = 16;
	public float thrust = 50;
	public float turnSpeed = 10;
	private const float DAMPFER = 0.1f;
	private const float WATER_DENSITY = 1000;
	
	private float voxelHalfHeight;
	private Vector3 localArchimedesForce;
	private List<Vector3> voxels;
	private bool isMeshCollider;
	private List<Vector3[]> forces; 
	Mesh waterMesh;
	/// <summary>
	/// Provides initialization.
	/// </summary>
	private void Start()
	{
		waterMesh = waterObj.GetComponent<MeshFilter> ().mesh;
		forces = new List<Vector3[]>(); //force gizmos
		
		// original rotation and position
		var originalRotation = transform.rotation;
		var originalPosition = transform.position;
		transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;
		
		// The object must have a collider
		if (GetComponent<Collider>() == null)
		{
			gameObject.AddComponent<MeshCollider>();
			Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no collider. MeshCollider has been added.", name));
		}
		isMeshCollider = GetComponent<MeshCollider>() != null;
		
		var bounds = GetComponent<Collider>().bounds;
		if (bounds.size.x < bounds.size.y)
		{
			voxelHalfHeight = bounds.size.x;
		}
		else
		{
			voxelHalfHeight = bounds.size.y;
		}
		if (bounds.size.z < voxelHalfHeight)
		{
			voxelHalfHeight = bounds.size.z;
		}
		voxelHalfHeight /= 2 * slicesPerAxis;
		
		// The object must have a RidigBody
		if (GetComponent<Rigidbody>() == null)
		{
			gameObject.AddComponent<Rigidbody>();
			Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no Rigidbody. Rigidbody has been added.", name));
		}
		GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -bounds.extents.y * 0f, 0) + transform.InverseTransformPoint(bounds.center);
		
		voxels = SliceIntoVoxels(isMeshCollider && isConcave);
		
		// Restore original rotation and position
		transform.rotation = originalRotation;
		transform.position = originalPosition;
		
		float volume = GetComponent<Rigidbody>().mass / density;
		
		WeldPoints(voxels, voxelsLimit);
		
		float archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y) * volume;
		localArchimedesForce = new Vector3(0, archimedesForceMagnitude, 0) / voxels.Count;
		
			}

	//commented out Update for integration into Pablo's script
//	void Update(){
//
//		if(Input.GetKey(KeyCode.W)){
//			gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * thrust);
//		}
//		if(Input.GetKey(KeyCode.D)){
//			gameObject.GetComponent<Rigidbody>().AddTorque(transform.up * turnSpeed);
//		}
//		if(Input.GetKey(KeyCode.A)){
//			gameObject.GetComponent<Rigidbody>().AddTorque(transform.up * -turnSpeed);
//		}
//		if(Input.GetKey(KeyCode.S)){
//			gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -thrust);
//		}
//
//		//boomRG.AddTorque(transform.up * boomSlider.value);
//		//gameObject.GetComponent<Rigidbody> ().AddTorque (transform.forward * heelSlider.value);
//
//	}
	
	/// <summary>
	/// Slices object into number of voxels represented by their center points.
	/// </summary>
	private List<Vector3> SliceIntoVoxels(bool concave)
	{
		var points = new List<Vector3>(slicesPerAxis * slicesPerAxis * slicesPerAxis);
		
		if (concave)
		{
			var meshCol = GetComponent<MeshCollider>();
			
			var convexValue = meshCol.convex;
			meshCol.convex = false;
			
//			// Concave slicing
			var bounds = GetComponent<Collider>().bounds;
			for (int ix = 0; ix < slicesPerAxis; ix++)
			{
				for (int iy = 0; iy < slicesPerAxis; iy++)
				{
					for (int iz = 0; iz < slicesPerAxis; iz++)
					{
						float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
						float y = bounds.min.y + bounds.size.y / slicesPerAxis * (0.5f + iy);
						float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);
						
						var p = transform.InverseTransformPoint(new Vector3(x, y, z));
						
						if (PointIsInsideMeshCollider(meshCol, p))
						{
							points.Add(p);
						}
					}
				}
			}
			if (points.Count == 0)
			{
				points.Add(bounds.center);
			}

			meshCol.convex = convexValue;
		}
		else
		{
			// Convex slicing
			var bounds = GetComponent<Collider>().bounds;
			for (int ix = 0; ix < slicesPerAxis; ix++)
			{
				for (int iy = 0; iy < slicesPerAxis; iy++)
				{
					for (int iz = 0; iz < slicesPerAxis; iz++)
					{
						float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
						float y = bounds.min.y + bounds.size.y / slicesPerAxis * (0.5f + iy);
						float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);
						
						var p = transform.InverseTransformPoint(new Vector3(x, y, z));
						
						points.Add(p);
					}
				}
			}
		}
		
		return points;
	}
	
	/// <summary>
	/// Returns whether the point is inside the mesh collider.
	/// </summary>
	private static bool PointIsInsideMeshCollider(Collider c, Vector3 p)
	{
		Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
		
		foreach (var ray in directions)
		{
			RaycastHit hit;
			if (c.Raycast(new Ray(p - ray * 1000, ray), out hit, 1000f) == false)
			{
				return false;
			}
		}
		
		return true;
	}
	
	/// <summary>
	/// Returns two closest points in the list.
	/// </summary>
	private static void FindClosestPoints(IList<Vector3> list, out int firstIndex, out int secondIndex)
	{
		float minDistance = float.MaxValue, maxDistance = float.MinValue;
		firstIndex = 0;
		secondIndex = 1;
		
		for (int i = 0; i < list.Count - 1; i++)
		{
			for (int j = i + 1; j < list.Count; j++)
			{
				float distance = Vector3.Distance(list[i], list[j]);
				if (distance < minDistance)
				{
					minDistance = distance;
					firstIndex = i;
					secondIndex = j;
				}
				if (distance > maxDistance)
				{
					maxDistance = distance;
				}
			}
		}
	}
	
	/// <summary>
	/// Welds closest points.
	private static void WeldPoints(IList<Vector3> list, int targetCount)
	{
		if (list.Count <= 2 || targetCount < 2)
		{
			return;
		}
		
		while (list.Count > targetCount)
		{
			int first, second;
			FindClosestPoints(list, out first, out second);
			
			var mixed = (list[first] + list[second]) * 0.5f;
			list.RemoveAt(second); // the second index is always greater that the first => removing the second item first
			list.RemoveAt(first);
			list.Add(mixed);
		}
	}

	/// <summary>
	/// Returns the water level at given location. 
	/// </summary>
	private float GetWaterLevel(float x, float z)
	{
		float waterLevelMultiplier = -1;
		float minDistanceSqr = Mathf.Infinity;
		Vector3 nearestVert = new Vector3 (x, 0, z);
		foreach (Vector3 vertex in waterMesh.vertices) 
		{
			Vector3 diff = nearestVert-vertex;
			float distSqr = diff.sqrMagnitude;

			if(distSqr < minDistanceSqr)
			{
				minDistanceSqr = distSqr;
				nearestVert = vertex;
			}
			return vertex.y * waterLevelMultiplier + 0.5f;
		}
		//return Mathf.Sin(Time.time * 2f);
		//print (Mathf.Sin(Time.time));
		return 0.0f;

	}



	private void FixedUpdate()
	{
		if (isMenu) {
			GetComponent<Rigidbody> ().AddForce (transform.forward * 200f);
		}
		forces.Clear(); //force gizmos
		
		foreach (var point in voxels)
		{
			var wp = transform.TransformPoint(point);
			float waterLevel = GetWaterLevel(wp.x, wp.z);
			
			if (wp.y - voxelHalfHeight < waterLevel)
			{
				float k = (waterLevel - wp.y) / (2 * voxelHalfHeight) + 0.5f;
				if (k > 1)
				{
					k = 1f;
				}
				else if (k < 0)
				{
					k = 0f;
				}
				
				var velocity = GetComponent<Rigidbody>().GetPointVelocity(wp);
				var localDampingForce = -velocity * DAMPFER * GetComponent<Rigidbody>().mass;
				var force = localDampingForce + Mathf.Sqrt(k) * localArchimedesForce;
				GetComponent<Rigidbody>().AddForceAtPosition(force, wp);
				
				forces.Add(new[] { wp, force }); // force gizmos
			}
		}
	}
	
	/// <summary>
	/// Draws gizmos.
	/// </summary>
	private void OnDrawGizmos()
	{
		if (voxels == null || forces == null)
		{
			return;
		}
		
		const float gizmoSize = 0.05f;
		Gizmos.color = Color.yellow;
		
		foreach (var p in voxels)
		{
			Gizmos.DrawCube(transform.TransformPoint(p), new Vector3(gizmoSize, gizmoSize, gizmoSize));
		}
		
		Gizmos.color = Color.cyan;
		
		foreach (var force in forces)
		{
			Gizmos.DrawCube(force[0], new Vector3(gizmoSize, gizmoSize, gizmoSize));
			Gizmos.DrawLine(force[0], force[0] + force[1] / GetComponent<Rigidbody>().mass);
		}
	}
}