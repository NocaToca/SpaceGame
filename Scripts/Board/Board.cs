using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public static int width = 6;
    public static int height = 6;

    public static Color highlightColor = Color.magenta;

    static bool RequestUpdate = false;

    static HexCells cellGen;

    static HexObject[,] hexes;
    
    bool release;

    [HideInInspector]
    public static List<Empire> empires;

    public static List<List<Hex>> tilesOfEmpires = new List<List<Hex>>();
    
    public static List<List<Unit>> unitsOfEmpires = new List<List<Unit>>();

    public static List<Unit> units;

    void Awake(){
        cellGen = GetComponentInChildren<HexCells>();
        cellGen.width = width;
        cellGen.height = height;
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
        GameMode.AddEmpireResource(new Resources());

    }

    void SpawnEmpires(){
        int empireCount = 1;
        empires = new List<Empire>();
        for(int i = 0; i < empireCount; i++){
            empires.Add(new Empire(new Color(1.0f, .647f, 0.0f), i));
            List<Hex> empHex = new List<Hex>();
            tilesOfEmpires.Add(empHex);
            List<Unit> empUnit = new List<Unit>();
            unitsOfEmpires.Add(empUnit);
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

        TakeControl(systemHexes[ran].hex, empires[0]);

        Ship[] addingUnits = new Ship[2];
        addingUnits[0] = new ColonyShip(empires[0], GetHexPosition(systemHexes[ran].hex));
        addingUnits[1] = new ProtectorShip(empires[0], GetHexPosition(systemHexes[ran].hex));

        unitsOfEmpires[0].Add(addingUnits[0]);
        unitsOfEmpires[0].Add(addingUnits[1]);


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

	public static void TakeControl(Hex hex, Empire empire){
        int index = Board.GetEmpireNumber(empire);
        tilesOfEmpires[index].Add(hex);
        cellGen.makeCells();
    }
	
	public Hex GetHexNearestToPos(Vector3 pos){
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

    public static void RequestMovement(Hex endingHex, Ship ship){
        int index = 0;
        Hex startingHex = GetHexShipOn(ship);
        Hex[] hexesInPath = HexBasedAStar.AStar(startingHex.referenceObject, endingHex.referenceObject, height, width);
        Hex[] hexPathCopy = new Hex[hexesInPath.Length];
        for(int i = hexesInPath.Length - 1; i >= 0; i--){
            hexPathCopy[index] = hexesInPath[i];
            index++;
        }
        if(ship.availableMovementPoints >= hexPathCopy.Length){
            Move(hexPathCopy[hexPathCopy.Length-1], ship);
            ship.availableMovementPoints -= hexPathCopy.Length-1;
        } else {
            Move(hexPathCopy[ship.availableMovementPoints], ship);
            ship.availableMovementPoints = 0;
        }
        RequestUpdate = true;
    }

    public static void Move(Hex hex, Ship ship){
        ship.ShipPosition = GetHexPosition(hex);
    }

    public static Empire GetEmpireThatControlsHex(Hex hex){
        int index = -1;
        for(int i = 0; i < empires.Count; i++){
            for(int k = 0; k < tilesOfEmpires[i].Count; k++){
                if(tilesOfEmpires[i][k] == hex){
                    index = i;
                }
            }
        }
        if(index != -1){
            return empires[index];
        } else {
            Debug.LogError("You have called for a hex that isn't owned by an empire!");
        }
        return null;
    }

    public static int GetEmpireNumber(Empire empire){
        for(int i = 0; i < empires.Count; i++){
            if(empires[i] == empire){
                return i;
            }
        }
        Debug.LogError("You called for an empire that the board doesn't recognize! Was it perhaps deleted by mistake?");
        return -1;
    }

    public static void ColonizePlanet(Empire empire, Hex hex, int index){
        SystemHex sysHex = (SystemHex)hex;
        if(sysHex != null){
            sysHex.AddColonizedPlanet(index);
            DestroyShip(FilterColonyShips(ShipsOnHex(hex, empire))[0]);
            int i = GetEmpireNumber(empire);
            tilesOfEmpires[i].Add(sysHex);
        } else {
            Debug.LogError("You have asked to colonize a system that doesn't exist! Was the system tile perhaps post-generated?");
        }
        cellGen.makeCells();
    }

    public static bool CheckForColonyShip(Hex hex){
        Ship[] colShips = FilterColonyShips(ShipsOnHex(hex));
        return colShips.Length >= 1;
    }

    public static bool DestroyShip(Ship ship){
        units.Remove(ship);
        for(int i = 0; i < unitsOfEmpires.Count; i++){
            if(unitsOfEmpires[i].Contains(ship)){
                unitsOfEmpires[i].Remove(ship);
                return true;
            }
        }
        return false;
    }

    public static Ship[] ShipsOnHex(Hex hex, Empire empire){
        HexCoordinates hexCoords = GetHexPosition(hex);
        return FilterShips(GetShipsOnPosition(hexCoords), empire);
    }

    public static Ship[] ShipsOnHex(Hex hex){
        HexCoordinates hexCoords = GetHexPosition(hex);
        return GetShipsOnPosition(hexCoords);
    }
    
    public static Ship[] GetShipsOnPosition(HexCoordinates hexCoords){
        List<Ship> ships = new List<Ship>();
        for(int i = 0; i < units.Count; i++){
            if(units[i] is Ship){
                Ship unit = (Ship)units[i];
                if(unit.ShipPosition.Equals(hexCoords)){
                    ships.Add(unit);
                }
            }
            
        }
        Ship[] returnShips = new Ship[ships.Count];
        for(int i = 0; i < ships.Count; i++){
            returnShips[i] = ships[i];
        }
        return returnShips;
    }

    public static Ship[] FilterShips(Ship[] ships, Empire empire){
        List<Ship> newShips = new List<Ship>();
        for(int i = 0; i < ships.Length; i++){
            if(GetShipEmpire(ships[i]) == empire){
                newShips.Add(ships[i]);
            }
        }
        Ship[] ship = new Ship[newShips.Count];
        for(int i = 0; i < ship.Length; i++){
            ship[i] = newShips[i];
        }
        return ship;
    }

    public static Ship[] FilterColonyShips(Ship[] ships){
        List<Ship> newShips = new List<Ship>();
        for(int i = 0; i < ships.Length; i++){
            if(ships[i] is ColonyShip){
                newShips.Add(ships[i]);
            }
        }
        Ship[] ship = new Ship[newShips.Count];
        for(int i = 0; i < ship.Length; i++){
            ship[i] = newShips[i];
        }
        return ship;
    }

    public static Empire GetShipEmpire(Ship ship){
        for(int i = 0; i < unitsOfEmpires.Count; i++){
            for(int k = 0; k < unitsOfEmpires[i].Count; k++){
                if(unitsOfEmpires[i][k] == ship){
                    return empires[i];
                }
            }
        }
        return null;
    }

    public static HexCoordinates GetHexPosition(Hex hex){
        int xPos = 0;
        int zPos = 0;
        for(int a = 0; a < hexes.GetLength(0); a++){
            for(int b = 0; b < hexes.GetLength(1); b++){
                if(hexes[a,b].hex == hex){
                    xPos = b;
                    zPos = a;
                }
            }
        }
        int x,y,z;
        z = zPos;
        y = (-xPos-zPos) - (-1)*(zPos/2);
        x = -y-z;
        return new HexCoordinates(x,y,z);
    }

    public static Hex GetHexShipOn(Ship ship){
        return GetHexFromHexCoords(ship.ShipPosition);
    }

    public static Hex GetHexFromHexCoords(HexCoordinates pos){
        int y = pos.y - (pos.z/2);
        int x = -y-pos.z;
        int z = pos.z;
        return hexes[z,x].hex;
    }

    public static bool IsHexControlled(Hex hex){
        for(int i = 0; i < tilesOfEmpires.Count; i++){
            if(tilesOfEmpires[i].Contains(hex)){
                return true;
            }
        }
        return false;
    }

    public static Planet GetPlanet(Hex hex, int index){
        if(hex is SystemHex){
            SystemHex sys = (SystemHex)hex;
            return sys.GetPlanet(index);
        } else {
            Debug.LogError("You requested planet data from a tile that has no planet!");
        }
        return null;

    }

    public static List<Planet> GetPlanets(Empire empire){
        Hex[] hexes = FilterHexes(1, empire);
        SystemHex[] sysHexes = new SystemHex[hexes.Length];
        for(int i = 0; i < hexes.Length; i++){
            sysHexes[i] = (SystemHex)hexes[i];
        }

        List<Planet> planets = new List<Planet>();
        for(int i = 0; i < sysHexes.Length; i++){
            int length = sysHexes[i].GetNumberOfPlanets();
            for(int k = 0; k < length; k++){
                planets.Add(GetPlanet(sysHexes[i], k));
            }
        }
        return planets;
    }

    public static Resources GetEmpiresGeneration(Empire empire){
        return GetPlanetGeneration(empire);
    }

    static List<Planet> FilterForColonizedPlanets(List<Planet> planets){
        List<Planet> col = new List<Planet>();
        foreach(Planet planet in planets){
            if(planet.Colonized){
                col.Add(planet);
            }
        }
        return col;
    }

    static Resources GetPlanetGeneration(Empire empire){
        List<Planet> planets = GetPlanets(empire);
        planets = FilterForColonizedPlanets(planets);

        Resources ResourcesGained = new Resources();
        for(int i = 0; i < planets.Count; i++){
            ResourcesGained.Add(planets[i].GetResourceProduction());
        }

        return ResourcesGained;
    }

    public static Hex[] FilterHexes(int mode, Empire empire){
        int index = GetEmpireNumber(empire);
        if(mode == 0){
            return FilterSpaceHex(index);
        }else 
        if(mode == 1){
            return FilterSystemHex(index);
        } else {
            Debug.LogError("You have tried filtering for a tile not supported!");
        }
        return null;
    }
    static Hex[] FilterSpaceHex(int index){
        List<Hex> list = new List<Hex>();
        for(int i = 0; i < tilesOfEmpires[index].Count; i++){
            if(tilesOfEmpires[index][i] is SpaceHex){
                list.Add(tilesOfEmpires[index][i]);
            }
        }
        Hex[] hexes = new Hex[list.Count];
        for(int i = 0; i < hexes.Length; i++){
            hexes[i] = list[i];
        }
        return hexes;
    }
    static Hex[] FilterSystemHex(int index){
        List<Hex> list = new List<Hex>();
        for(int i = 0; i < tilesOfEmpires[index].Count; i++){
            if(tilesOfEmpires[index][i] is SystemHex){
                list.Add(tilesOfEmpires[index][i]);
            }
        }
        Hex[] hexes = new Hex[list.Count];
        for(int i = 0; i < hexes.Length; i++){
            hexes[i] = list[i];
        }
        return hexes;
    }

    public static Empire GetPlayerEmpire(){
        return empires[0];
    }

    public static Empire GetEmpireOwningPlanet(Planet planet){
        foreach(Empire empire in empires){
            List<Planet> planets = GetPlanets(empire);

            for(int i = 0; i < planets.Count; i++){
                if(planets[i] == planet){
                    return empire;
                }
            }
        }
        Debug.LogError("You requested the controlling empire of a planet that isnt colonized!");
        return null;
    }

    public static void ResetShipMovementPoints(Empire empire){
        List<Ship> ships = GetShipsOfEmpire(empire);
        foreach(Ship ship in ships){
            ship.ResetMovement();
        }
    }

    public static List<Ship> GetShipsOfEmpire(Empire empire){
        List<Ship> ships = new List<Ship>();

        int index = GetEmpireNumber(empire);

        for(int i = 0; i < unitsOfEmpires[index].Count; i++){
            if(unitsOfEmpires[index][i] is Ship){
                ships.Add((Ship)unitsOfEmpires[index][i]);
            }
        }
        return ships;
    }
}
