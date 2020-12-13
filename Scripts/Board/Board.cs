using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    //The dimensions that the board will take
    public static int width = 6;
    public static int height = 6;

    //When tiles are selected/want to be seen, use this color
    public static Color highlightColor = Color.magenta;

    //Whether or not we are requesting an update from the cell gen mesh
    static bool RequestUpdate = false;

    //The generation class responsible for editing and making the hexes (cells)
    static HexCells cellGen;

    //The array of the objects representing the hex; obtained by the HexCells class
    static HexObject[,] hexes;
    
    //This bool is to control the functions started by mouse input to make sure that it doesnt activate again until the clicked is released
    bool release;

    //The list of empires in order of their turns
    public static List<Empire> empires;

    //The tiles controlled by empires, with empire being parrallel with the empire array (IE empire[0] = tilesOfEmpires[0])
    public static List<List<Hex>> tilesOfEmpires = new List<List<Hex>>();
    
    //In similar fashion with the above list, this array holds the units controlled by each empire
    public static List<List<Unit>> unitsOfEmpires = new List<List<Unit>>();

    //This list of units in play during the game
    public static List<Unit> units;

/***************************************************************************Starting Functions**********************************************************************/
    //In the awake function, we want to get the cell generation component for use and set the width and height, along with instantiating the units list
    void Awake(){
        cellGen = GetComponentInChildren<HexCells>();
        cellGen.width = width;
        cellGen.height = height;
        units = new List<Unit>();
    }

    // Start is called before the first frame update
    //We obtain the hex array and initialize each hex here, along with initializing the empire resources after spawning them
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

    //Spawns and instantiates the empires in the game
    void SpawnEmpires(){

        ///How many empires we will want to spawn, static until further implemtation 
        int empireCount = 1;
        empires = new List<Empire>();

        //Spawning each empire and setting up their cooresponding lists
        for(int i = 0; i < empireCount; i++){
            empires.Add(new Empire(new Color(1.0f, .647f, 0.0f), i));
            List<Hex> empHex = new List<Hex>();
            tilesOfEmpires.Add(empHex);
            List<Unit> empUnit = new List<Unit>();
            unitsOfEmpires.Add(empUnit);
        }

        //Finding every system hex in our tile array
        List<HexObject> systemHexes = new List<HexObject>();
        for(int y = 0; y < hexes.GetLength(0); y++){
            for(int x = 0; x < hexes.GetLength(1); x++){
                if(hexes[y,x].hex is SystemHex){
                    systemHexes.Add(hexes[y,x]);
                }
            }
        }

        //And now we take control of a random system
        int ran = Random.Range(0, systemHexes.Count);
        TakeControl(systemHexes[ran].hex, empires[0]);

        //Creating and giving the current empire their starting units
        Ship[] addingUnits = new Ship[2];
        addingUnits[0] = new ColonyShip(empires[0], GetHexPosition(systemHexes[ran].hex));
        addingUnits[1] = new ProtectorShip(empires[0], GetHexPosition(systemHexes[ran].hex));

        unitsOfEmpires[0].Add(addingUnits[0]);
        unitsOfEmpires[0].Add(addingUnits[1]);


        //Cast is just so I can get the Colonize function and colonize the empire's starting planet
        if(systemHexes[ran].hex is SystemHex){
            SystemHex sysHex = (SystemHex)systemHexes[ran].hex;
            sysHex.planets[0].Colonize(empires[0]);
        }
        

        units.Add(addingUnits[0]);
        units.Add(addingUnits[1]);

        cellGen.makeCells();

    }

/************************************************************************Hex Functions*************************************************************************/
//Any of the functions that are used to obtain hexes

//Takes in the position of a point in world space and runs through every hex's center to determine which is closest (up for optimization)
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

    //From an x,y coordinate pertaining to the 2d grid of our hex objects
    public static HexObject GetHex(Vector2 vect){
        //Debug.Log(vect.x + "," + vect.y);
        return hexes[(int)vect.y,(int)vect.x];
    }

    //Using x,y,z hex coordinates, we return the coorisponding hex in our 2d grid
    public static Hex GetHexFromHexCoords(HexCoordinates pos){
        int y = pos.y - (pos.z/2);
        int x = -y-pos.z;
        int z = pos.z;
        return hexes[z,x].hex;
    }

    //Using the hex position of the ship, we just return the hex the ship is on
    public static Hex GetHexShipOn(Ship ship){
        return GetHexFromHexCoords(ship.ShipPosition);
    }

    //A filter function to return a certain number of hexes 
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

    //Basically we just find out how many type of the hexes there are and then assign them to an array to return
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

    /******************************************************************Functions Regarding Units*******************************************************/
    //The main function to handle ship movement throughout the board. Initially called by the canvas controller class
    public static void RequestMovement(Hex endingHex, Ship ship){
        int index = 0;
        Hex startingHex = GetHexShipOn(ship);
        Hex[] hexesInPath = HexBasedAStar.AStar(startingHex.referenceObject, endingHex.referenceObject, height, width);
        Hex[] hexPathCopy = new Hex[hexesInPath.Length];
        
        //For easy accesability, we need to swap the order of the hex path so its from start->finish rather than finish->start
        for(int i = hexesInPath.Length - 1; i >= 0; i--){
            hexPathCopy[index] = hexesInPath[i];
            index++;
        }

        //We need to determine if the ship can move the whole path and handle the situation accordingly
        if(ship.availableMovementPoints >= hexPathCopy.Length){
            Move(hexPathCopy[hexPathCopy.Length-1], ship);
            ship.availableMovementPoints -= hexPathCopy.Length-1;
        } else {
            Move(hexPathCopy[ship.availableMovementPoints], ship);
            ship.availableMovementPoints = 0;
        }
        RequestUpdate = true;
    }

    //Moves a ship to the according position
    public static void Move(Hex hex, Ship ship){
        ship.ShipPosition = GetHexPosition(hex);
    }

    //Checks if theres a colony ship on the tile
    public static bool CheckForColonyShip(Hex hex){
        Ship[] colShips = FilterColonyShips(ShipsOnHex(hex));
        return colShips.Length >= 1;
    }

    //Destroys a specific ship. Returns true if the ship was in an empire are it was a nomad empire
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

    //Getting the ships ships on the hex controlled by a certain empire
    public static Ship[] ShipsOnHex(Hex hex, Empire empire){
        HexCoordinates hexCoords = GetHexPosition(hex);
        return FilterShips(GetShipsOnPosition(hexCoords), empire);
    }

    //Returning all of the ships on a hex
    public static Ship[] ShipsOnHex(Hex hex){
        HexCoordinates hexCoords = GetHexPosition(hex);
        return GetShipsOnPosition(hexCoords);
    }
    
    //From hex coords x,y,z, we obtain the ships on the position
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

    //Filtering ships off of an empire
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

    //Filtering for the colony ships on a tile
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

    //Reseting the ship movement points of each empire as they end their turns
    public static void ResetShipMovementPoints(Empire empire){
        List<Ship> ships = GetShipsOfEmpire(empire);
        foreach(Ship ship in ships){
            ship.ResetMovement();
        }
    }

    //Obtaining all of the ships controlled by an empire out of their list of units
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

    /**********************************************************************************Planet Functions*******************************************************/

    //Colonizes the specific planet on the hex for the empire given; throws an error if the hex is not a system hex
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

    //Using the planet number and hex that it is on, return the planet referring to the index (index higher than the planet array length are accepted, but indexes lower than 0 will cause a seperate error)
    //Errors if the hex we inputted is not a system hex
    public static Planet GetPlanet(Hex hex, int index){
        if(hex is SystemHex){
            SystemHex sys = (SystemHex)hex;
            return sys.GetPlanet(index);
        } else {
            Debug.LogError("You requested planet data from a tile that has no planet!");
        }
        return null;

    }

    //Obtains all of the planets controlled by an empire
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

    //Having a list of planets, we return the planets that are colonized
    static List<Planet> FilterForColonizedPlanets(List<Planet> planets){
        List<Planet> col = new List<Planet>();
        foreach(Planet planet in planets){
            if(planet.Colonized){
                col.Add(planet);
            }
        }
        return col;
    }

    //Obtains the resources outputted by all of the planets in an empire
    static Resources GetPlanetGeneration(Empire empire){
        List<Planet> planets = GetPlanets(empire);
        planets = FilterForColonizedPlanets(planets); //we only want colonized planets to generate resources

        Resources ResourcesGained = new Resources();
        for(int i = 0; i < planets.Count; i++){
            ResourcesGained.Add(planets[i].GetResourceProduction());
        }

        return ResourcesGained;
    }

/************************************************************************************Empire functions****************************************************************/
//Most of the logic surrounding empires is done in the GameMode class, but the board class has all of the information regarding the empire's doings and control

    //Gives control of a hex to an empire
	public static void TakeControl(Hex hex, Empire empire){
        int index = Board.GetEmpireNumber(empire);
        tilesOfEmpires[index].Add(hex);
        cellGen.makeCells();
    }

    //Returns the empire that controls this hex; errors if the hex has no controlling empire
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

    //All of the list here require the associated number in the empires array; this function returns the number associated with that empire in regards to the array. Errors if the empire is not in the array
    public static int GetEmpireNumber(Empire empire){
        for(int i = 0; i < empires.Count; i++){
            if(empires[i] == empire){
                return i;
            }
        }
        Debug.LogError("You called for an empire that the board doesn't recognize! Was it perhaps deleted by mistake?");
        return -1;
    }

    //Returns the empire of the inputted ship; returns null if the ship has no empire (which would be a nomadic empire)
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

    //Returns whether or not a hex is controlled by an empire
    public static bool IsHexControlled(Hex hex){
        for(int i = 0; i < tilesOfEmpires.Count; i++){
            if(tilesOfEmpires[i].Contains(hex)){
                return true;
            }
        }
        return false;
    }

    //Returns the resources outputted by the empire. Currently only considers planet generation (ship upkeep is definitely going to be implemented)
    public static Resources GetEmpiresGeneration(Empire empire){
        return GetPlanetGeneration(empire);
    }

    //Returns the empire of the player. For multiplayer implementation this should return the empire of the client but that implementation is far out. For now, the player is always going to be the first empire
    public static Empire GetPlayerEmpire(){
        return empires[0];
    }

    //Taking in the planet, returns the empire that controls it. Errors if there is no empire controlling it
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

/***************************************************************************Coordinate Functions***************************************/
//Since we have to convert our hex coordinates to grid coordinates and vice versa, it'd be helpful to have these functions to make things easier

    //Returns the hex coordinates of the hex
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
    
    //Returns a Vector2 of the position of the hex in our 2d grid
    public static Vector2 FindHexCoordsInBoard(Hex hex){

        for(int z = 0; z < hexes.GetLength(0); z++){
            for(int x = 0; x < hexes.GetLength(1); x++){
                if(hex == hexes[z,x].hex){
                    return new Vector2(x, z);
                }
            }
        }
        return new Vector2(-1, -1); //While this could be logged as an error, I still havent decided if I want hexes in another board outside of the galaxy (like the L-Cluster in Stellaris)
    }

/************************************************************************************Misc Functions****************************************************************/
//Sometimes, we just have functions that don't fit into any category!

    //Update is called every frame. We just check if we need to update the cell mesh (should only happen when a change in the HexObjects occurs)
    void Update () {
		if(RequestUpdate){
            cellGen.makeCells();
        }
        
	}

    //Highlights hexes a certain colour.
    public static void DebugHighlight(HexObject hex){
        cellGen.makeCells();
    }

    //Simple return functions for our non-static width and height 
    public int GetHeight(){
        return cellGen.height;
    }
    public int GetWidth(){
        return cellGen.width;
    }
}
