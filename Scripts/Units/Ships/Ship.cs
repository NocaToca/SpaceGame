using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Unit{

    public Hex hex;

    public float health;
    public float damage;
    public GameObject unitObject;

    public string representingLetter;

    public int movementPoints = 2;

    // public Ship(Empire empire, Hex hex){
    //     Super(empire);
    //     this.hex = hex;
    // }

    public void Move(Hex hex){
        this.hex.ShipsOnHex.Remove(this);
        this.hex = hex;
        hex.ShipsOnHex.Add(this);
    }

}

public class ColonyShip : Ship{

    public ColonyShip(Empire empire, Hex hex){
        OwningEmpire = empire;
        this.hex = hex;
        Type = true;
        name = "Colony Ship";
        representingLetter = "C";
        movementPoints = 3;
    }

    public void ColonizePlanet(Planet planet){

    }

}

public class ProtectorShip : Ship{

    public ProtectorShip(Empire empire, Hex hex){
        OwningEmpire = empire;
        this.hex = hex;
        Type = true;
        name = "Protector Ship";
        representingLetter = "P";
        movementPoints = 2;
    }

}

