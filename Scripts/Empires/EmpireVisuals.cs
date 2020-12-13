using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpireVisuals : MonoBehaviour
{
    //The list of our empire colors
    public static List<Color> empireColors = new List<Color>();

    //Returns the color of the empire inputted
    public static Color GetEmpireColor(Empire empire){
        return empireColors[empire.playerNumber];
    }

    //Returns the text description of the empire inputted
    public static string GetEmpireText(Empire empire){
        return "The " + empire.Name + " empire controls this tile";
    }
}
