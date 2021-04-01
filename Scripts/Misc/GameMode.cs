using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class GameMode : MonoBehaviour
{
    //The gamemode is responsible for handling gameflow and game variables

    //The Resource each empire has
    static List<Resource> empireResource = new List<Resource>();
    static List<List<Tech>> empireTechs = new List<List<Tech>>();
    static List<List<Tech>> empireTechQueues = new List<List<Tech>>();

    static List<Player> players = new List<Player>();

    static Player currentPlayer;
    static int turnNumber;

    public static Thread processingThread = new Thread(ProcessTurn);

    static Empire empire;

    public static void StartGame(){
        if(players == null){
            Debug.LogError("You haven't initialized the players before starting the game!");
        }
        currentPlayer = players[0];
    }

    public static void NextPlayer(){
        int index = turnNumber%players.Count;
        bool ai = !players[index].human;
        if(ai){
            MainAI.MakeMove(players[index].empire);
        }
    }

    public static void ProcessTurn(){
        Resource GeneratedRevenure = Board.GetEmpiresGeneration(empire);

        int index = Board.GetEmpireNumber(empire);
        empireResource[index] = GeneratedRevenure.Add(empireResource[index]);

        List<Planet> planets = Board.GetPlanets(empire);
        for(int i = 0; i < planets.Count; i++){
            planets[i].BuildQueue();
        }

        players[turnNumber%players.Count].turn = false;
        turnNumber++;
        players[turnNumber%players.Count].turn = true;

        Board.ResetShipMovementPoints(empire);
        HandleTechQueue(empire);
        
        MoveNomadShips();
        AI.DoTurn();
        //NextPlayer();

        //processingThread.Abort();
    }

    public static bool isPlayerTurn(int empire){
        return players[empire].turn;
    }

    public static bool isPlayerTurn(Empire empire){
        return players[Board.GetEmpireNumber(empire)].turn;
    }

    public static void AddPlayer(Player player){
        players.Add(player);
    }

    //Adds the resoucres to the empire's stock
    public static void AddEmpireResource(Resource resource){
        empireResource.Add(resource);
    }

    //Initializing all of the empires techs
    public static void InitializeTechs(int numberOfEmpires){
        for(int i = 0; i < numberOfEmpires; i++){
            empireTechs.Add(new List<Tech>());
            empireTechQueues.Add(new List<Tech>());
        }
    }

    //Adds a tech to the list of an empires completed techs
    public static void AddTech(Tech tech, Empire empire){
        int index = Board.GetEmpireNumber(empire);
        empireTechs[index].Add(tech);
    }

    //Gets an empires techs
    public static List<Tech> GetEmpiresTechs(Empire empire){
        if(empireTechs.Count == 0){
            return Tech.GetAllTechs();
        }
        int index = Board.GetEmpireNumber(empire);
        return empireTechs[index];
    }

    //Ends the turn of the empire, adding all of the Resource it generated up and moving to the next
    public static void EndTurn(Empire empire){
        GameMode.empire = empire;
        NextPlayer();
        processingThread.Start();
        MainController.canvasController.UpdateButtons();
        CanvasController.Clear();
        
    }

    //returns the Resource of the player
    public static Resource GetPlayerResource(){
        return empireResource[0];
    }

    //Out of what techs an empire controls, gets the buildings avaliable to it
    public static List<Building> GetBuildingsAvialableToEmpire(Empire empire){
        if(empireTechs == null){
            return Building.GetAllBuildings();
        }
        List<Building> allBuildings = Building.GetAllBuildings();
        //Tech requirements
        List<Tech> techs = GetEmpiresTechs(empire);
        List<string> names = new List<string>();
        //names.Clear();
        foreach(Tech tech in techs){
            names.Add(tech.name);
        }
        //For everybuilding, we check the tech requirements for it. If we dont have the tech, remove the requirements (We have to do it based off of the name since otherwise it'd do it off of memory address, which would obviously be wrong)
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

    //Out of the technologies we have, gets the units avaliable to our empire
    public static List<Unit> GetUnitsAvailableToEmpire(Empire empire){
        List<Unit> allUnits = Unit.GetAllUnits(); 
        return allUnits;
    }

    //Handles the tech queue of the empire, if there is even anything inside the tech queue
    public static void HandleTechQueue(Empire empire){
        int index = Board.GetEmpireNumber(empire);
        if(empireTechQueues[index].Count == 0){
            return;
        }
        Resource resource = empireResource[index];
        float science = empireTechQueues[index][0].science;
        if(resource.Science >= science){
            resource.Science = resource.Science - science;
            empireTechs[index].Add(empireTechQueues[index][0]);
            empireTechQueues[index].RemoveAt(0);
        }
    }

    //Out of the techs we have, we get the techs available to the specified empire
    public static List<Tech> GetTechsAvailableToEmpire(Empire empire){
        int index = Board.GetEmpireNumber(empire);
        List<Tech> techsFromJustObtained = Tech.FilterTechs(empireTechs[index]);
        List<string> names = new List<string>();
        for(int i = 0; i < techsFromJustObtained.Count; i++){
            names.Add(techsFromJustObtained[i].name);
        }
        //int numberRemoved = 0;
        foreach(Tech tech in empireTechQueues[index]){
            if(names.Contains(tech.name)){
                techsFromJustObtained.RemoveAt(names.IndexOf(tech.name));
                names.Remove(tech.name);
            }
        }
        return techsFromJustObtained;
    }

    //Moves all of the nomad ships
    public static void MoveNomadShips(){
        Board.MoveNomadShips();
        //The null parameter is replacing the empire paramenter
        Board.ResetShipMovementPoints(null);
    }

    //Adds a tech to the specified empire's queue
    public static void AddToTechQueue(Tech tech, Empire empire){
        int index = Board.GetEmpireNumber(empire);
        empireTechQueues[index].Add(tech);
    }

    public static int SetEmpireFleetLimit(Empire empire){
        empire.fleetSize = 4;
        return empire.fleetSize;
    }

    public static int EmpireFleetSize(int empire){
        return SetEmpireFleetLimit(Board.empires[empire]);
    }
}

public class Player{
    public readonly Empire empire;
    public readonly bool human;
    public bool turn;

    public Player(Empire empire, bool human, bool turn){
        this.empire = empire;
        this.human = human;
        this.turn = turn;
    }
}
