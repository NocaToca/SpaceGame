using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Our planet class here handles all of the logic revolving around planets!
*/
public class Planet
{
    //A boolean representing whether or not the planet is colonized. We don't have to store which empire it is since we can assume by getting the hex its on from the board class, but I feel that'll eat up too much
    public bool Colonized = false;
    public Empire EmpireThatIsColonized;

    //Gives a list of buildings that have been built on this tile
    List<Building> buildings = new List<Building>();

    //The building queue on this planet
    public List<Building> Queue = new List<Building>();

    //The natural resources this planet starts with
    Resources NaturalResources;

    //How much production this planet has made for its current project
    float productionAmount;

    public Planet(){

    }

    //Returns the buildings on our planet
    public List<Building> GetBuildings(){
        return buildings;
    }

    //Colonizes the planet for the given empire
    public void Colonize(Empire empire){
        Colonized = true;
        EmpireThatIsColonized = empire;
    }

    //Gets the information on who colonized this planet, if it is colonized
    public string GetColonizedText(){
        string info = "";
        if(Colonized){
            info = "The " + Board.GetEmpireOwningPlanet(this).Name + " colonized this planet";
        } else {
            info = "No empire has conolonized this planet";
        }
        return info;
    }

    //Returns the informaiton on the planet
    public virtual string GetInfo(){
        return "Error! A Planet type is not specified!";
    }

    //Returns the color of the planet
    public virtual Color GetColor(){
        return Color.black;
    }

    //Generates planets how many times is inputted, adding them to an array
    public static Planet[] GeneratePlanets(int numPlanets){
        Planet[] planets = new Planet[numPlanets];
        for (int i = 0; i < planets.Length; i++){
            planets[i] = GeneratePlanet();
        }
        return planets;
    }

    //Generates a random planet
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

    //Returns how much resources this planet generates per turn
    public Resources GetResourceProduction(){
        Resources producedResources = new Resources();
        producedResources.Add(NaturalResources);
        for(int i = 0; i < buildings.Count; i++){
            producedResources.Add(buildings[i].cost);
        }
        return producedResources;
    }

    //Adds a building to queue
    public void AddToQueue(Building building){
        Queue.Add(building);
    }
    //Removes a building from queue
    public void RemoveFromQueue(Building building){
        Queue.Remove(building);
    }

    //Builds whatever is in queue. If there is nothing in queue no production is created, however left over production from previously built buildings can be used to create the next building in queue faster
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

    //Returns a float with the center and radius of the value
    public static float GetResourceFromVals(float center, float radius){
        float val = Mathf.Sin(Random.Range(0, 2 * Mathf.PI));
        return center + (radius * val);
    }

    //Returns the base resources generated per turn by our planet 
    public Resources GetNaturalResources(){
        return NaturalResources;
    }

    //A abstract function responsible for generating the natural resource production of each planet
    public virtual void GenerateNaturalProduction(){
        Debug.LogError("You have tried to initialize a planet of null type!");
    }

    //Sets the natural resources of the planet
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
