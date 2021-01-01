using UnityEngine;
using System.Collections.Generic;

/*
The hex mesh class here is responsible for constructing the mesh of the hex

I didnt make this class myself since I have a bad understanding of making meshes with verticies that arent squares, but I know we basically just create 6 traingles for the mesh

Thank you CatLike Coding!
*/
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;

	List<Color> colors;

	MeshCollider meshCollider;

	//On awake, we initialize everything and set out mesh to this class
	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
		colors = new List<Color>();
	}

	//In our first function, we reset all of our lists before creating them in Triangulate2 and setting them
    public void Triangulate (HexObject cells) {
		
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		colors.Clear();
		Triangulate2(cells);

		hexMesh.vertices = vertices.ToArray();
		hexMesh.colors = colors.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();

		meshCollider.sharedMesh = hexMesh;

		//Before we finish, we need to set the material shader attached to this object parameters to what we want them to be
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material.SetColor("Color_73CDFA71", HexVisuals.GetColor(cells.hex));
		meshRenderer.material.SetVector("Center", cells.transform.position); 
		
		//meshRenderer.material = 
	}
	
	//Here create each traingle and set their color
	void Triangulate2 (HexObject cell) {
        Vector3 center = cell.transform.localPosition;
		Vector3 zero = Vector3.zero;
        for (int i = 0; i < 6; i++) {
            AddTriangle(
                zero,
                zero + HexUtilities.corners[i],
                zero + HexUtilities.corners[i+1]
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
