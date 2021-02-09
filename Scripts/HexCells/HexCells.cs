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

	public HexObject cellPrefab; //The prefab we base our cells on

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

	public AnimationCurve aniCurve;
	public int octaves;
    public float persistance;
    public float lacunarity;
    public float scale;

	Color[] cellColors;
	bool[] debugTouch;

	//Initializing everything on awake
	void Awake () {
        
	}

	//The initialize function to, well, initialize the cell generator
	public void Initialize(){
		hexSettings = new HexSettings(/*Space*/.89f, /*System*/.10f, /*Empty*/.10f,/*Asteroid*/.1f,/*Deep Space*/.90f,/*QAF*/.50f, /*Neutron Star*/.50f);
		hexSettings.SetLowerTilesHeight(.3f);
		hexSettings.SetMiddleTilesHeight(.95f);
		hexSettings.SetUpperTilesHeight(1.0f);

		NoiseSettings settings = new NoiseSettings(octaves, persistance, lacunarity, scale);
		HexGenerator.GenerateNoiseBoard(width, height, 0, settings, aniCurve);

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
		position.x = (x + z * 0.5f - z / 2) * (HexUtilities.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexUtilities.outerRadius * 1.5f);

		System.Random rand = new System.Random(seed);

		//Initializing each cell
		HexObject cell = hexes[z,x] = Instantiate<HexObject>(cellPrefab);
		hexes[z,x].hex = HexGenerator.CreateNewHex(rand, hexSettings, x, z);
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

	//If the board was already initialized, we dont want to make the cellgen make a new board. Instead, we give the cellgen the old hex array and tell it thats the new board.
	public void SetHexes(HexObject[,] hexes){
		//this.hexes = hexes;
		cellPrefab = GetComponentInChildren<HexObject>();
		int height = hexes.GetLength(0);
		int width = hexes.GetLength(1);
		this.hexes = new HexObject[height,width];
		cells = new HexObject[height * width];
		for(int y = 0, i = 0; y < height; y++){
			for(int x = 0; x < width; x++, i++){
				Vector3 position;
				position.x = (x + y * 0.5f - y / 2) * (HexUtilities.innerRadius * 2f);
				position.y = 0f;
				position.z = y * (HexUtilities.outerRadius * 1.5f);

				HexObject cell = this.hexes[y,x] = GameObject.Instantiate<HexObject>(cellPrefab);
				this.hexes[y,x].hex = hexes[y,x].hex;
				cell.hex = hexes[y,x].hex;
				cell.transform.SetParent(transform, false);
				cell.transform.localPosition = position;

				hexes[y,x].hex.referenceObject = hexes[y,x];

				cells[i] = cell;
			}
		}
	}
}

//Setting structs
public struct HexSettings{
	public float lowerTilesHeight;
	public float middleTilesHeight;
	public float upperTilesHeight;

	public HexValues SpaceHex;
	public HexValues SystemHex;
	public HexValues EmptyHex;
	public HexValues AsteroidField;
	public HexValues DeepSpace;
	public HexValues QuantumAsteroidField;
	public HexValues NeutronStar;

	public HexSettings(float spaceChance, float systemChance, float emptyChance, float asteroidChance, float deepChance, float quantumChance, float neutronChance){
		SpaceHex = new HexValues("Space", 1.0f-spaceChance, "Middle");
		SystemHex = new HexValues("System", 1.0f-systemChance, "Middle");
		EmptyHex = new HexValues("Empty", 1.0f-emptyChance, "Lower");
		AsteroidField = new HexValues("Asteroid Field", 1.0f-asteroidChance, "Middle");
		DeepSpace = new HexValues("Deep Space", 1.0f-deepChance, "Lower");
		QuantumAsteroidField = new HexValues("Quantum Asteroid Field", 1.0f-quantumChance, "Upper");
		NeutronStar = new HexValues("Neutron Star", 1.0f - neutronChance, "Upper");
		lowerTilesHeight = .1f;
		middleTilesHeight = .9f;
		upperTilesHeight = 1.0f;
	}

	public void SetLowerTilesHeight(float val){
		lowerTilesHeight = val;
	}
	public void SetMiddleTilesHeight(float val){
		middleTilesHeight = val;
	}
	public void SetUpperTilesHeight(float val){
		upperTilesHeight = val;
	}
}

public struct HexValues{
	public float chance;
	public string type;
	public string height;

	public HexValues(string type, float chance, string height){
		this.type = type;
		this.chance = chance;
		this.height = height;
	}

}

