using UnityEngine;
using System;

//The hex coordinate system is so we can store our x,y,z hex coordinates in an array
[System.Serializable]
public struct HexCoordinates {

	public int x { get; private set; }

	public int z { get; private set; }

    public int y { get; private set;}

	public HexCoordinates (int x, int y, int z) {
		this.x = x;
        this.y = y;
		this.z = z;
	}

    //Our function to determine if a struct equals a struct
    public override bool Equals(object obj){
        if(!(obj is HexCoordinates)){
            return false;
        }
        HexCoordinates pos = (HexCoordinates)obj;
        if(pos.x == this.x && pos.y == this.y && pos.z == this.z){
            return true;
        }
        return false;
    }

    //A simple to string function to be able to print our hex coordinates
    public override string ToString () {
		return ("(" +
			x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")");
	}

    public override int GetHashCode(){
        return Tuple.Create(x,z,y).GetHashCode();
    }
}

public struct Coords{
    public float x { get; private set; }
    public float y { get; private set; }

    public Coords(float x, float y){

        this.x = x;
        this.y = y; 

    }
}

