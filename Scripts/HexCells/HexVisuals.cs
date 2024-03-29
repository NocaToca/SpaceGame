﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The static class containing all the information we need to return the display of a hex
public static class HexVisuals{

    //Initial colors of certain tiles
    static Color sysHexCol = Color.white;
    static Color spaceHexCol = Color.grey;
    static Color emptyHexCol = new Color(0,0,0,0);
    static Color asteroidFieldCol = new Color(0.423f, 0.164f, 0.054f);
    static Color deepSpaceCol = Color.black;
    static Color quantumAsteroidFieldCol = Color.blue;
    static Color neutronStarCol = Color.cyan;

    //Gets the color of the hex
    public static Color GetColor(Hex hex){
        if(Board.IsHexControlled(hex)){
            return EmpireVisuals.GetEmpireColor(Board.GetEmpireThatControlsHex(hex));
        }
        if(hex is SystemHex){
            return sysHexCol;
        }
        if(hex is SpaceHex){
            return spaceHexCol;
        }
        if(hex is EmptyHex){
            return emptyHexCol;
        }
        if(hex is AsteroidField){
            return asteroidFieldCol;
        }
        if(hex is DeepSpace){
            return deepSpaceCol;
        }
        if(hex is QuantumAsteroidField){
            return quantumAsteroidFieldCol;
        }
        if(hex is NeutronStar){
            return neutronStarCol;
        }
        return new Color(0,0,0,0);


    }

    //Gets the display text of the ship on the hex
    public static string GetShipText(Hex hex, int index){
        Ship[] ships = Board.ShipsOnHex(hex);
        string info = " ";
        if(ships.Length != 0){
            info = "There is a " + ships[index % ships.Length].name + " on this tile.";
        } else {
            info = " ";
        }
        return info;
    }

    //Gets the display letter of the ship on the hex
    public static string GetShipLetter(Hex hex, int index){
        Ship[] ships = Board.ShipsOnHex(hex);
        string info = " ";
        if(ships.Length != 0){
            info = ships[index % ships.Length].representingLetter;
        } else {
            info = " ";
        }
        return info;
    }
}