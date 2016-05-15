using UnityEngine;
using System.Collections;

public class DynamicMeshHole : MonoBehaviour
{
	[SerializeField]
	MeshFilter _meshFilter = null;
	Mesh _mesh = null;
	bool _isHit = false;
	Vector3[] _hitVertices = new Vector3[3];

	// Use this for initialization
	void Start()
	{
		_mesh = _meshFilter.mesh;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Mesh mesh = _mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
				_hitVertices[0] = p0;
				_hitVertices[1] = p1;
				_hitVertices[2] = p2;
				_isHit = true;
			}
		}
		if (_isHit)
		{
			Debug.DrawLine(_hitVertices[0], _hitVertices[1]);
			Debug.DrawLine(_hitVertices[1], _hitVertices[2]);
			Debug.DrawLine(_hitVertices[2], _hitVertices[0]);
		}
	}
}
