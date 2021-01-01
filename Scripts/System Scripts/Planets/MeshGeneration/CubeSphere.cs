using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeSphere : MonoBehaviour
{
	//Since we need to generate a sphere to be able to work with it, we will use the "cube sphere" method to generate this
	public static int planetsGened = 0;

	//The properties of each cube sphere
    public int width;
    public int height;
    public int length;

    public float radius = 1;
	public float scale = 1;

    public int gridSize;

	//These two bools are, because I am using these solely to generate planets, to determine what point in the planet-making process we're at
	public bool IsAtmosphere = true;
	public bool IsClouds = false;

    private Vector3[] vertices;
    private Vector3[] normals;
    private Color32[] cubeUV;
    private Vector2[] uvs;

    private Mesh mesh;

	//The material for each planet piece
	public static Material Atmosphere;
	public static Material Planet;
	public static Material Clouds;


    void Awake(){
        
    }

	void Start(){
		
	}

	//Generates the planet
    public void Generate () {
		//First we get the mesh and mesh renderer
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		MeshRenderer mr = GetComponent<MeshRenderer>();
		mesh.name = "Procedural Sphere";

		//mesh.subMeshCount = 3;
        //TextureRenderer tr = GetComponentInChildren<TextureRenderer>();
		//Each planet object we set the child to the current object containing this planet; so it's organized and we can work with it later
		if(!IsAtmosphere && planetsGened < 6){
			//Generates the body of the planet
			gameObject.name = "Body";
			mr.material = Planet;
			//tr.WorkWithMesh(this.gameObject);
			CubeSphere sphere = Instantiate(this);
			planetsGened++;
			sphere.IsAtmosphere = true;
			sphere.gridSize = gridSize + gridSize/4;
			sphere.radius = radius + .05f;
			GameObject parent = this.transform.parent.gameObject;
			sphere.transform.SetParent(parent.transform);
			sphere.Generate();
			//sphere.SetColor(TextureManager.AtmosphereColor);
		} else 
		if(!IsClouds)
		{
			//Generates the atmosphere of the planet
			gameObject.name = "Atmosphere";
			mr.material = Atmosphere;
			mr.castShadows = false;
			//tr.WorkWithAtmosphere(this.gameObject);
			//gridSize += 1;
			CubeSphere sphere = Instantiate(this);
			sphere.IsClouds = true;
			sphere.radius = radius + .01f;
			GameObject parent = this.transform.parent.gameObject;
			sphere.transform.SetParent(parent.transform);
			sphere.Generate();
			//sphere.SetClouds(TextureManager.AtmosphereColor);
		} else {
			//Generates the clouds of the planet
			gameObject.name = "Clouds";
			mr.material = Clouds;
			mr.castShadows = false;
		}

		//Sets the width,height,and length to the grid size
		width = gridSize;
		height = gridSize;
		length = gridSize;
        
		CreateVertices();
		CreateTriangles();
        CreateColliders();
		
	}
    
	//Creates the verticies of the sphere. 
    private void CreateVertices(){
        
        int cornerVertices = 8;
        int edgeVertices = (width + height + length - 3) * 4;
        int faceVertices = ((width - 1) * (height - 1) + (width - 1) * (length - 1) + (height - 1) * (length - 1)) * 2;

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[vertices.Length];
        cubeUV = new Color32[vertices.Length];
        uvs = new Vector2[vertices.Length];

        int v = 0;
		//Setting the sides
        for(int y = 0; y <= height; y++){
            for (int x = 0; x <= width; x++, v++) {
                SetVertex(v, x, y, 0);
                SetUVs(v,x,y);
            }
            for (int z = 1; z <= length; z++, v++) {
                SetVertex(v, width, y, z);
                SetUVs(v,z,y);
            }
            for (int x = width - 1; x >= 0; x--, v++) {
                SetVertex(v, x, y, length);
                SetUVs(v,x,y);
            }
            for (int z = length - 1; z > 0; z--, v++) {
                SetVertex(v, 0, y, z);
                SetUVs(v,z,y);
		    }
        }

		//And the top/bottom
        for (int z = 1; z < length; z++) {
			for (int x = 1; x < width; x++, v++) {
				SetVertex(v, x, height, z);
                SetUVs(v,width - x,length - z);
			}
		}
		for (int z = 1; z < length; z++) {
			for (int x = 1; x < width; x++, v++) {
				SetVertex(v, x, 0, z);
                SetUVs(v,width - x,length - z);
			}
		}
		
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors32 = cubeUV;
        mesh.uv = uvs;
        
    }

	/*
	For each vertex, we want to find the normal of it based on the normal equations for a sphere. 
	*/
    private void SetVertex (int i, int x, int y, int z) {
		Vector3 v = new Vector3(x, y, z) * 2.0f/gridSize - Vector3.one;
        float x2 = v.x * v.x;
		float y2 = v.y * v.y;
		float z2 = v.z * v.z;
		Vector3 s;
		s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
		s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
		s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);
        normals[i] = s;
		vertices[i] = normals[i] * radius;
        //float u = (Mathf.Atan2(v.z,v.x) / (2f * Mathf.PI));
        //float v2 = (Mathf.Asin(v.y) / Mathf.PI) + 0.5f;  
        //uvs[i] = new Vector2(u,v2);
        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 1);
	}

	//Setting the UVs to the uv equation of a sphere too!
    private void SetUVs(int i, int x, int y){
        Vector3 vector = vertices[i];
        float u = 0.5f + (Mathf.Atan2(vector.z,vector.x) / (2f * Mathf.PI));
        float v = 0.5f -(Mathf.Asin(vector.y) / Mathf.PI);
        uvs[i] = new Vector2(u, v);
    }

    // private void OnDrawGizmos () {
	// 	if (vertices == null) {
	// 		return;
	// 	}
	// 	Gizmos.color = Color.black;
	// 	for (int i = 0; i < vertices.Length; i++) {
	// 		Gizmos.DrawSphere(vertices[i], 0.1f);
	// 	}
	// }

	//Creates the triangles of the mesh so it can render
    private void CreateTriangles(){
        // int quads = (width * height + width * length + height * length) * 2;
		// int[] triangles = new int[quads * 6];
		int[] trianglesZ = new int[(width * height) * 12];
		int[] trianglesX = new int[(height * length) * 12];
		int[] trianglesY = new int[(width * length) * 12];
        int ring = (width + length) * 2;
		int tZ = 0, tX = 0, tY = 0, v = 0;

        for (int y = 0; y < height; y++, v++) {
			for (int q = 0; q < width; q++, v++) {
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < length; q++, v++) {
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < width; q++, v++) {
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < length - 1; q++, v++) {
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
		}
        tY = CreateTopFace(trianglesY, tY, ring);
		tY = CreateBottomFace(trianglesY, tY, ring);
		int[] allTriangles = new int[trianglesX.Length + trianglesY.Length + trianglesZ.Length];
		for(int i = 0; i < trianglesZ.Length; i++){
			allTriangles[i] = trianglesZ[i];
		}
		for(int i = 0; i < trianglesY.Length; i++){
			allTriangles[i + trianglesZ.Length] = trianglesY[i];
		}
		for(int i = 0; i < trianglesX.Length; i++){
			allTriangles[i + trianglesZ.Length + trianglesY.Length] = trianglesX[i];
		}
		mesh.triangles = allTriangles;
		//mesh.SetTriangles(trianglesZ, 0);
		//mesh.SetTriangles(trianglesX, 1);
		//mesh.SetTriangles(trianglesY, 2);
        
    }

	//The top of the mesh
    private int CreateTopFace (int[] triangles, int t, int ring) {
		int v = ring * height;
		for (int x = 0; x < width - 1; x++, v++) {
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
		}
		t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

		int vMin = ring * (height + 1) - 1;
		int vMid = vMin + 1;
        int vMax = v + 2;
        for (int z = 1; z < length - 1; z++, vMin--, vMid++, vMax++) {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + width - 1);
            for (int x = 1; x < width - 1; x++, vMid++) {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + width - 1, vMid + width);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + width - 1, vMax + 1);
        }

        int vTop = vMin - 2;
		t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < width - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
		}
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

		return t;
	}

	//The bottom of the mesh
    private int CreateBottomFace (int[] triangles, int t, int ring) {
		int v = 1;
		int vMid = vertices.Length - (width - 1) * (length - 1);
		t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
		for (int x = 1; x < width - 1; x++, v++, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
		}
		t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

		int vMin = ring - 2;
		vMid -= width - 2;
		int vMax = v + 2;

		for (int z = 1; z < length - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid + width - 1, vMin + 1, vMid);
			for (int x = 1; x < width - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid + width - 1, vMid + width, vMid, vMid + 1);
			}
			t = SetQuad(triangles, t, vMid + width - 1, vMax + 1, vMid, vMax);
		}

		int vTop = vMin - 1;
		t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
		for (int x = 1; x < width - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
		}
		t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
		
		return t;
	}

	//Setting each quad of traingles, then returning the index that we're at
    private static int SetQuad (int[] triangles, int i, int v00, int v10, int v01, int v11) {
		triangles[i] = v00;
		triangles[i + 1] = triangles[i + 4] = v01;
		triangles[i + 2] = triangles[i + 3] = v10;
		triangles[i + 5] = v11;
		return i + 6;
	}

	//Creates a sphere collider
    private void CreateColliders(){
		gameObject.AddComponent<SphereCollider>();
	}

	//Sets the texture of the planet body
	public void SetTexture(Texture2D texture){
		MeshRenderer mr = GetComponent<MeshRenderer>();
		mr.sharedMaterial.mainTexture = texture;
        mr.material.SetTexture("Main", texture);
	}

	//Sets the atmosphere color
	public void SetColor(Color col){
		MeshRenderer mr = GetComponent<MeshRenderer>();
		//mr.sharedMaterial.mainTexture = texture;
		mr.castShadows = false;
        mr.material.SetColor("Color_F1E7D8FD", col);
	}

	//Sets the texture of the clouds
	public void SetClouds(Texture2D texture){
		MeshRenderer mr = GetComponent<MeshRenderer>();
		mr.castShadows = false;
		mr.material.SetTexture("Texture2D_FDA423C8", texture);

	}
}
