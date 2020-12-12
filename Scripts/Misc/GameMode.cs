using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{

    static List<Resources> empireResources = new List<Resources>();

    public static void AddEmpireResource(Resources resource){
        empireResources.Add(resource);
    }
    public static void EndTurn(Empire empire){
        Resources GeneratedRevenure = Board.GetEmpiresGeneration(empire);

        int index = Board.GetEmpireNumber(empire);
        empireResources[index] = GeneratedRevenure.Add(empireResources[index]);

        List<Planet> planets = Board.GetPlanets(empire);
        for(int i = 0; i < planets.Count; i++){
            planets[i].BuildQueue();
        }

        Board.ResetShipMovementPoints(empire);
    }
    public static Resources GetPlayerResources(){
        return empireResources[0];
    }


}
