using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet
{
    public bool Colonized = false;
    public Empire EmpireThatIsColonized;

    public Planet(){

    }

    public void Colonize(Empire empire){
        Colonized = true;
        EmpireThatIsColonized = empire;
    }

    public string GetColonizedText(){
        string info = "";
        if(Colonized){
            info = "The " + EmpireThatIsColonized.Name + " colonized this planet";
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
        //Debug.Log(numPlanets);
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
        return returnPlanet;
        
    }
}

public class ArcticPlanet : Planet{

    public override string GetInfo(){
        string info = "an Artic Planet. ";
        return info + GetColonizedText();
    }

    public override Color GetColor(){
        return Color.blue;
    }

}
public class ContinetalPlanet : Planet{

    public override string GetInfo(){
        string info =  "a Continetal Planet";
        return info + GetColonizedText();
    }

    public override Color GetColor(){
        return Color.green;
    }

}
public class MoltenPlanet : Planet{

    public override string GetInfo(){
        string info =  "a Molten Planet";
        return info + GetColonizedText();
    }

    public override Color GetColor(){
        return Color.red;
    }

}
