using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemStorage : MonoBehaviour
{
    //Responsible for storing all of our info on our system generation
    public GameObject generationObject;

    //The list of generation managers we have initiated
    static GenerationManager[,] systemManagers;

    public GenerationManager gm;

    public static bool LoadScene = false;

    //Whether or not I'm currently working on things in the planet generation scene
    public bool WorkingWithScene = false;

    public Material Atmosphere;
	public Material Planet;
	public Material Clouds;
    
    // Start is called before the first frame update
    void Start()
    {
        //If we're loading the scene, load it
        if(LoadScene){
            LoadSystem(MainController.displayingHex);
            LoadScene = false;
        }
        if(WorkingWithScene){
            
            CubeSphere.Atmosphere = Atmosphere;
            CubeSphere.Planet = Planet;
            CubeSphere.Clouds = Clouds;
            gm = new GenerationManager();
            gm.MakeOceanPlanet(GameObject.FindGameObjectsWithTag("SystemManager")[0]);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Loading the system based off of the coordinates of the hex we want to look at
    public void LoadSystem(Hex hex){
        GameObject system = GameObject.FindGameObjectsWithTag("SystemManager")[0];
        Vector2 coords = Board.FindHexCoordsInBoard(hex);
        //Debug.Log(coords);
        int height = (int)coords.y;
        int width = (int)coords.x;
        systemManagers[height,width].Load(system);
    }

    //Exits the system
    public static void ExitSystem(Hex hex){
        Vector2 coords = Board.FindHexCoordsInBoard(hex);
        int height = (int)coords.x;
        int width = (int)coords.y;
        systemManagers[height,width].Exit();
    }

    //Initializes our system storage
    public static void InitializeSystemStorage(int width, int height, HexObject[,] hexes){
        systemManagers = new GenerationManager[height,width];
        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
                systemManagers[y,x] = GenerationManager.GenerateSystem(hexes[y,x].hex);
            }
        }
    }
}
