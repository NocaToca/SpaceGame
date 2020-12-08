using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public static int width = 6;
    public static int height = 6;

    public static Color spaceColor = Color.black;
	public static Color systemColor = Color.white;
    public static Color highlightColor = Color.magenta;

    static bool RequestUpdate = false;

    static HexCells cellGen;

    static HexObject[,] hexes;
    
    bool release;

    [HideInInspector]
    public List<Empire> empires;

    public List<Unit> units;

    void Awake(){
        cellGen = GetComponentInChildren<HexCells>();
        cellGen.spaceColor = spaceColor;
        cellGen.systemColor = systemColor;
        units = new List<Unit>();
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

        Ship[] addingUnits = new Ship[2];
        addingUnits[0] = new ColonyShip(empires[0], systemHexes[ran].hex);
        addingUnits[1] = new ProtectorShip(empires[0], systemHexes[ran].hex);

        systemHexes[ran].hex.ShipsOnHex.Add(addingUnits[0]);
        systemHexes[ran].hex.ShipsOnHex.Add(addingUnits[1]);


        //Woah! I didnt know I could do this! I have to edit a lot of code for this now
        if(systemHexes[ran].hex is SystemHex){
            SystemHex sysHex = (SystemHex)systemHexes[ran].hex;
            sysHex.planets[0].Colonize(empires[0]);
        }
        

        units.Add(addingUnits[0]);
        units.Add(addingUnits[1]);

        cellGen.makeCells();

    }

    void Update () {
		if(RequestUpdate){
            cellGen.makeCells();
        }
        
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

    public static void DebugHighlight(HexObject hex){
        hex.color = highlightColor;
        cellGen.makeCells();
    }

    public static HexObject GetHex(Vector2 vect){
        //Debug.Log(vect.x + "," + vect.y);
        return hexes[(int)vect.y,(int)vect.x];
    }

    public static Vector2 FindHexCoordsInBoard(Hex hex){

        for(int z = 0; z < hexes.GetLength(0); z++){
            for(int x = 0; x < hexes.GetLength(1); x++){
                if(hex == hexes[z,x].hex){
                    return new Vector2(x, z);
                }
            }
        }
        return new Vector2(-1, -1);
    }

    public int GetHeight(){
        return cellGen.height;
    }
    public int GetWidth(){
        return cellGen.width;
    }

    public static void RequestMovement(Hex startingHex, Hex endingHex, Ship ship){
        int index = 0;
        Hex[] hexesInPath = HexBasedAStar.AStar(startingHex.referenceObject, endingHex.referenceObject, height, width);
        Hex[] hexPathCopy = new Hex[hexesInPath.Length];
        for(int i = hexesInPath.Length - 1; i >= 0; i--){
            hexPathCopy[index] = hexesInPath[i];
            index++;
        }
        if(ship.movementPoints >= hexPathCopy.Length){
            ship.Move(hexPathCopy[hexPathCopy.Length-1]);
        } else {
            ship.Move(hexPathCopy[ship.movementPoints]);
        }
        RequestUpdate = true;
    }
}
