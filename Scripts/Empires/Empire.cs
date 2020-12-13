using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empire
{
    //The number represnting our empire as seen in the board class
    public int playerNumber;

    public string Name = "Noca";

    public Empire(Color color, int playerNumber){
        EmpireVisuals.empireColors.Add(color);
        this.playerNumber = playerNumber;
    }

    public string GetText(){
        return "The " + Name + " empire controls this tile";
    }

    public static string GetDefaultText(){
        return "There is no one controlling this tile";
    }


}
