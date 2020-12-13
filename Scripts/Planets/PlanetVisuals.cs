﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetVisuals : MonoBehaviour
{
    //The planet visual class handles all of our visual components of our planets

    //Gets a color depending on the planet inputted
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
