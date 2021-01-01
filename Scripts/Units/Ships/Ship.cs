using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Unit{
    //Stats of the unit
    public float health;
    public float damage;
    
    //Position of the unit in terms of our hex coordinates
    public HexCoordinates ShipPosition;

    //The letter representing our unit
    public string representingLetter;

    //How many movement points the ship has and how many are left this turn
    public int movementPoints = 2;
    public int availableMovementPoints = 2;

    // public Ship(Empire empire, Hex hex){
    //     Super(empire);
    //     this.hex = hex;
    // }

    //Every turn we would want to reset our available movement points
    public void ResetMovement(){
        availableMovementPoints = movementPoints;
    }


}

public class ColonyShip : Ship{

    static readonly Resource productionCost = new Resource(10.0f, 15.0f);
    static readonly Resource Upkeep = new Resource(0.5f);

    public ColonyShip(){
        name = "Colony Ship";
        Type = true;
        maintenance = Upkeep;
        cost = productionCost; 
        
        representingLetter = "C";
        movementPoints = 3;
        availableMovementPoints = 3;

        health = 100.0f;
        damage = 0.0f;
    }

    public ColonyShip(Empire empire, HexCoordinates pos){
        OwningEmpire = empire;
        Type = true;
        name = "Colony Ship";
        representingLetter = "C";
        movementPoints = 3;
        ShipPosition = pos;
        availableMovementPoints = 3;
        maintenance = Upkeep;
        cost = productionCost; 

        health = 100.0f;
        damage = 0.0f;
    }

    public void ColonizePlanet(Planet planet){

    }
    public override void Build(){
        Board.Build(MainController.displayingHex, CanvasController.currentPlanetDisplayed, new ColonyShip());
    }

}

public class ProtectorShip : Ship{

    static readonly Resource Upkeep = new Resource(0.75f);
    static readonly Resource productionCost = new Resource(7.5f, 10.0f);

    public ProtectorShip(){
        Type = true;
        name = "Protector Ship";
        maintenance = Upkeep;
        cost = productionCost; 
        movementPoints = 2;
        representingLetter = "P";

        health = 90.0f;
        damage = 30.0f;
    }

    public ProtectorShip(Empire empire, HexCoordinates pos){
        OwningEmpire = empire;
        Type = true;
        name = "Protector Ship";
        representingLetter = "P";
        movementPoints = 2;
        ShipPosition = pos;
        maintenance = Upkeep;
        cost = productionCost;
         
        health = 90.0f;
        damage = 30.0f;
    }
    public override void Build(){
        Board.Build(MainController.displayingHex, CanvasController.currentPlanetDisplayed, new ProtectorShip());
    }

}

public class AssualtShip : Ship{
    static readonly Resource Upkeep = new Resource(0.75f);
    static readonly Resource productionCost = new Resource(7.5f, 12.0f);

    public AssualtShip(){
        Type = true;
        name = "Assualt Ship";
        maintenance = Upkeep;
        cost = productionCost; 
        movementPoints = 2;
        representingLetter = "AS";

        health = 90.0f;
        damage = 50.0f;
    }

    public AssualtShip(Empire empire, HexCoordinates pos){
        OwningEmpire = empire;
        Type = true;
        name = "Assualt Ship";
        representingLetter = "AS";
        movementPoints = 2;
        ShipPosition = pos;
        maintenance = Upkeep;
        cost = productionCost;
         
        health = 90.0f;
        damage = 50.0f;
    }
    public override void Build(){
        Board.Build(MainController.displayingHex, CanvasController.currentPlanetDisplayed, new AssualtShip());
    }
}

public class SpaceAmoeba : Ship{
    public SpaceAmoeba(HexCoordinates pos){
        Type = true;
        name = "Space Amoeba";
        representingLetter = "A";
        movementPoints = 2;
        ShipPosition = pos;

        health = 150.0f;
        damage = 50.0f;
    }
    //No build function because only the AI can build it, so it has to be placed
}

