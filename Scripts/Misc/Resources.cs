using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources
{
    
    public float Gold;
    public float Production;

    public Resources(){

        Gold = 0;
        Production = 0;

    }
    public Resources(float Gold, float Production){
        this.Gold = Gold;
        this.Production = Production;
    
    }

    public void SetGold(float Gold){
        this.Gold = Gold;
    }
    public void SetProduction(float prod){
        Production = prod;
    }

    public Resources Add(Resources resource){
        this.Gold += resource.Gold;
        this.Production += resource.Production;
        return this;
    }

    public string GoldToString(){
        int gold = (int)Gold;
        return "" + gold;
    }
    public string ProdToString(){
        int prod = (int)Production;
        return "" + prod;
    }

}
