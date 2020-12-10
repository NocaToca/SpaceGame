﻿using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;

	List<Color> colors;

	MeshCollider meshCollider;

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
		colors = new List<Color>();
	}

    public void Triangulate (HexObject cells) {
		
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		colors.Clear();
		//for (int i = 0; i < cells.Length; i++) {
			Triangulate2(cells);
		//}
		hexMesh.vertices = vertices.ToArray();
		hexMesh.colors = colors.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();

		meshCollider.sharedMesh = hexMesh;

		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material.SetColor("Color_73CDFA71", HexVisuals.GetColor(cells.hex));
		meshRenderer.material.SetVector("Center", cells.transform.position); 
		
		//meshRenderer.material = 
	}
	
	void Triangulate2 (HexObject cell) {
        Vector3 center = cell.transform.localPosition;
		Vector3 zero = Vector3.zero;
        for (int i = 0; i < 6; i++) {
            AddTriangle(
                zero,
                zero + Utilities.corners[i],
                zero + Utilities.corners[i+1]
            );
			AddTriangleColor(HexVisuals.GetColor(cell.hex));
        }
	}

    void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	void AddTriangleColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
}
