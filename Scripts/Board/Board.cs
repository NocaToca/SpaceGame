using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    //The dimensions that the board will take
    public static int width = 6;
    public static int height = 6;

    public int SetWidth = 6;
    public int SetHeight = 6;

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
    public static List<List<Fleet>> fleetsOfEmpires = new List<List<Fleet>>();

    //This list of units in play during the game
    public static List<Unit> units;

    public SystemStorage systemStorage;

    public Material Atmosphere;
	public Material Planet;
	public Material Clouds;
	public Material Star;

    public static bool initialized = false;

    static bool RequestShipUpdate = false;
    static bool getReadyToUpdate = false;


/***************************************************************************Starting Functions**********************************************************************/
    //In the awake function, we want to get the cell generation component for use and set the width and height, along with instantiating the units list
    void Awake(){
        if(!initialized){
            width = SetWidth;
            height = SetHeight;
            cellGen = GetComponentInChildren<HexCells>();
            cellGen.width = width;
            cellGen.height = height;
            cellGen.Initialize();
            units = new List<Unit>();
            CubeSphere.Atmosphere = Atmosphere;
            CubeSphere.Planet = Planet;
            CubeSphere.Clouds = Clouds;
            CubeSphere.Star = Star;
        } else {
            
        }
        
    }

    // Start is called before the first frame update
    //We obtain the hex array and initialize each hex here, along with initializing the empire Resource after spawning them
    //If we already initialized the board, we dont want to initialize it again! So we just generate what we had previously
    void Start(){
        if(!initialized){
            hexes = cellGen.GetHexes();
            for(int y = 0; y < hexes.GetLength(0); y++){
                for(int x = 0; x < hexes.GetLength(1); x++){
                    hexes[y,x].hex.Initialize();
                }
            }

            SpawnEmpires();
            SpawnNomadUnits();
            GameMode.AddEmpireResource(new Resource());
            GameMode.InitializeTechs(empires.Count);

            SystemStorage.InitializeSystemStorage(width, height, hexes);
            initialized = true;
            RequestUpdate = true;
        } else {
            width = SetWidth;
            height = SetHeight;

            cellGen = GetComponentInChildren<HexCells>();
            cellGen.width = width;
            cellGen.height = height;

            cellGen.SetHexes(hexes);
            cellGen.makeCells();

            //hexes.Clear();
            hexes = null;

            hexes = cellGen.GetHexes();

            ReassignRefrenceObjects();
        }
        
    }

    private void ReassignRefrenceObjects(){
        foreach(HexObject hex in hexes){
            hex.hex.referenceObject = hex;
        }
    }

    //Spawns and instantiates the empires in the game
    void SpawnEmpires(){

        ///How many empires we will want to spawn, static until further implemtation 
        int empireCount = 2;
        empires = new List<Empire>();
        List<Color> empColors = new List<Color>();
        empColors.Add(new Color(1.0f, .67f, 0.0f));
        empColors.Add(new Color(.192f, .929f, .309f));

        //Spawning each empire and setting up their cooresponding lists
        for(int i = 0; i < empireCount; i++){
            empires.Add(new Empire(empColors[i], i));
            List<Hex> empHex = new List<Hex>();
            tilesOfEmpires.Add(empHex);
            List<Unit> empUnit = new List<Unit>();
            unitsOfEmpires.Add(empUnit);
            GameMode.SetEmpireFleetLimit(empires[i]);
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

        //Cast is just so I can get the S function and colonize the empire's starting planet
        if(systemHexes[ran].hex is SystemHex){
            SystemHex sysHex = (SystemHex)systemHexes[ran].hex;
            sysHex.planets[0].Colonize(empires[0]);
        }

        units.Add(addingUnits[0]);
        units.Add(addingUnits[1]);

        systemHexes.Remove(systemHexes[ran]);
        ran = Random.Range(0, systemHexes.Count);
        TakeControl(systemHexes[ran].hex, empires[1]);

        addingUnits = new Ship[2];
        addingUnits[0] = new ColonyShip(empires[1], GetHexPosition(systemHexes[ran].hex));
        addingUnits[1] = new ProtectorShip(empires[1], GetHexPosition(systemHexes[ran].hex));

        unitsOfEmpires[1].Add(addingUnits[0]);
        unitsOfEmpires[1].Add(addingUnits[1]);

        //Cast is just so I can get the S function and colonize the empire's starting planet
        if(systemHexes[ran].hex is SystemHex){
            SystemHex sysHex = (SystemHex)systemHexes[ran].hex;
            sysHex.planets[0].Colonize(empires[1]);
        }

        units.Add(addingUnits[0]);
        units.Add(addingUnits[1]);

        GameMode.AddPlayer(new Player(empires[0], true, true));
        GameMode.AddPlayer(new Player(empires[1], false, false));

        cellGen.makeCells();

    }

    private void SpawnNomadUnits(){
        List<SpaceHex> spacehexes = new List<SpaceHex>();
        foreach(HexObject hex in hexes){
            if(hex.hex is SpaceHex){
                spacehexes.Add((SpaceHex)hex.hex);
            }
        }
        int numberOfNomadUnits = Random.Range(0, 15);
        for(int i = 0; i < numberOfNomadUnits; i++){
            int index = Random.Range(0, spacehexes.Count);
            units.Add(new SpaceAmoeba(GetHexPosition(spacehexes[index])));
        }
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

    public static List<Hex> GetEmpireHexes(Empire empire){
        return tilesOfEmpires[GetEmpireNumber(empire)];
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

    //Checks to see what hexes are currently on the user's screen, and then returns them
    public static List<Hex> GetHexesOnScreenWithShips(){
        if(hexes[0,0] == null){
            return new List<Hex>();
        }
        List<Hex> hexesOnScreen = new List<Hex>();
        List<HexObject> hexesWithUnits = new List<HexObject>();
        
        List<Ship> ships = GetAllShips();
        foreach(Ship ship in ships){
            HexObject hex = GetHexShipOn(ship).referenceObject;
            if(!hexesWithUnits.Contains(hex)){
                hexesWithUnits.Add(hex);
            }
        }

        foreach(HexObject hex in hexesWithUnits){
            if(hex == null){
                break;
            }
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(hex.gameObject.transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            if(onScreen){
                hexesOnScreen.Add(hex.hex);
            }
            hex.hex.referenceObject = hex;
        }

        return hexesOnScreen;
    }

    /******************************************************************Functions Regarding Units*******************************************************/
    public static void RequestMovement(Hex endingHex, Ship ship){
        RequestMovement(endingHex, new Fleet(ship));
    }
    //The main function to handle ship movement throughout the board. Initially called by the canvas controller class
    public static void RequestMovement(Hex endingHex, Fleet fleet){
        int index = 0;
        Hex startingHex = GetHexShipOn(fleet.shipsInFleet[0]);
        Hex[] hexesInPath = HexBasedAStar.AStar(startingHex.referenceObject, endingHex.referenceObject, height, width);
        Hex[] hexPathCopy = new Hex[hexesInPath.Length];
        
        //For easy accesability, we need to swap the order of the hex path so its from start->finish rather than finish->start
        for(int i = hexesInPath.Length - 1; i >= 0; i--){
            hexPathCopy[index] = hexesInPath[i];
            index++;
        }

        //We need to determine if the ship can move the whole path and handle the situation accordingly
        int movePoints = fleet.GetMovePoints();
        if(movePoints >= hexPathCopy.Length){
            Move(hexPathCopy[hexPathCopy.Length-1], fleet);
            fleet.Move(hexPathCopy.Length-1);
        } else {
            Move(hexPathCopy[movePoints], fleet);
            fleet.Move(movePoints);
        }
        RequestUpdate = true;

    }

    //Moves a ship to the according position
    public static void Move(Hex hex, Fleet fleet){
        HexCoordinates pos = GetHexPosition(hex);
        foreach(Ship ship in fleet.shipsInFleet){
            ship.ShipPosition = pos;
        }
        //MainController.canvasController.ShowUnitButtonsOnCanvas();
        RequestShipUpdate = true;
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
        List<Ship> ships;
        if(empire != null){
            ships = GetShipsOfEmpire(empire);
            
        } else {
            ships = GetNomadShips();
        }
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

    //Returns all of the ships without an empire, or Nomad ships
    public static List<Ship> GetNomadShips(){
        List<Ship> ships = new List<Ship>();
        foreach(Unit unit in units){
            if(unit is Ship){
                Ship ship = (Ship)unit;
                Empire empire = GetShipEmpire(ship);
                if(empire == null){
                    ships.Add(ship);
                }
            }
        }
        return ships;
    }

    //Returns the amount of Resource the empire unit's uses in one turn
    public static Resource GetUnitGeneration(Empire empire){
        int index = GetEmpireNumber(empire);
        Resource sum = new Resource();
        foreach(Unit unit in unitsOfEmpires[index]){
            sum.Add(unit.maintenance);
        }
        return sum;
    }

    //Builds a specific ship on the planet which is retrieved through the canvas controller 
    public static void Build(Hex hex, int index, Ship ship){
        Planet planet = GetPlanet(hex, index);
        ship.ShipPosition = GetHexPosition(hex);
        ship.OwningEmpire = GetEmpireOwningPlanet(planet);
        planet.GetStarport().AddToQueue(ship);
        //CanvasController.Clear();
    }

    //Adds a ship to the list of units that we have. Used mostly for newly built ships
    public static void AddShip(Ship ship, Empire empire){
        int index = GetEmpireNumber(empire);
        unitsOfEmpires[index].Add(ship);
        units.Add(ship);
    }

    //Get's all ships out of the units array
    public static List<Ship> GetAllShips(){
        List<Ship> ships = new List<Ship>();
        //foreach(List<Unit> shipList in units){
            foreach(Unit unit in units){
                if(unit is Ship){
                    Ship ship = (Ship)unit;
                    ships.Add(ship);
                }
            }
        //}
        return ships;
    }

    //Makes the AI move each nomad ship
    public static void MoveNomadShips(){
        foreach(Unit unit in units){
            if(unit is Ship){
                Ship ship = (Ship)unit;
                if(GetShipEmpire(ship) == null){
                    AI.MoveShip(ship);
                }
            }
        }
    }

    //If there are two opposing ships on a tile, you can have them fight
    public static void Fight(){
        //First we need to get all of the ships
        Ship[] ships = GetShipsOnPosition(GetHexPosition(MainController.displayingHex));
        List<Ship> sideOne = new List<Ship>();
        List<Ship> sideTwo = new List<Ship>();
        sideOne.Add(ships[0]);

        //A currently niave approach, but we assume that whatever ship isnt with the empire of the first ship is the enemy
        for(int i = 1; i < ships.Length; i++){
            if(GetShipEmpire(ships[i]) == GetShipEmpire(sideOne[0])){
                sideOne.Add(ships[i]);
            } else {
                sideTwo.Add(ships[i]);
            }
        }

        //Then, for each ship, we do the damage output
        foreach(Ship attackingShip in sideOne){
            foreach(Ship defendingShip in sideTwo){
                defendingShip.health -= attackingShip.damage;
                if(defendingShip.health <= 0){
                    DestroyShip(defendingShip);
                }
            }
        }
        foreach(Ship attackingShip in sideTwo){
            foreach(Ship defendingShip in sideOne){
                defendingShip.health -= attackingShip.damage;
                if(defendingShip.health <= 0){
                    DestroyShip(defendingShip);
                }
            }
        }

        //Incase any ships died in this battle, we need to recall the canvas so it doesn't display dead ships
        MainController.RequestHexRecall();
    }

    //This function is much like the above function, but it is called from outside of the canvas (most likely the AI), so we know what hex we're on
    public static void Fight(Ship attackingShip, Ship shipBeingAttacked){
        Ship[] ships = GetShipsOnPosition(attackingShip.ShipPosition);
        Fleet fleetOne = GetFleetFromShip(attackingShip);
        Fleet fleetTwo = GetFleetFromShip(shipBeingAttacked);
        if(fleetOne == null || fleetTwo == null){
            if(fleetOne == null){
                List<Ship> ship = new List<Ship>();
                ship.Add(attackingShip);
                fleetOne = new Fleet(ship);
            }
            if(fleetTwo == null){
                List<Ship> ship = new List<Ship>();
                ship.Add(shipBeingAttacked);
                fleetTwo = new Fleet(ship);
            }
        }

        float attackingDamageDealt = fleetOne.CalculateAttackDamage();
        fleetTwo.DealDamage(attackingDamageDealt);

        float defendingDamageDealt = fleetTwo.CalculateDefenseDamage();
        fleetOne.DealDamage(defendingDamageDealt);

    }

    public static Ship GetOpposingShipOnHex(Ship ship){
        Ship[] ships = GetShipsOnPosition(ship.ShipPosition);
        Empire empire = GetShipEmpire(ship);
        foreach(Ship ship2 in ships){
            if(GetShipEmpire(ship2) != empire){
                return ship2;
            }
        }
        return null;
    }

    //Checks to see if the hex has fleets from two different empires on it currently. No allies!
    public static bool DoesHexHaveOpposingFleets(Hex hex){
        Ship[] ships = ShipsOnHex(hex);
        Empire empire = GetShipEmpire(ships[0]);
        foreach(Ship ship in ships){
            if(GetShipEmpire(ship) != empire){
                return true;
            }
        }
        return false;
    }

    /**********************************************************************************Functions for Fleets***************************************************/
    public static Fleet GetFleetFromShip(Ship ship){
        int empire = GetEmpireNumber(GetShipEmpire(ship));
        if(empire == -1){
            return MakeNomadFleet(ship);
        }
        return GetShipFleet(empire, ship);
    }

    public static Fleet MakeNomadFleet(Ship ship){
        Ship[] ships = GetShipsOnPosition(ship.ShipPosition);
        List<Ship> nomadShips = GetNomadShips();
        List<Ship> nomadShipsOnTile = new List<Ship>();

        foreach(Ship ship2 in ships){
            if(nomadShips.Contains(ship2)){
                nomadShipsOnTile.Add(ship2);
            }
        }

        return new Fleet(nomadShipsOnTile);
    }

    public static void CreateFleet(int empire, List<Ship> ships){
        if(!ValidateNewFleet(empire, ships)){
            AddToFleet(empire, ships);
            return;
        }
    }

    public static bool ValidateNewFleet(int empire, List<Ship> ships){
        //foreach(List<Fleet> fleetList in fleetsOfEmpires[empire]){
            foreach(Fleet fleet in fleetsOfEmpires[empire]){
                foreach(Ship ship in ships){
                    if(fleet.shipsInFleet.Contains(ship)){
                        return false;
                    }
                }
            }
        //}
        return true;
    }

    public static void AddToFleet(int empire, List<Ship> ships){
        List<Ship> newShips = new List<Ship>();
        //int checkSpace = 0;
        int fleetNumber = fleetsOfEmpires[empire].Count;
        int numFleetsCombined = 0;
        List<int> fleetIndexes = new List<int>();

        foreach(Ship ship in ships){
            Fleet fleet = GetShipFleet(empire, ship);
            if(fleet != null){
                int num = GetFleetNumber(empire, fleet);
                if(num < fleetNumber && num >= 0){
                    fleetNumber = num;
                    numFleetsCombined++;
                    fleetIndexes.Add(num);
                }
                foreach(Ship ship2 in fleet.shipsInFleet){
                    if(!newShips.Contains(ship2)){
                        newShips.Add(ship2);
                    }
                }
            }
            if(!newShips.Contains(ship)){
                newShips.Add(ship);
            }
            if(!ValidateFleetSize(newShips, empire)){
                return;
            }
        }

        if(fleetIndexes.Count > 1){
            fleetNumber = HandleFleetIndexes(fleetIndexes, empire);
        }

        Fleet newFleet = new Fleet(newShips);

        if(fleetNumber == fleetsOfEmpires[empire].Count){
            fleetsOfEmpires[empire].Add(newFleet);
        } else {
            fleetsOfEmpires[empire][fleetNumber] = newFleet;
        }
    }

    public static bool ValidateFleetSize(List<Ship> ships, int empire){
        Fleet newFleet = new Fleet(ships);
        return newFleet.FleetSize() < GameMode.EmpireFleetSize(empire);
    }

    public static int HandleFleetIndexes(List<int> indexes, int empire){
        List<Fleet> fleets = new List<Fleet>();
        for(int i = 0; i < fleetsOfEmpires[empire].Count; i++){
            if(indexes.Contains(i)){
                fleets.Add(fleetsOfEmpires[empire][i]);
            }
        }
        for(int i = 1; i < fleets.Count; i++){
            fleetsOfEmpires[empire].Remove(fleets[i]);
        }
        return GetFleetNumber(empire, fleets[0]);
    }

    public static int GetFleetNumber(int empire, Fleet fleet){
        for(int i = 0; i < fleetsOfEmpires[empire].Count; i++){
            if(fleet == fleetsOfEmpires[empire][i]){
                return i;
            }
        }
        return -1;
    }

    public static Fleet GetShipFleet(Ship ship){
        Empire empire = GetShipEmpire(ship);
        return GetShipFleet(GetEmpireNumber(empire), ship);
    }

    public static Fleet GetShipFleet(int empire, Ship ship){
        if(fleetsOfEmpires.Count == 0){
            return null;
        }
        //foreach(List<Fleet> fleetList in fleetsOfEmpires[empire]){
        foreach(Fleet fleet in fleetsOfEmpires[empire]){
            if(fleet.shipsInFleet.Contains(ship)){
                return fleet;
            }
        }   
        //}
        return null;
    }

    /**********************************************************************************Planet Functions*******************************************************/

    //Colonizes the specific planet on the hex for the empire given; throws an error if the hex is not a system hex
    public static void ColonizePlanet(Empire empire, Hex hex, int index){
        SystemHex sysHex = (SystemHex)hex;
        if(sysHex != null){
            sysHex.AddColonizedPlanet(index);
            sysHex.planets[index%sysHex.planets.Length].Colonize(empire);
            DestroyShip(FilterColonyShips(ShipsOnHex(hex, empire))[0]);
            int i = GetEmpireNumber(empire);
            tilesOfEmpires[i].Add(sysHex);
            PlanetCanvasMain.Refresh();
        } else {
            Debug.LogError("You have asked to colonize a system that doesn't exist! Was the system tile perhaps post-generated?");
        }
        //cellGen.makeCells();
    }

    //Using the planet number and hex that it is on, return the planet referring to the index (index higher than the planet array length are accepted, but indexes lower than 0 will cause a seperate error)
    //Errors if the hex we inputted is not a system hex
    public static Planet GetPlanet(Hex hex, int index){
        if(hex is SystemHex){
            SystemHex sys = (SystemHex)hex;
            return sys.GetPlanet(index);
        } else {
            //Debug.LogError("You requested planet data from a tile that has no planet!");
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

    //Obtains the Resource outputted by all of the planets in an empire
    static Resource GetPlanetGeneration(Empire empire){
        List<Planet> planets = GetPlanets(empire);
        planets = FilterForColonizedPlanets(planets); //we only want colonized planets to generate Resource

        Resource ResourceGained = new Resource();
        for(int i = 0; i < planets.Count; i++){
            ResourceGained.Add(planets[i].GetResourceProduction());
        }

        return ResourceGained;
    }

    //Builds a building on the specific planet. Or more specifically, adds it to queue
    public static void Build(Hex hex, int index, Building building){
        Planet planet = GetPlanet(hex, index);
        building.pos = GetHexPosition(hex);
        planet.AddToQueue(building);
        PlanetCanvasMain.Refresh();
        //CanvasController.Clear();
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
        //Debug.LogError("You called for an empire that the board doesn't recognize! Was it perhaps deleted by mistake?");
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

    //Returns the Resource outputted by the empire. Currently only considers planet generation (ship upkeep is definitely going to be implemented)
    public static Resource GetEmpiresGeneration(Empire empire){
        Resource planetGen = GetPlanetGeneration(empire);
        planetGen.Add(GetUnitGeneration(empire));
        return planetGen;
    }

    //Returns the empire of the player. For multiplayer implementation this should return the empire of the client but that implementation is far out. For now, the player is always going to be the first empire
    public static Empire GetPlayerEmpire(){
        if(empires == null){
            return new Empire(new Color(1.0f, .67f, 0.0f), 0);
        }
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
        //Debug.LogError("You requested the controlling empire of a planet that isnt colonized!");
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
            RequestUpdate = false;
        }
        if(RequestShipUpdate){
            MainController.canvasController.ShowUnitButtonsOnCanvas();
            RequestShipUpdate = false;
        }
        if(GameMode.processingThread.IsAlive && !getReadyToUpdate){
            getReadyToUpdate = true;
            MainController.canvasController.UpdateButtons();
            CanvasController.Clear();
        }
        if(!GameMode.processingThread.IsAlive && getReadyToUpdate){
            MainController.canvasController.UpdateButtons();
            MainController.canvasController.RequestRedisplayOfResources();
            CanvasController.Clear();
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
