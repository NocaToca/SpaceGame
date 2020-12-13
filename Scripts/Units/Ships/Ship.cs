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

    public ColonyShip(Empire empire, HexCoordinates pos){
        OwningEmpire = empire;
        Type = true;
        name = "Colony Ship";
        representingLetter = "C";
        movementPoints = 3;
        ShipPosition = pos;
        availableMovementPoints = 3;

    }

    public void ColonizePlanet(Planet planet){

    }

}

public class ProtectorShip : Ship{

    public ProtectorShip(Empire empire, HexCoordinates pos){
        OwningEmpire = empire;
        Type = true;
        name = "Protector Ship";
        representingLetter = "P";
        movementPoints = 2;
        ShipPosition = pos;
    }

}

