using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource
{
    //The resource class is responsible for all functionality regarding our resources
    
    //The resources we have
    public float Gold;
    public float Production;
    public float Science;

    //Various constructors; if a float isn't inputted it is assumed it's zero
    public Resource(){
        Gold = 0;
        Production = 0;
    }
    public Resource(float Gold){
        this.Gold = Gold;
        Production = 0.0f;
        Science = 0.0f;
    }
    public Resource(float Gold, float Production){
        this.Gold = Gold;
        this.Production = Production;
        Science = 0.0f;
    }
    public Resource(float Gold, float Production, float Science){
        this.Gold = Gold;
        this.Production = Production;
        this.Science = Science;
    }

    //Setting the resource values
    public void SetGold(float Gold){
        this.Gold = Gold;
    }
    public void SetProduction(float prod){
        Production = prod;
    }
    public void SetScience(float Science){
        this.Science = Science;
    }

    //Adds two resources together
    public Resource Add(Resource resource){
        this.Gold += resource.Gold;
        this.Production += resource.Production;
        this.Science += resource.Science;
        return this;
    }

    //Returns the string value of cooresponding resource (casted to int)
    public string GoldToString(){
        int gold = (int)Gold;
        return "" + gold;
    }
    public string ProdToString(){
        int prod = (int)Production;
        return "" + prod;
    }
    public string ScienceToString(){
        int science = (int)Science;
        return "" + science;
    }

}
