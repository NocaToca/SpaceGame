using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public static void MoveShip(Ship ship){
        Hex hex = Board.GetHexShipOn(ship);

        Vector2 coords = Board.FindHexCoordsInBoard(hex);

        List<Hex> positions = new List<Hex>();
        positions.Clear();

        //So we want to move the ship to a smart location. For now, this is just going to be to move the ship as far as we can move.
        //We want to get all of the hexes that are within the ships movement radius
        for(int i = -ship.availableMovementPoints; i < ship.availableMovementPoints; i++){
            int x = i;
            int y = ship.availableMovementPoints - Mathf.Abs(x);
            if(coords.x - x > 0 && coords.y - y > 0 && coords.x - x < Board.width-1 && coords.y - y < Board.height - 1){
                Hex hexToAdd = Board.GetHex(new Vector2(coords.x - x, coords.y - y)).hex;
                if(!positions.Contains(hexToAdd)){
                    positions.Add(hexToAdd);
                }
            }
        }
        for(int i = -ship.availableMovementPoints; i < ship.availableMovementPoints; i++){
            int y = i;
            int x = ship.availableMovementPoints - Mathf.Abs(y);
            if(coords.x - x > 0 && coords.y - y > 0 && coords.x - x < Board.width-1 && coords.y - y < Board.height - 1){
                Hex hexToAdd = Board.GetHex(new Vector2(coords.x - x, coords.y - y)).hex;
                if(!positions.Contains(hexToAdd)){
                    positions.Add(hexToAdd);
                }
            }
        }

        int rand = Random.Range(0, positions.Count-1);
        Board.RequestMovement(positions[rand], ship);
    }

    public static void DoTurn(){
        List<Ship> ships = Board.GetNomadShips();
        foreach(Ship ship in ships){
            if(Board.DoesHexHaveOpposingFleets(Board.GetHexShipOn(ship))){
                Board.Fight(Board.GetHexShipOn(ship));
            }
        }
    }
}
