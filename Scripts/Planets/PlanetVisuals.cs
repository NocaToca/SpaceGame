using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetVisuals : MonoBehaviour
{
    public static Color GetPlanetColorFromPlanet(Planet planet){
        if(planet is MoltenPlanet){
            return Color.red;
        }
        if(planet is ContinetalPlanet){
            return Color.green;
        }
        if(planet is ArcticPlanet){
            return Color.blue;
        }
        return new Color(0,0,0,0);
    }
}
