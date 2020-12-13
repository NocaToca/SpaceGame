using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    //The gamemode is responsible for handling gameflow and game variables

    //The resources each empire has
    static List<Resources> empireResources = new List<Resources>();

    //Adds the resoucres to the empire's stock
    public static void AddEmpireResource(Resources resource){
        empireResources.Add(resource);
    }

    //Ends the turn of the empire, adding all of the resources it generated up and moving to the next
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

    //returns the resources of the player
    public static Resources GetPlayerResources(){
        return empireResources[0];
    }


}
