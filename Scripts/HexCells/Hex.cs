using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Hex
{
    public string type; 
    
    public Color color;

    public Empire ControllingEmpire;

    public Hex(){

    }

    public void TakeControl(Empire empire){
        ControllingEmpire = empire;
        color = empire.empireColor;
    }

    public virtual void Initialize(){
        return;
    }

    public virtual Color GetColor(){
        return Color.black;
    }

    public virtual bool Interact(){
        return false;
    }

    public virtual string GetType(){
        return "";
    }

    public virtual void GiveOptions(){
        return;
    }

    public static Hex CreateNewHex(System.Random seed, HexSettings hexSettings){

        //float percent = (seed.Next(0, 100))/100.0f;
        float percent = Random.Range(0.0f, 1.0f);
        //Debug.Log(percent);

        HexValues[] chances = new HexValues[2];
        chances[1] = hexSettings.SpaceHex;
        chances[0] = hexSettings.SystemHex;


        //float prev = chances[0];

        //I'm sorting the array from least to greatest
        for(int i = 1; i < chances.Length; i++){
            if(chances[i].chance > chances[i-1].chance){
                HexValues store = chances[i-1];
                chances[i-1] = chances[i];
                chances[i] = store;
                i = 1;
            }
        }
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
            //Debug.Log("Test");

            returningHex = new SystemHex();

        }

        returningHex.type = chances[index].type;
        return returningHex;

    }
}
[System.Serializable]
public class SystemHex : Hex{

    Planet[] planets;

    public Color color = Color.white;

    public override void Initialize(){
        float numPlanets = Random.Range(0.0f, 1.0f) * 6561.0f;
        numPlanets = Mathf.Sqrt(Mathf.Sqrt(numPlanets));
        int num = 10 - ((int)(numPlanets) + 1);
        planets = Planet.GeneratePlanets(num);
        color = Color.white;
    }

    public override Color GetColor(){
        return color;
    }

    public override bool Interact(){
        
        string info = (planets.Length == 1) ? "This system has " + planets.Length + " planet." : "This system has " + planets.Length + " planets.";
        for(int i = 0; i < planets.Length; i++){
            info = info + "\nPlanet " + (i+1) + " is " + planets[i].GetInfo();
        }
        info = info + "\n";
        if(ControllingEmpire == null){
            info = info + "There is no one controlling this tile";
        } else {
            info = info + "The " + ControllingEmpire.Name + " Empire controls this tile";
        }
        Debug.Log(info);

        return true;

    }
    public override string GetType(){
        return "System";
    }
}
[System.Serializable]
public class SpaceHex : Hex{

    public override Color GetColor(){
        return Color.black;
    }

    public override bool Interact(){
        string info = "Hex has nothing on it";
        if(ControllingEmpire == null){
            info = info + ". There is no one controlling this tile";
        } else {
            info = info + ". The " + ControllingEmpire.Name + " Empire controls this tile";
        }
        Debug.Log(info);

        return false;
    }

    public override string GetType(){
        return "Space";
    }
}
