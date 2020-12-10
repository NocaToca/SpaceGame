using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpireVisuals : MonoBehaviour
{
    public static List<Color> empireColors = new List<Color>();

    public static Color GetEmpireColor(Empire empire){
        return empireColors[empire.playerNumber];
    }

    public static string GetEmpireText(Empire empire){
        return "The " + empire.Name + " empire controls this tile";
    }
}
