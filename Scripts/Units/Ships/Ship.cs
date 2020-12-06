using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Unit{

    public Hex hex;

    public float health;
    public float damage;
    public GameObject unitObject;

    // public Ship(Empire empire, Hex hex){
    //     Super(empire);
    //     this.hex = hex;
    // }

    public void Move(Hex hex){
        this.hex = hex;
    }

}

public class ColonyShip : Ship{

    public ColonyShip(Empire empire, Hex hex){
        OwningEmpire = empire;
        this.hex = hex;
    }

    public void ColonizePlanet(Planet planet){

    }

}

public class ProtectorShip : Ship{

    public ProtectorShip(Empire empire, Hex hex){
        OwningEmpire = empire;
        this.hex = hex;
    }

}

