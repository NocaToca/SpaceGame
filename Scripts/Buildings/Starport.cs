using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starport : Building
{
    //We want to override but use the same name as our building cost
    new public static Resources buildingCost = new Resources(50.0f, 5.0f);

    public Starport(HexCoordinates pos){
        cost = new Resources();
        cost.SetGold(-1.0f);
        cost.SetProduction(2.0f);
        this.pos = pos;
    }
}
