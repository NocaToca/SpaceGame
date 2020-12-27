using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    //The gamemode is responsible for handling gameflow and game variables

    //The resources each empire has
    static List<Resources> empireResources = new List<Resources>();
    static List<List<Tech>> empireTechs = new List<List<Tech>>();
    static List<List<Tech>> empireTechQueues = new List<List<Tech>>();

    //Adds the resoucres to the empire's stock
    public static void AddEmpireResource(Resources resource){
        empireResources.Add(resource);
    }

    public static void InitializeTechs(int numberOfEmpires){
        for(int i = 0; i < numberOfEmpires; i++){
            empireTechs.Add(new List<Tech>());
            empireTechQueues.Add(new List<Tech>());
        }
    }

    public static void AddTech(Tech tech, Empire empire){
        int index = Board.GetEmpireNumber(empire);
        empireTechs[index].Add(tech);
    }

    public static List<Tech> GetEmpiresTechs(Empire empire){
        int index = Board.GetEmpireNumber(empire);
        return empireTechs[index];
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
        HandleTechQueue(empire);
        CanvasController.Clear();
        MoveNomadShips();
        AI.DoTurn();
    }

    //returns the resources of the player
    public static Resources GetPlayerResources(){
        return empireResources[0];
    }

    public static List<Building> GetBuildingsAvialableToEmpire(Empire empire){
        List<Building> allBuildings = Building.GetAllBuildings();
        //Tech requirements
        List<Tech> techs = GetEmpiresTechs(empire);
        List<string> names = new List<string>();
        //names.Clear();
        foreach(Tech tech in techs){
            names.Add(tech.name);
        }
        for(int i = 0; i < allBuildings.Count; i++){
            foreach(Tech tech in allBuildings[i].TechPrereq){
                if(!names.Contains(tech.name)){
                    allBuildings.Remove(allBuildings[i]);
                    //Debug.Log(tech.name);
                    //names.RemoveAt(i);
                    //i--;
                    break;
                }
            }
        }
        return allBuildings;
    }

    public static List<Unit> GetUnitsAvailableToEmpire(Empire empire){
        List<Unit> allUnits = Unit.GetAllUnits(); 
        return allUnits;
    }

    public static void HandleTechQueue(Empire empire){
        int index = Board.GetEmpireNumber(empire);
        if(empireTechQueues[index].Count == 0){
            return;
        }
        Resources resource = empireResources[index];
        float science = empireTechQueues[index][0].science;
        if(resource.Science >= science){
            resource.Science = resource.Science - science;
            empireTechs[index].Add(empireTechQueues[index][0]);
            empireTechQueues[index].RemoveAt(0);
        }
    }

    public static List<Tech> GetTechsAvailableToEmpire(Empire empire){
        int index = Board.GetEmpireNumber(empire);
        List<Tech> techsFromJustObtained = Tech.FilterTechs(empireTechs[index]);
        List<string> names = new List<string>();
        for(int i = 0; i < techsFromJustObtained.Count; i++){
            names.Add(techsFromJustObtained[i].name);
        }
        int numberRemoved = 0;
        foreach(Tech tech in empireTechQueues[index]){
            if(names.Contains(tech.name)){
                techsFromJustObtained.RemoveAt(names.IndexOf(tech.name));
                names.Remove(tech.name);
            }
        }
        return techsFromJustObtained;
    }

    public static void MoveNomadShips(){
        Board.MoveNomadShips();
        Board.ResetShipMovementPoints(null);
    }

    public static void AddToTechQueue(Tech tech, Empire empire){
        int index = Board.GetEmpireNumber(empire);
        empireTechQueues[index].Add(tech);
    }
}
