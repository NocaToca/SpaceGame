using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexVisuals{

    static Color sysHexCol = Color.white;
    static Color spaceHexCol = Color.black;

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
        return new Color(0,0,0,0);


    }

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