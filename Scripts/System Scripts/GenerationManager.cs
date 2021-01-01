using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager
{
    //The class responsible for generating our planets
    static int planetsGened = 0;

    public static Material Planet;
    public static Material Atmosphere;
    public static Material Clouds;

    public CubeSphere cubeSpherePrefab;

    List<GameObject> planets = new List<GameObject>();
    List<Texture2D> planetTextures = new List<Texture2D>();
    List<Color> atmosphereColors = new List<Color>();
    //List<bool> cloudsEnabled = new List<bool>();
    List<Texture2D> cloudTextures = new List<Texture2D>();

    List<GameObject> gameObjectsLoaded = new List<GameObject>();

    CubeSphere body;
    CubeSphere clouds;
    CubeSphere atmosphere;

    //This is just a base function class to generate a molten planet based on nothing
    public void MakeOceanPlanet(GameObject system){
        GameObject planet = new GameObject("Planet");
            
        planet.transform.parent = system.transform;
        cubeSpherePrefab = system.GetComponentInChildren<CubeSphere>();
        CubeSphere sphere = Object.Instantiate(cubeSpherePrefab);
        sphere.transform.parent = planet.transform;
        sphere.Generate();

        body = planet.transform.GetChild(0).gameObject.GetComponent<CubeSphere>();
        atmosphere = planet.transform.GetChild(1).gameObject.GetComponent<CubeSphere>();
        clouds = planet.transform.GetChild(2).gameObject.GetComponent<CubeSphere>();

        Texture2D planetTexture = PlanetVisuals.GetOceanPlanetTexture();
        Color atmosphereColor = PlanetVisuals.GetOceanPlanetAtmosphereColor();
        Texture2D cloudTexture = PlanetVisuals.GetOceanPlanetCloudTexture();

        body.SetTexture(planetTexture);
        atmosphere.SetColor(atmosphereColor);
        if(cloudTexture != null){
            clouds.SetClouds(cloudTexture);
        } else {
            clouds.gameObject.SetActive(false);
        }
        gameObjectsLoaded.Add(body.gameObject);
        gameObjectsLoaded.Add(atmosphere.gameObject);
        gameObjectsLoaded.Add(clouds.gameObject);
    }

    //Loads the information stored in the generation script
    public void Load(GameObject system){
        //GameObject system = GameObject.FindGameObjectsWithTag("SystemManager")[0]
        //We basically just create each planet
        for(int i = 0; i < 1; i++){
            GameObject planet = new GameObject("Planet");
            
            planet.transform.parent = system.transform;
            cubeSpherePrefab = system.GetComponentInChildren<CubeSphere>();
            CubeSphere sphere = Object.Instantiate(cubeSpherePrefab);
            sphere.transform.parent = planet.transform;
            sphere.Generate();

            body = planet.transform.GetChild(0).gameObject.GetComponent<CubeSphere>();
            atmosphere = planet.transform.GetChild(1).gameObject.GetComponent<CubeSphere>();
            clouds = planet.transform.GetChild(2).gameObject.GetComponent<CubeSphere>();

            body.SetTexture(planetTextures[i]);
            atmosphere.SetColor(atmosphereColors[i]);
            if(cloudTextures[i] != null){
                clouds.SetClouds(cloudTextures[i]);
            } else {
                clouds.gameObject.SetActive(false);
            }
            gameObjectsLoaded.Add(body.gameObject);
            gameObjectsLoaded.Add(atmosphere.gameObject);
            gameObjectsLoaded.Add(clouds.gameObject);
        }
        
    }

    //Destroys all of the gameobjects loaded
    public void Exit(){
        foreach(GameObject gameObject in gameObjectsLoaded){
            Object.Destroy(gameObject, 0.0f);
        }
        gameObjectsLoaded.Clear();
    }

    //Generates a system based off of the hex
    public static GenerationManager GenerateSystem(Hex hex){
        GenerationManager gm = new GenerationManager();
        if(hex is SystemHex){
            SystemHex sys = (SystemHex)hex;
            int numPlanets = sys.planets.Length;
            
            for(int i = 0; i < numPlanets; i++){
                GeneratePlanet(sys.planets[i], gm);
            }
        }
        return gm;
    }

    //Generates a planet using a planet manager
    public static GenerationManager GeneratePlanet(Planet planet, GenerationManager gm){
        //GameObject planet = new GameObject("Planet");
        //planet.transform.parent = this.transform;
        //CubeSphere sphere = Instantiate(cubeSpherePrefab);
        //sphere.transform.parent = planet.transform;
        //sphere.Generate();

        PlanetManager pm = new PlanetManager();
        //planetsGened++;
        pm.SetIndex(planetsGened);

        pm.Generate(planet);

        //planets.Add(planet);
        gm.planetTextures.Add(pm.planetTexture);
        gm.atmosphereColors.Add(pm.planetColor);
        gm.cloudTextures.Add(pm.cloudTexture);
        return gm;
    }

    public static void ResetGeneration(){
        planetsGened = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
