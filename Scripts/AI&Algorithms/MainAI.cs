using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MainAI
{
    public static void MakeMove(Empire empire){
        EmpireData empData = new EmpireData(empire);
        ArmyAI.MoveUnits(empData);

    }

    
}

public class EmpireData{
        public List<Ship> ships;
        public List<Hex> hexes;
        public List<SystemHex> syshexes;

        Empire empire;
        int index;

        public EmpireData(Empire empire){
            this.empire = empire;
            index = Board.GetEmpireNumber(empire);
            hexes = Board.GetEmpireHexes(empire);
            ships = Board.GetShipsOfEmpire(empire);
            
            Hex[] h = Board.FilterHexes(1, empire);
            syshexes = new List<SystemHex>();
            foreach(Hex hex in h){
                syshexes.Add((SystemHex)hex);
            }

        }
    }