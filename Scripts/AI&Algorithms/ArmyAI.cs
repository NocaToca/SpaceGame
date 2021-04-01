using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArmyAI
{
    public static void MoveUnits(EmpireData data){
        foreach(Ship ship in data.ships){
            AI.MoveShip(ship);
        }
    }
}
