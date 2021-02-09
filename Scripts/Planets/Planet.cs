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

    public string name;

    //Gives a list of buildings that have been built on this tile
    List<Building> buildings = new List<Building>();

    //The building queue on this planet
    public List<Building> BuildingQueue = new List<Building>();

    public List<Building> availableBuildings;

    public List<Pops> popsOnPlanet;

    public float percentTowardsPlanet = 0.5f;

    //The natural Resource this planet starts with
    Resource NaturalResource;

    //How much production this planet has made for its current project
    float productionAmount;
    float foodAmount;

    public Planet(){
        //SetAvailableBuildings();
    }

    //Returns the buildings on our planet
    public List<Building> GetBuildings(){
        return buildings;
    }

    //Colonizes the planet for the given empire
    public void Colonize(Empire empire){
        popsOnPlanet = new List<Pops>();
        Colonized = true;
        EmpireThatIsColonized = empire;
        popsOnPlanet.Add(new Pops());
    }

    public void SetName(string name){
        this.name = name;
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

        int typesOfPlanets = 4;
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
        } else
        if(index == 3){
            returnPlanet = new OceanPlanet();
        }
        returnPlanet.GenerateNaturalProduction();
        return returnPlanet;
        
    }

    public void GeneratePop(){
        if(!Colonized){
            return;
        }
        float food = GetResourceProduction().Food;
        foodAmount += food;
        float goal = GetNextFoodAmount();
        if(food >= goal){
            List<Pops> typesOfPops = GetTypesOfPops();
            int ran = Random.Range(0, typesOfPops.Count - 1);
            popsOnPlanet.Add(typesOfPops[ran]);
        }
    }

    private float GetNextFoodAmount(){
        return Mathf.Exp(popsOnPlanet.Count) + 1.0f;
    }

    private List<Pops> GetTypesOfPops(){
        List<Pops> returnPops = new List<Pops>();
        List<string> names = new List<string>();
        foreach(Pops pop in popsOnPlanet){
            if(!names.Contains(pop.name)){
                names.Add(pop.name);
                returnPops.Add(pop);
            }
        }
        return returnPops;
    }

    //Returns how much Resource this planet generates per turn
    public Resource GetResourceProduction(){
        Resource producedResource = new Resource();
        producedResource.Add(NaturalResource);
        for(int i = 0; i < buildings.Count; i++){
            producedResource.Add(buildings[i].cost);
        }
        // foreach(Pops pop in popsOnPlanet){
        //     producedResource.Add(pop.popResource);
        //     if(pop.assignedResource != null){
        //         producedResource.Add(pop.popResource);
        //     }
        // }
        return producedResource;
    }

    //Adds a building to queue
    public void AddToQueue(Building building){
        BuildingQueue.Add(building);
        SetAvailableBuildings();
    }
    //Removes a building from queue
    public void RemoveFromQueue(Building building){
        BuildingQueue.Remove(building);
        SetAvailableBuildings();
    }

    //Refreshes the check for what buildings are available on this planet
    public void RefreshAvailableBuildings(){
        SetAvailableBuildings();
    }

    //Builds whatever is in queue. If there is nothing in queue no production is created, however left over production from previously built buildings can be used to create the next building in queue faster
    public void BuildQueue(){
        float production = GetResourceProduction().Production;
        productionAmount += production * percentTowardsPlanet;
        if(BuildingQueue.Count != 0){
            if(productionAmount == BuildingQueue[0].GetBuildingCost().Production){
                productionAmount = 0;
                buildings.Add(BuildingQueue[0]);
                BuildingQueue.Remove(BuildingQueue[0]);
            } else
            if (productionAmount > BuildingQueue[0].GetBuildingCost().Production){
                productionAmount -= BuildingQueue[0].GetBuildingCost().Production;
                buildings.Add(BuildingQueue[0]);
                BuildingQueue.Remove(BuildingQueue[0]);
            }
            SetAvailableBuildings();
        } else {
            productionAmount = 0;
        }
        if(HasStarport()){
            GetStarport().BuildQueue(production);
        }
        GeneratePop();
    }

    //Returns a float with the center and radius of the value
    public static float GetResourceFromVals(float center, float radius){
        float val = Mathf.Sin(Random.Range(0, 2 * Mathf.PI));
        return center + (radius * val);
    }

    //Returns the base Resource generated per turn by our planet 
    public Resource GetNaturalResource(){
        return NaturalResource;
    }

    //A abstract function responsible for generating the natural resource production of each planet
    public virtual void GenerateNaturalProduction(){
        Debug.LogError("You have tried to initialize a planet of null type!");
    }

    //Sets the natural Resource of the planet
    public void SetNaturalResource(float gold, float prod, float science, float food){
        NaturalResource = new Resource(gold, prod, science, food);
        //NaturalResource.SetGold(gold);
        //NaturalResource.SetProduction(prod);
    }

    //Sets what buildings are available on this planet
    public void SetAvailableBuildings(){
        //If, for some reason, the planet isnt colonized, we just abort this check
        if(!SystemStorage.workingScence && Board.GetEmpireOwningPlanet(this) == null){
            return;
        }

        //This will return the buildings based off of tech requirements, so we don't have to go through that
        availableBuildings = GameMode.GetBuildingsAvialableToEmpire(EmpireThatIsColonized);
        List<string> names = new List<string>();
        for(int i = 0; i < buildings.Count; i++){
            names.Add(buildings[i].name);
        }

        for(int i = 0; i < BuildingQueue.Count; i++){
            names.Add(BuildingQueue[i].name);
        }

        //Building Requirements
        for(int i = 0; i < availableBuildings.Count; i++){
            foreach(Building building in availableBuildings[i].buildRequirements){
                if(!names.Contains(building.name)){
                    availableBuildings.Remove(availableBuildings[i]);
                    break;
                }
            }
        }
        
        //Checking to see if we already have the buildings
        for(int i = 0; i < availableBuildings.Count; i++){
            if(names.Contains(availableBuildings[i].name)){
                availableBuildings.Remove(availableBuildings[i]);
            }
        }
    }

    //Returns whether or not the planet has a starport
    public bool HasStarport(){
        foreach(Building building in buildings){
            if(building is Starport){
                return true;
            }
        }
        return false;
    }

    //Gets the starport. HasStarport() should be called before calling this
    public Starport GetStarport(){
        foreach(Building building in buildings){
            if(building is Starport){
                return (Starport)building;
            }
        }
        Debug.LogError("The planet detected a Starport but it seemingly vanished!");
        return null;
    }

    public void AssignPopProduction(int index){
        Resource resource = new Resource();
        resource.SetProduction(1.0f);
        popsOnPlanet[index].assignedResource = resource;
    }
    public void AssignPopFood(int index){
        Resource resource = new Resource();
        resource.SetFood(1.0f);
        popsOnPlanet[index].assignedResource = resource;
    }
    public void AssignPopScience(int index){
        Resource resource = new Resource();
        resource.SetScience(1.0f);
        popsOnPlanet[index].assignedResource = resource;
    }
    public void AssignPopGold(int index){
        Resource resource = new Resource();
        resource.SetGold(1.0f);
        popsOnPlanet[index].assignedResource = resource;
    }
    public void UnassignPop(int index){
        popsOnPlanet[index].assignedResource = null;
    }

    public List<Pops> GetUnassignedPops(){
        List<Pops> unassignedPops = new List<Pops>();
        foreach(Pops pop in popsOnPlanet){
            if(pop.assignedResource == null){
                unassignedPops.Add(pop);
            }
        }
        return unassignedPops;
    }
}

/*
For each planet, we setup the natural resources in the GenerationSettings script, where it should look much cleaner.

There is nothing unique about planets besides the resource generation yet
*/

//Low Food, High Gold, Med Prod
public class ArcticPlanet : Planet{

    public static float GoldCenter;
    public static float GoldRad;

    public static float FoodCenter;
    public static float FoodRad;

    public static float ProdCenter;
    public static float ProdRad;
    
    public static float ScienceCenter;
    public static float ScienceRad;

    public override string GetInfo(){
        string info = "an Artic Planet. ";
        return info + GetColonizedText();
    }

    public override Color GetColor(){
        return Color.cyan;
    }

    public override void GenerateNaturalProduction(){
        float gold = GetResourceFromVals(GoldCenter, GoldRad);
        float prod = GetResourceFromVals(ProdCenter, ProdRad);
        float science = GetResourceFromVals(ScienceCenter, ScienceRad);
        float food = GetResourceFromVals(FoodCenter, FoodRad);
        SetNaturalResource(gold, prod, science, food);
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
    
    public static float ScienceCenter;
    public static float ScienceRad;

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
        float science = GetResourceFromVals(ScienceCenter, ScienceRad);
        float food = GetResourceFromVals(FoodCenter, FoodRad);
        SetNaturalResource(gold, prod, science, food);
        
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

    public static float ScienceCenter;
    public static float ScienceRad;

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
        float science = GetResourceFromVals(ScienceCenter, ScienceRad);
        float food = GetResourceFromVals(FoodCenter, FoodRad);
        SetNaturalResource(gold, prod, science, food);
        
    }

}

public class OceanPlanet : Planet{
    
    public static float GoldCenter;
    public static float GoldRad;

    public static float FoodCenter;
    public static float FoodRad;

    public static float ProdCenter;
    public static float ProdRad;
    
    public static float ScienceCenter;
    public static float ScienceRad;

    public override string GetInfo(){
        string info =  "an Ocean Planet";
        return info + GetColonizedText();
    }

    public override Color GetColor(){
        return Color.blue;
    }

    public override void GenerateNaturalProduction(){
        float gold = GetResourceFromVals(GoldCenter, GoldRad);
        float prod = GetResourceFromVals(ProdCenter, ProdRad);
        float science = GetResourceFromVals(ScienceCenter, ScienceRad);
        float food = GetResourceFromVals(FoodCenter, FoodRad);
        SetNaturalResource(gold, prod, science, food);
        
    }
}
