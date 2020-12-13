using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The cell gen class. Needs massive improvement but a bit too integral to change right now, but it isn't going to get any harder later
public class HexCells : MonoBehaviour
{
	//Dimensions inherted from the Board class
	[HideInInspector]
    public int width = 6;
	public int height = 6;

	public HexObject cellPrefab; //The prefab we vase our cells on

    HexObject[] cells; //A 1d array containing all of our hexes

	//The generation settings. Made to be editable but currently dont have an reason to do so
	[HideInInspector]
    public HexSettings hexSettings;

	//The 2d array containing all of our objects
	[HideInInspector]
	public HexObject[,] hexes;

	[HideInInspector]
	public Color spaceColor = new Color(1,1,1);
	[HideInInspector]
	public Color systemColor;

	//The seed of our generation
	public int seed;
	System.Random rand;

	Color[] cellColors;
	bool[] debugTouch;

	//Initializing everything on awake
	void Awake () {
        hexSettings = new HexSettings(.90f, .10f);
		cells = new HexObject[height * width];
		cellColors = new Color[height * width];
		debugTouch = new bool[height * width];

		hexes = new HexObject[height, width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				debugTouch[i] = false;
				CreateCell(x, z, i++);
			}
		}
	}

	//On start we create all of our cells
    void Start () {
		makeCells();
	}

	//We want to update the shader parameters which can be done by re-triangulating the cells
	public void makeCells(){
	
		for(int i = 0; i < height*width; i++){
			if(cells[i] == null){
				Debug.LogError("Error 001 \n HexCells.cs");

			}
		}
		for(int i = 0; i < cells.Length; i++){
			cells[i].hexMesh.Triangulate(cells[i]);
		}
	}
	
	//Creates cell at position x,z and index i
	void CreateCell (int x, int z, int i) {
		//Getting the center position of the hex
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (Utilities.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (Utilities.outerRadius * 1.5f);

		System.Random rand = new System.Random(seed);

		//Initializing each cell
		HexObject cell = hexes[z,x] = Instantiate<HexObject>(cellPrefab);
		hexes[z,x].hex = Hex.CreateNewHex(rand, hexSettings);
		cell.hex = hexes[z,x].hex;
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;

		hexes[z,x].hex.referenceObject = hexes[z,x];
		cell.hasBeenTouched = debugTouch[i];

		cells[i] = cell;
	}

	//Returns our 2d hex array
	public HexObject[,] GetHexes(){
		return hexes;
	}
}

//Setting structs
public struct HexSettings{
	public HexValues SpaceHex;
	public HexValues SystemHex;

	public HexSettings(float spaceChance, float systemChance){
		SpaceHex = new HexValues("Space", 1.0f-spaceChance);
		SystemHex = new HexValues("System", 1.0f-systemChance);
	}
}

public struct HexValues{
	public float chance;
	public string type;

	public HexValues(string type, float chance){
		this.type = type;
		this.chance = chance;

	}

}

