using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet
{
    public bool Colonized = false;
    public Empire EmpireThatIsColonized;

    List<Building> buildings = new List<Building>();

    public List<Building> Queue = new List<Building>();

    Resources NaturalResources;

    float productionAmount;

    public Planet(){

    }

    public List<Building> GetBuildings(){
        return buildings;
    }

    public void Colonize(Empire empire){
        Colonized = true;
        EmpireThatIsColonized = empire;
    }

    public string GetColonizedText(){
        string info = "";
        if(Colonized){
            info = "The " + Board.GetEmpireOwningPlanet(this).Name + " colonized this planet";
        } else {
            info = "No empire has conolonized this planet";
        }
        return info;
    }

    public virtual string GetInfo(){
        return "Error! A Planet type is not specified!";
    }

    public virtual Color GetColor(){
        return Color.black;
    }

    public static Planet[] GeneratePlanets(int numPlanets){
        Planet[] planets = new Planet[numPlanets];
        for (int i = 0; i < planets.Length; i++){
            planets[i] = GeneratePlanet();
        }
        return planets;
    }

    public static Planet GeneratePlanet(){
        float percent = Random.Range(0.0f, 1.0f);

        int typesOfPlanets = 3;
        float p = 1.0f;
        int index = -1;
        for(int i = 0; i < typesOfPlanets; i++){
            p -= 1.0f/(float)typesOfPlanets;
            index++;
            if(percent > p){
                break;
            }
        }
        Planet returnPlanet = new Planet();
        if(index == 0){
            //Debug.Log("1");
            returnPlanet = new ArcticPlanet();
        } else
        if(index == 1){
            //Debug.Log("2");
            returnPlanet = new ContinetalPlanet();
        } else
        if(index == 2){
            //Debug.Log("3");
            returnPlanet = new MoltenPlanet();
        }
        returnPlanet.GenerateNaturalProduction();
        return returnPlanet;
        
    }

    public Resources GetResourceProduction(){
        Resources producedResources = new Resources();
        producedResources.Add(NaturalResources);
        for(int i = 0; i < buildings.Count; i++){
            producedResources.Add(buildings[i].cost);
        }
        return producedResources;
    }

    public void AddToQueue(Building building){
        Queue.Add(building);
    }
    public void RemoveFromQueue(Building building){
        Queue.Remove(building);
    }

    public void BuildQueue(){
        float production = GetResourceProduction().Production;
        productionAmount += production;
        if(Queue.Count != 0){
            if(productionAmount == Queue[0].GetBuildingCost().Production){
                productionAmount = 0;
            } else
            if (productionAmount > Queue[0].GetBuildingCost().Production){
                productionAmount -= Queue[0].GetBuildingCost().Production;
            }
        } else {
            productionAmount = 0;
        }
    }

    public static float GetResourceFromVals(float center, float radius){
        float val = Mathf.Cos(Random.Range(0, 2 * Mathf.PI));
        return center + (radius * val);
    }

    public Resources GetNaturalResources(){
        return NaturalResources;
    }

    public virtual void GenerateNaturalProduction(){
        Debug.LogError("You have tried to initialize a planet of null type!");
    }

    public void SetNaturalResources(float gold, float prod){
        NaturalResources = new Resources(gold, prod);
        //NaturalResources.SetGold(gold);
        //NaturalResources.SetProduction(prod);
    }
}

//Low Food, High Gold, Med Prod
public class ArcticPlanet : Planet{

    public static float GoldCenter;
    public static float GoldRad;

    public static float FoodCenter;
    public static float FoodRad;

    public static float ProdCenter;
    public static float ProdRad;

    public override string GetInfo(){
        string info = "an Artic Planet. ";
        return info + GetColonizedText();
    }

    public override Color GetColor(){
        return Color.blue;
    }

    public override void GenerateNaturalProduction(){
        float gold = GetResourceFromVals(GoldCenter, GoldRad);
        float prod = GetResourceFromVals(ProdCenter, ProdRad);
        SetNaturalResources(gold, prod);
    }

}
//High Food, Low Gold, Med Prod
public class ContinetalPlanet : Planet{

    public static float GoldCenter;
    public static float GoldRad;

    public static float FoodCenter;
    public static float FoodRad;

    public static float ProdCenter;
    public static float ProdRad;

    public override string GetInfo(){
        string info =  "a Continetal Planet";
        return info + GetColonizedText();
    }

    public override Color GetColor(){
        return Color.green;
    }

    public override void GenerateNaturalProduction(){
        float gold = GetResourceFromVals(GoldCenter, GoldRad);
        float prod = GetResourceFromVals(ProdCenter, ProdRad);
        SetNaturalResources(gold, prod);
        
    }

}
//Very high Prod, Low Gold, Low Food
public class MoltenPlanet : Planet{

    public static float GoldCenter;
    public static float GoldRad;

    public static float FoodCenter;
    public static float FoodRad;

    public static float ProdCenter;
    public static float ProdRad;

    public override string GetInfo(){
        string info =  "a Molten Planet";
        return info + GetColonizedText();
    }

    public override Color GetColor(){
        return Color.red;
    }

    public override void GenerateNaturalProduction(){
        float gold = GetResourceFromVals(GoldCenter, GoldRad);
        float prod = GetResourceFromVals(ProdCenter, ProdRad);
        SetNaturalResources(gold, prod);
        
    }

}
