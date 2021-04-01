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
        planetsColonized[index%planets.Length] = true;
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
public class EmptyHex : Hex{

}
public class AsteroidField : Hex{

}
public class DeepSpace : Hex {

}
public class QuantumAsteroidField : Hex{

}
public class NeutronStar : Hex{
    
}