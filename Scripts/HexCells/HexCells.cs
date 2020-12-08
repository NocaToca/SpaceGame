using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexCells : MonoBehaviour
{
    public int width = 6;
	public int height = 6;



	public HexObject cellPrefab;

    public Text cellLabelPrefab;

    HexObject[] cells;

    public Canvas gridCanvas;

	[HideInInspector]
    public HexSettings hexSettings;

	[HideInInspector]
	public HexObject[,] hexes;

	[HideInInspector]
	public Color spaceColor = new Color(1,1,1);
	[HideInInspector]
	public Color systemColor;

	public int seed;
	System.Random rand;

	Color[] cellColors;
	bool[] debugTouch;

	void Awake () {
        //gridCanvas = GetComponent<Canvas>();
        hexSettings = new HexSettings(.90f, .10f);
		

		cells = new HexObject[height * width];
		cellColors = new Color[height * width];
		debugTouch = new bool[height * width];

		hexes = new HexObject[height, width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				debugTouch[i] = false;
				//cellColors[i] = new Color(rand.Next(0, 1000)/1000.0f, rand.Next(0, 1000)/1000.0f, rand.Next(0, 1000)/1000.0f);
				CreateCell(x, z, i++);
			}
		}
	}

    void Start () {
		
		makeCells();
	}

	public void Interact(int index){
		//Debug.Log("index");
		//int x = Mathf.Abs(coordinates.x);
		//int z = Mathf.Abs(coordinates.z);
		//int index = x + z * width + z/ 2;
		//if(index >= cells.Length){
		//Debug.Log(index);
		//Debug.Log(cells.Length);
		//	return;
		//}
		
		HexObject cell = cells[index];
		//Debug.Log(cell.hasBeenTouched);
		//cellColors[index] = touchedColor;
		//debugTouch[index] = true;
		//cells.Clear();
		for(int i = 0; i < cells.Length; i++){
			Destroy(cells[i].gameObject, 0.0f);
		}
		for (int k = 0, i = 0; k < height; k++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, k, i++);
			}
		}
		makeCells();
	}

	public void makeCells(){
	
		for(int i = 0; i < height*width; i++){
			if(cells[i] == null){
				Debug.Log("Error 001 \n HexCells.cs 86");

			}
		}
		for(int i = 0; i < cells.Length; i++){
			cells[i].hexMesh.Triangulate(cells[i]);
		}
		
		// for(int i = 1; i < height*width; i++){
		// 	HexMesh hexMesh = cells[i].GetComponentInChildren<HexMesh>();
		// 	if(hexMesh != null){
		// 		GameObject byebye = hexMesh.gameObject;
		// 		if(byebye != null){
		// 			Object.Destroy(byebye, 0.0f);
		// 		}	
		// 	}
			
					

		// }
	}
	
	void CreateCell (int x, int z, int i) {

		//hexes[z, x] = new Hex();

		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (Utilities.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (Utilities.outerRadius * 1.5f);

		System.Random rand = new System.Random(seed);
        
		HexObject cell = hexes[z,x] = Instantiate<HexObject>(cellPrefab);
		hexes[z,x].hex = Hex.CreateNewHex(rand, hexSettings);
		cell.hex = hexes[z,x].hex;
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;

		hexes[z,x].hex.referenceObject = hexes[z,x];
	
		//cell.color = defaultColor;

		//hexes[z, x].displayHex = cell;

		cellColors[i] = cell.hex.GetColor();

		hexes[z, x].coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		
		cell.color = cellColors[i];
		cell.hasBeenTouched = debugTouch[i];

		cells[i] = cell;

        // Text label = Instantiate<Text>(cellLabelPrefab);
		// label.rectTransform.SetParent(gridCanvas.transform, false);
		// label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
		// label.text = x.ToString() + "\n" + z.ToString();
	}

	public HexObject[,] GetHexes(){
		return hexes;
	}
}

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

