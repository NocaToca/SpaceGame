using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public Color spaceColor = Color.black;
	public Color systemColor = Color.white;

    HexCells cellGen;

    HexObject[,] hexes;
    
    bool release;

    [HideInInspector]
    public List<Empire> empires;

    void Awake(){
        cellGen = GetComponentInChildren<HexCells>();
        cellGen.spaceColor = spaceColor;
        cellGen.systemColor = systemColor;
    }

    // Start is called before the first frame update
    void Start(){

        hexes = cellGen.GetHexes();
        for(int y = 0; y < hexes.GetLength(0); y++){
            for(int x = 0; x < hexes.GetLength(1); x++){
                hexes[y,x].hex.Initialize();
            }
        }

        SpawnEmpires();

    }

    void SpawnEmpires(){
        int empireCount = 1;
        empires = new List<Empire>();
        for(int i = 0; i < empireCount; i++){
            empires.Add(new Empire(new Color(1.0f, .647f, 0.0f)));
        }

        List<HexObject> systemHexes = new List<HexObject>();
        for(int y = 0; y < hexes.GetLength(0); y++){
            for(int x = 0; x < hexes.GetLength(1); x++){
                if(hexes[y,x].hex.GetType() == "System"){
                    systemHexes.Add(hexes[y,x]);
                }
            }
        }
        int ran = Random.Range(0, systemHexes.Count);

        systemHexes[ran].hex.TakeControl(empires[0]);
        systemHexes[ran].color = empires[0].empireColor;

        cellGen.makeCells();

    }

    void Update () {
		
        
	}

	
	
	public Hex GetHexNearestToPos(Vector3 pos){
        //Debug.Log("Touch");
		//position = transform.InverseTransformPoint(position);
		//HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		//Debug.Log("touched at " + coordinates.ToString());
        //Vector3 pos = hit.collider.gameObject.transform.position;
        if(pos != null){
            int index = 0;
            int savedIndexX = 0;
            int savedIndexY = 0;

            int timeSinceLastChange = 0;

            float prevD = 9999999.0f;

            bool breaking = false;

            float min = 99999999.0f;
            for(int z = 0; z < hexes.GetLength(0); z++){
                for(int x = 0; x < hexes.GetLength(1); x++){
                   timeSinceLastChange++;
                   
                    float d = Mathf.Sqrt(
                        Mathf.Pow((hexes[z, x].transform.position.x) - pos.x, 2) +
                        Mathf.Pow((hexes[z, x].transform.position.y) - pos.y, 2) +
                        Mathf.Pow((hexes[z, x].transform.position.z) - pos.z, 2)); 
                    if(min > d){
                        min = d;
                        savedIndexX = x;
                        savedIndexY = z;
                        timeSinceLastChange = 0;
                        prevD = d;
                    }
                    // } else if(d > prevD){
                    //     index += hexes.GetLength(1) - x;
                    //     break;
                    // }
                    index++;
                    
                }

                if(breaking){
                    break;
                }
               
            }
            return hexes[savedIndexY, savedIndexX].hex;
            
        } else {
            Debug.LogError("Position recieved to return hex is null");
            return null;
        }
        

        
	}
}
