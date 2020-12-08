using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Hex
{
    public string type; 
    
    public Color color;

    public Empire ControllingEmpire;

    public List<Ship> ShipsOnHex;

    public HexUI info;

    public HexObject referenceObject;


    public Hex(){
        ShipsOnHex = new List<Ship>();
        info = new HexUI();
    }

    public void TakeControl(Empire empire){
        ControllingEmpire = empire;
        color = empire.empireColor;
        referenceObject.color = empire.empireColor;
        empire.owningHexes.Add(this); 
    }

    public virtual void Initialize(){
        return;
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

    public virtual Color GetColor(){
        return Color.black;
    }

    public virtual bool CheckForColonyShip(){
        for(int i = 0; i < ShipsOnHex.Count; i++){
            if(ShipsOnHex[i].name == "Colony Ship"){
                return true;
            }
        }
        return false;
    }

    public virtual bool DestroyColonyShip(){
        for(int i = 0; i < ShipsOnHex.Count; i++){
            if(ShipsOnHex[i].name == "Colony Ship"){
                Ship ship = ShipsOnHex[i];
                ShipsOnHex.Remove(ship);
                ship.OwningEmpire.owningUnits.Remove(ship);

                return true;
            }
        }
        return false;
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

    public string GetShipInfo(int index){
        string info = " ";
        if(ShipsOnHex.Count != 0){
            info = "There is a " + ShipsOnHex[index % ShipsOnHex.Count].name + " on this tile.";
        } else {
            info = " ";
        }
        return info;
    }

    public string GetShipLetter(int index){
        string info = " ";
        if(ShipsOnHex.Count != 0){
            info = ShipsOnHex[index % ShipsOnHex.Count].representingLetter;
        } else {
            info = " ";
        }
        return info;
    }

    public virtual Color GetPlanetColor(int index){
        return Color.black;//ColorFromPlanet(planets[index]);
    }

    public virtual string GetPlanetString(int index){
        return "";//GetInfoOnPlanet(planets[index]);
    }

    public virtual int GetPlanetsLength(){
        return 0;
    }

    public virtual int GetShipCount(){
        return ShipsOnHex.Count;
    }
}
[System.Serializable]
public class SystemHex : Hex{

    public Planet[] planets;

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
        
        string info = GetInfo();
        //Debug.Log(info);

        return true;

    }

    private string GetInfo(){
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

        return info;
    }

    public override string GetType(){
        return "System";
    }

    public override Color GetPlanetColor(int index){
        return ColorFromPlanet(planets[index%planets.Length]);
    }

    public Color ColorFromPlanet(Planet planet){
        return planet.GetColor();
    }

    public override string GetPlanetString(int index){
        return GetInfoOnPlanet(planets[index%planets.Length], index%planets.Length);
    }

    private string GetInfoOnPlanet(Planet planet,int index){
        return "Planet " + (index+1) + " is a " + planet.GetInfo();
    }

    public override int GetPlanetsLength(){
        return planets.Length;
    }
    
    public void Colonize(Empire empire, int index){
        TakeControl(empire);
        planets[index].Colonize(empire);
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
        //Debug.Log(info);

        return false;
    }

    public override string GetType(){
        return "Space";
    }

    public override Color GetPlanetColor(int index){
        return new Color(0, 0, 0, 0);
    }

    public override string GetPlanetString(int index){
        return "There is nothing of interest on this hex";
    }
}
