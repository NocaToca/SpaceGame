using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemStorage : MonoBehaviour
{
    //Responsible for storing all of our info on our system generation
    public GameObject generationObject;

    public static int index;

    //The list of generation managers we have initiated
    static GenerationManager[,] systemManagers;

    public static GenerationManager gm;

    public static bool LoadScene = false;

    //Whether or not I'm currently working on things in the planet generation scene
    public bool WorkingWithScene = false;
    public static bool workingScence = false;

    public bool inHexes;

    public static PlanetController pc;

    public float radius;

    public static Planet basePlanet1;
    public static Planet basePlanet2;

    public Material Atmosphere;
	public Material Planet;
	public Material Clouds;
	public Material Star;
    
    // Start is called before the first frame update
    void Start()
    {
        if(inHexes){
            return;
        }
        workingScence = WorkingWithScene;
        index = 0;
        Planet showingPlanet = null;
        pc = GetComponentInChildren<PlanetController>();
        //If we're loading the scene, load it
        if(LoadScene){
            CubeSphere.Star = Star;
            radius = 10.0f;
            //gm.radius = radius;
            //gm.PlaceStar(GameObject.FindGameObjectsWithTag("SystemManager")[0]);
            LoadSystem(MainController.displayingHex);
            LoadScene = false;
            showingPlanet = Board.GetPlanet(MainController.displayingHex, 0);
        } else
        if(WorkingWithScene){
            
            basePlanet1 = new OceanPlanet();
            basePlanet1.SetNaturalResource(5.0f, 3.0f, 8.0f, 5.0f);
            basePlanet2 = new ArcticPlanet();
            basePlanet2.SetNaturalResource(2.0f, 3.0f, 10.0f, 4.0f);
            basePlanet2.Colonize(Board.GetPlayerEmpire());
            CubeSphere.Atmosphere = Atmosphere;
            CubeSphere.Planet = Planet;
            CubeSphere.Clouds = Clouds;
            CubeSphere.Star = Star;
            gm = new GenerationManager();
            gm.MakeOceanPlanet(GameObject.FindGameObjectsWithTag("SystemManager")[0]);
            gm.MakeArcticPlanet(GameObject.FindGameObjectsWithTag("SystemManager")[0]);
            gm.radius = radius;
            gm.PlacePlanets();
            gm.PlaceStar(GameObject.FindGameObjectsWithTag("SystemManager")[0]);

            showingPlanet = basePlanet1;

        }
        PlanetCanvasMain pcm = GameObject.FindGameObjectsWithTag("Canvas")[0].GetComponent<PlanetCanvasMain>();
        pcm.SetRelativeStorageUnit(this);
        pcm.ShowPlanetInfo(showingPlanet);
    }

    public static void ShowPlanetInfo(){
        PlanetCanvasMain pcm = GameObject.FindGameObjectsWithTag("Canvas")[0].GetComponent<PlanetCanvasMain>();
        index++;
        if(workingScence){
            if(index % 2 == 0){
                pcm.ShowPlanetInfo(basePlanet1);
            } else {
                pcm.ShowPlanetInfo(basePlanet2);
            }
        } else {
            pcm.ShowPlanetInfo(Board.GetPlanet(MainController.displayingHex, index));
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
        systemManagers[height,width].radius = radius;
        systemManagers[height,width].PlaceStar(GameObject.FindGameObjectsWithTag("SystemManager")[0]);
        systemManagers[height,width].Load(system);
        gm = systemManagers[height,width];
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
