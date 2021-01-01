using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS CLASS WAS USED TO BRING ME TO CREATE THE MORE IMPORTANT "CUBE SPHERE" CLASS, SEE THAT CLASS
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneGrid : MonoBehaviour
{
    private Vector3[] verticies;

    private Mesh mesh;

    
    public void Generate(int width, int height){
        
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Grid";

        verticies = new Vector3[(width + 1) * (height + 1)];
        Vector2[] uv = new Vector2[verticies.Length];
        Vector4[] tangents = new Vector4[verticies.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for(int i = 0, y = 0; y <= height; y++){
            for(int x = 0; x <= width; x++, i++){
                verticies[i] = new Vector3(x,y);
                uv[i] = new Vector2((float)x/width, (float)y/height);
                tangents[i] = tangent;
            }
        }

        int[] triangles = new int[width * height * 6];
        for(int ti = 0, vi = 0, y = 0; y < height; y++, vi++){
            for(int x = 0; x < width; x++, ti += 6, vi++){
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + width + 1;
                triangles[ti + 5] = vi + width + 2;
            }
        }
        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.tangents = tangents;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        
    }

    // private void OnDrawGizmos(){
    //     if(verticies == null){
    //         return;
    //     }
    //     Gizmos.color = Color.black;
    //     for(int i = 0; i < verticies.Length; i++){
    //         Gizmos.DrawSphere(verticies[i], 0.1f);
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
