using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit{

    public Empire OwningEmpire;

    public GameObject unitObject;

    public bool Type; //False = Land Unit | True = Ship Unit (I just made it a bool because theres two values)

    public string name;

    public Resource cost;
    public Resource maintenance;

    // public Unit(Empire empire){
    //     OwningEmpire = empire;
    // }

    //Gets all available units
    public static List<Unit> GetAllUnits(){
        List<Unit> units = new List<Unit>();
        units.Add(new ColonyShip());
        units.Add(new ProtectorShip());
        units.Add(new AssualtShip());
        return units;
    }

    public virtual void Build(){

    }

    //Clears ships from the list of units
    public static List<Unit> ClearShipsFromList(List<Unit> units){
        List<Unit> filteredUnits = new List<Unit>();
        foreach(Unit unit in units){
            if(!(unit is Ship)){
                filteredUnits.Add(unit);
            }
        }
        return filteredUnits;
    }

    public Resource GetCost(){
        return cost;
    }


}

