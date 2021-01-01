using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    //Our base Resource in buildings, along with the position of where the building is
    public Resource cost; //The cost of the building to upkeep
    public static Resource buildingCost; //The building cost of the building
    public HexCoordinates pos; //The position the building is on
    public Resource singleBuildingCost; //Used to set our static variable

    //The list of requirements to build the building. Either buildings themselves or technologies
    public List<Building> buildRequirements = new List<Building>();
    public List<Tech> TechPrereq = new List<Tech>();

    //The name of the building
    public string name;

    //returns the building cost
    public Resource GetBuildingCost(){
        return singleBuildingCost;
    }

    //Returns all of the buildings in the game
    public static List<Building> GetAllBuildings(){
        List<Building> buildings = new List<Building>();
        buildings.Add(new Starport());
        buildings.Add(new Mine());
        buildings.Add(new ShipYard());
        return buildings;
    }

    //An abstract function required for other buildings to be buildable
    public virtual void Build(){

    }

    //Returns the base building type of the building
    public virtual Building Base(){
        return null;
    }

}

//A simple mining building
public class Mine : Building{

    new public static Resource buildingCost = new Resource(10.0f, 25.0f);

    public Mine(){
        cost = new Resource();
        cost.SetGold(3.0f);
        cost.SetProduction(1.0f);
        name = "Mine";
        singleBuildingCost = buildingCost;
    }

    public override void Build(){
        Board.Build(MainController.displayingHex, CanvasController.currentPlanetDisplayed, new Mine());
    }
}

//A shipyard to improve ship construction
public class ShipYard: Building{
    new public static Resource buildingCost = new Resource(10.0f, 35.0f);

    public ShipYard(){
        cost = new Resource();
        cost.SetGold(-1.0f);
        cost.SetProduction(5.0f);
        name = "Shipyard";
        singleBuildingCost = buildingCost;

        buildRequirements.Add(new Starport());
        TechPrereq.Add(Tech.GetShipyards());
    }
    
    public override void Build(){
        Board.Build(MainController.displayingHex, CanvasController.currentPlanetDisplayed, new ShipYard());
    }

}
