using UnityEngine;

[System.Serializable]
public struct HexCoordinates {

	public int x { get; private set; }

	public int z { get; private set; }

    public int y { get {return -x - z;}}

	public HexCoordinates (int x, int z) {
		this.x = x;
		this.z = z;
	}

    public static HexCoordinates FromOffsetCoordinates (int x, int z) {
		return new HexCoordinates(x - (z/2), z);
	}

    public override string ToString () {
		return ("" +
			x.ToString() + ", " + y.ToString() + ", " + z.ToString() + "");
	}

    public static HexCoordinates FromPosition (Vector3 position) {
		float Xw = position.x;
        float Yw = position.z;

        Yw = Mathf.Abs(Yw);
        Xw = Mathf.Abs(Xw);

        int iX, iZ;
        float rootThree = Mathf.Sqrt(3.0f);

        float sides = 7.5f;
        float height = rootThree*(7.5f/2.0f);

        int coord1 = 0;
        int coord2 = -1;
        float h, w;
        for(h = 0; h < Yw; h += sides*2){
            coord1 = 0;
            for(w = ((h % 2) * height) + height*2; w < Xw; w += height*2){
                coord1++;
            }
            coord2++;
        }

        //So now we know we're in the range of 4 hexagons
        //Just have to find out which one is closest
        Coords hexOne = new Coords(coord1 * sides * 2, coord2 * height * 2);
        Coords hexTwo = new Coords((coord1-1) * sides * 2, coord2 * height * 2);
        Coords hexThree = new Coords(coord1 * sides * 2, (coord2-1) * height * 2);
        Coords hexFour = new Coords((coord1-1) * sides * 2, (coord2-1) * height * 2);

        
        float d1 = Mathf.Sqrt(Mathf.Pow(hexOne.x - Xw, 2.0f) + Mathf.Pow(hexOne.y - Yw, 2));
        float d2 = Mathf.Sqrt(Mathf.Pow(hexTwo.x - Xw, 2.0f) + Mathf.Pow(hexTwo.y - Yw, 2));
        float d3 = Mathf.Sqrt(Mathf.Pow(hexThree.x - Xw, 2.0f) + Mathf.Pow(hexThree.y - Yw, 2));
        float d4 = Mathf.Sqrt(Mathf.Pow(hexFour.x - Xw, 2.0f) + Mathf.Pow(hexFour.y - Yw, 2));

        float min = (d1 < d2) ? d1 : d2;
        min = (min < d3) ? min : d3;
        min = (min < d4) ? min : d4;

        int offsetX;
        int offsetY;

        if(min == d1){
            offsetX = 0;
            offsetY = 0;
        } else
        if(min == d2){
            offsetX = 1;
            offsetY = 0;
        } else
        if(min == d3){
            offsetX = 0;
            offsetY = 1;
        } else {
            offsetX = 1;
            offsetY = 1;
        }

        iX = coord1 - offsetX;
        iZ = coord2 - offsetY;
        //iZ = -iX - iZ;

        Debug.Log("World Coords: (" + Xw + ", " + Yw + ")");
        Debug.Log("Hex Coords: (" + iX + ", " + (-iX-iZ) + ", " + iZ + ")");


		return new HexCoordinates(iX, iZ);
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

