using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    //Our base resources in buildings, along with the position of where the building is
   public Resources cost;
   public static Resources buildingCost;
   public HexCoordinates pos;
   public Resources singleBuildingCost;

   public List<Building> buildRequirements = new List<Building>();
   public List<Tech> TechPrereq = new List<Tech>();

   public string name;

    //returns the building cost
   public Resources GetBuildingCost(){
       return singleBuildingCost;
   }

   public static List<Building> GetAllBuildings(){
       List<Building> buildings = new List<Building>();
       buildings.Add(new Starport());
       buildings.Add(new Mine());
        buildings.Add(new ShipYard());
       return buildings;
   }

   public virtual void Build(){

   }

   public virtual Building Base(){
       return null;
   }

}

public class Mine : Building{

    new public static Resources buildingCost = new Resources(10.0f, 25.0f);

    public Mine(){
        cost = new Resources();
        cost.SetGold(3.0f);
        cost.SetProduction(1.0f);
        name = "Mine";
        singleBuildingCost = buildingCost;
    }

    public override void Build(){
        Board.Build(MainController.displayingHex, CanvasController.currentPlanetDisplayed, new Mine());
   }
}

public class ShipYard: Building{
    new public static Resources buildingCost = new Resources(10.0f, 35.0f);

    public ShipYard(){
        cost = new Resources();
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
