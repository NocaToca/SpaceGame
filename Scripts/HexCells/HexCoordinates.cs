using UnityEngine;

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

    public override string ToString () {
		return ("(" +
			x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")");
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

