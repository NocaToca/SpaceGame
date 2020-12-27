using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Hex
{
    //The visual component of the hex
    public HexObject referenceObject; 

    //While this doesn't do anything here, it's needed for certain hex types and it's much friendlier just to initialize all hexes even if it doesnt do anything
    public virtual void Initialize(){
        return;
    }

    //Using a seed and a settings struct, we reliably randomly create a random type of Hex
    public static Hex CreateNewHex(System.Random seed, HexSettings hexSettings){

        float percent = Random.Range(0.0f, 1.0f);

        //This array is left like this for easier future implemtation of more types of hexes
        HexValues[] chances = new HexValues[2];
        chances[1] = hexSettings.SpaceHex;
        chances[0] = hexSettings.SystemHex;

        //I'm sorting the array from least to greatest in terms of their chance
        for(int i = 1; i < chances.Length; i++){
            if(chances[i].chance > chances[i-1].chance){
                HexValues store = chances[i-1];
                chances[i-1] = chances[i];
                chances[i] = store;
                i = 1;
            }
        }
        //As now we can just run through an if statement for each and break once we find the first one that is true
        int index = 0;
        for(int i = 1; i < chances.Length; i++){
            if(percent > chances[i].chance){
                index = i;
                break;
            }
        }

        Hex returningHex = new Hex();

        if(chances[index].type == "Space"){
            returningHex = new SpaceHex();
        } else
        if(chances[index].type == "System"){
            returningHex = new SystemHex();
        }
        return returningHex;

    }
}
[System.Serializable]
public class SystemHex : Hex{

    //The planet array; holds all the planet information of the hex
    public Planet[] planets;

    //A bool array for whether each planet in the planet array is colonized
    bool[] planetsColonized;

    //public Color color = Color.white;

    public override void Initialize(){
        float numPlanets = Random.Range(0.0f, 1.0f) * 6561.0f;
        numPlanets = Mathf.Sqrt(Mathf.Sqrt(numPlanets));
        int num = 10 - ((int)(numPlanets) + 1);
        planets = Planet.GeneratePlanets(num);
        planetsColonized = new bool[num];
        for(int i = 0; i < num; i++){
            planetsColonized[i] = false;
        }
    }

    public string GetPlanetString(int index){
        return GetInfoOnPlanet(planets[index%planets.Length], index%planets.Length);
    }

    private string GetInfoOnPlanet(Planet planet,int index){
        return "Planet " + (index+1) + " is a " + planet.GetInfo();
    }

    public int GetPlanetsLength(){
        return planets.Length;
    }

    public void AddColonizedPlanet(int index){
        planetsColonized[index] = true;
        planets[index%planets.Length].Colonized = true;
    }

    public Planet GetPlanet(int index){
        index %= planets.Length;
        return planets[index];
    }

    public int GetNumberOfPlanets(){
        return planets.Length;
    }
    
}
[System.Serializable]
public class SpaceHex : Hex{

}
