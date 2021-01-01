using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The main AI class. It's going to be responsible for units and planets with no empire; most things will inherent from this too
public class AI : MonoBehaviour
{
    //Moves the selected ship based off of where it can move. Right now it's just completely random
    public static void MoveShip(Ship ship){

        //We need the position coordinates of the ship currently to go through each available position
        Hex hex = Board.GetHexShipOn(ship);
        Vector2 coords = Board.FindHexCoordsInBoard(hex);
        List<Hex> positions = new List<Hex>();
        positions.Clear();

        //So we want to move the ship to a smart location. For now, this is just going to be to move the ship as far as we can move.
        //We want to get all of the hexes that are within the ships movement radius
        //We could just choose a random hex in the board, but we want the ship to use all of its movement points
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

        //Simply just generating a random number in the list
        int rand = Random.Range(0, positions.Count-1);
        Board.RequestMovement(positions[rand], ship);
    } 

    //Asking the AI to politely do it's turn
    public static void DoTurn(){
        //We want to move each ship the AI owns
        List<Ship> ships = Board.GetNomadShips();
        foreach(Ship ship in ships){
            if(Board.DoesHexHaveOpposingFleets(Board.GetHexShipOn(ship))){
                Board.Fight(Board.GetHexShipOn(ship));
            }
        }
    }
}
