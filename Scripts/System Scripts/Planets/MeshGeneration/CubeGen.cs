using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Generates a cube. THIS IS NOT USED, CUBE SPHERE IS THE SPHERE I USE
public class CubeGen : MonoBehaviour
{
    public int width;
    public int height;

    PlaneGrid planeGridPrefab; 

    PlaneGrid[] planeGrids;

    void Awake(){
        planeGridPrefab = GetComponentInChildren<PlaneGrid>();
        if(planeGridPrefab == null){
            Debug.LogError("There is no plane grid that is a child of the cube generator! We can't generate the cube!");
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        planeGrids = new PlaneGrid[6];
        for(int i = 0; i < 6; i++){
            planeGrids[i] = Instantiate(planeGridPrefab);
            planeGrids[i].gameObject.SetActive(true);
            planeGrids[i].Generate(width, height);
        }

        planeGrids[1].transform.Rotate(0, 180, 0);
        planeGrids[1].transform.position = new Vector3(width, 0, height);

        planeGrids[2].transform.Rotate(90, 0, 0);
        planeGrids[2].transform.position = new Vector3(0, height, 0);

        planeGrids[3].transform.Rotate(270, 0, 0);
        planeGrids[3].transform.position = new Vector3(0, 0, width);

        planeGrids[4].transform.Rotate(0, 90, 0);
        planeGrids[4].transform.position = new Vector3(0, 0, width);

        planeGrids[5].transform.Rotate(0, 270, 0);
        planeGrids[5].transform.position = new Vector3(width, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
