using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PCL{
    
    public static bool DeterminePlanetColonizable(Planet planet){
        
        bool con1 = !planet.Colonized;
        bool con2 = MainController.displayingHex != null && Board.CheckForColonyShip(MainController.displayingHex);
        return con1 && con2;

    }

}
