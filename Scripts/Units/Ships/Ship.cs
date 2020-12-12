using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Unit{
    public float health;
    public float damage;
    
    public HexCoordinates ShipPosition;

    public string representingLetter;

    public int movementPoints = 2;
    public int availableMovementPoints = 2;

    // public Ship(Empire empire, Hex hex){
    //     Super(empire);
    //     this.hex = hex;
    // }
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

