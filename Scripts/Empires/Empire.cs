using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empire
{
    
    public Color empireColor;

    public List<Unit> owningUnits;

    public string Name = "Noca";

    public List<Hex> owningHexes;

    public Empire(Color color){
        empireColor = color;
        owningUnits = new List<Unit>();
        owningHexes = new List<Hex>();
    }

    public string GetText(){
        return "The " + Name + " empire controls this tile";
    }

    public static string GetDefaultText(){
        return "There is no one controlling this tile";
    }


}
