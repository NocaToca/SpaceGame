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

    public static int timesSwaped = 0;
    public float angleFromCenter;

    public int displayingPlanet = 0;

    public float radius = 7.5f;
    List<Vector3> planetPos = new List<Vector3>();

    public CubeSphere cubeSpherePrefab;

    List<GameObject> planets = new List<GameObject>();
    List<Texture2D> planetTextures = new List<Texture2D>();
    List<Color> atmosphereColors = new List<Color>();
    //List<bool> cloudsEnabled = new List<bool>();
    List<Texture2D> cloudTextures = new List<Texture2D>();

    List<float> planetRotations = new List<float>();

    List<GameObject> gameObjectsLoaded = new List<GameObject>();

    //bool waiting = false;

    CubeSphere body;
    CubeSphere clouds;
    CubeSphere atmosphere;

    Vector3 movingPos;
    public float movingAngle;
    public float lerp = 0.0f;

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
        planetRotations.Add(Random.Range(10.0f, 24.0f));

        planets.Add(planet);
        gameObjectsLoaded.Add(body.gameObject);
        gameObjectsLoaded.Add(atmosphere.gameObject);
        gameObjectsLoaded.Add(clouds.gameObject);
    }

    public void MakeArcticPlanet(GameObject system){
        GameObject planet = new GameObject("Planet");
            
        planet.transform.parent = system.transform;
        cubeSpherePrefab = system.GetComponentInChildren<CubeSphere>();
        CubeSphere sphere = Object.Instantiate(cubeSpherePrefab);
        sphere.transform.parent = planet.transform;
        sphere.Generate();

        body = planet.transform.GetChild(0).gameObject.GetComponent<CubeSphere>();
        atmosphere = planet.transform.GetChild(1).gameObject.GetComponent<CubeSphere>();
        clouds = planet.transform.GetChild(2).gameObject.GetComponent<CubeSphere>();

        Texture2D planetTexture = PlanetVisuals.GetArcticPlanetTexture();
        Color atmosphereColor = PlanetVisuals.GetArcticPlanetAtmosphereColor();
        Texture2D cloudTexture = null;

        body.SetTexture(planetTexture);
        atmosphere.SetColor(atmosphereColor);
        if(cloudTexture != null){
            clouds.SetClouds(cloudTexture);
        } else {
            clouds.gameObject.SetActive(false);
        }
        planetRotations.Add(Random.Range(10.0f, 24.0f));

        planets.Add(planet);
        gameObjectsLoaded.Add(body.gameObject);
        gameObjectsLoaded.Add(atmosphere.gameObject);
        gameObjectsLoaded.Add(clouds.gameObject);
    }

    public void PlacePlanets(){
        timesSwaped = 0;
        if(planets.Count == 0){
            return;
        }
        //float radius = this.radius * 2.0f;
        if(planets.Count == 1){
            Vector3 pos = new Vector3(-radius,0,0);
            planets[0].transform.position = pos;
            planets[0].transform.Rotate(0, 0, planetRotations[0]);
            return;
        }
        int val = planets.Count;
        float increment = 360.0f / val;
        for(float angle = angleFromCenter, i = 0; angle < 360.0f + angleFromCenter; angle += increment, i += 1.0f){
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            Vector3 pos = new Vector3(x, 0, y);

            planets[(int)i].transform.position = pos;
            planets[(int)i].transform.Rotate(0, 0, planetRotations[(int)i]);
        }
    }

    public void SetPlanetPos(){
        Vector3 primePlanetPos = planets[0].transform.position;
        angleFromCenter = Mathf.Atan(primePlanetPos.z/primePlanetPos.x) * Mathf.Rad2Deg;
    }

    public bool MovePlanets(){
        lerp += .01f;
        int val = planets.Count;
        float angleOffset = Mathf.Lerp(0, 360/val, lerp);
        angleOffset *= Mathf.Deg2Rad;
        float increment = 360.0f / val;
        for(float angle = 180.0f, i = 0; angle < 540.0f; angle += increment, i += 1.0f){
            float x = Mathf.Cos((angle * Mathf.Deg2Rad) + angleOffset) * radius;
            float y = Mathf.Sin((angle * Mathf.Deg2Rad) + angleOffset) * radius;
            Vector3 pos = new Vector3(x, 0, y);

            planets[(int)i].transform.position = pos;
            //planets[(int)i].transform.Rotate(0, 0, planetRotations[(int)i]);
        }
        //Debug.Log(lerp);
        return lerp > .99f && lerp < 1.01f;
    }
    
    public void SwapOrder(){
        List<GameObject> pla = new List<GameObject>();
        for(int i = 1; i < planets.Count; i++){
            pla.Add(planets[i]);
        }
        pla.Add(planets[0]);
        planets = pla;
        timesSwaped++;
    }

    public void PlaceStar(GameObject system){
        GameObject planet = new GameObject("Star");
            
        planet.transform.parent = system.transform;
        cubeSpherePrefab = system.GetComponentInChildren<CubeSphere>();
        CubeSphere sphere = Object.Instantiate(cubeSpherePrefab);
        sphere.transform.parent = planet.transform;
        sphere.IsStar = true;
        //sphere.radius *= 5.0f;
        //sphere.gridSize *= 5;
        sphere.Generate();
        planet.transform.localScale = new Vector3(25, 25, 25);
        planet.transform.position = new Vector3(28, 0, 0);


    }

    //Loads the information stored in the generation script
    public void Load(GameObject system){
        //GameObject system = GameObject.FindGameObjectsWithTag("SystemManager")[0]
        gameObjectsLoaded.Clear();
        planets.Clear();
        //We basically just create each planet
        for(int i = 0; i < planetTextures.Count; i++){
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

            planets.Add(planet);
            gameObjectsLoaded.Add(body.gameObject);
            gameObjectsLoaded.Add(atmosphere.gameObject);
            gameObjectsLoaded.Add(clouds.gameObject);
        }

        PlacePlanets();
        
    }

    //Destroys all of the gameobjects loaded
    public void Exit(){
        foreach(GameObject gameObject in gameObjectsLoaded){
            Object.Destroy(gameObject, 0.0f);
        }
        foreach(GameObject planet in planets){
            Object.Destroy(planet, 0.0f);
        }
        gameObjectsLoaded.Clear();
        planets.Clear();
    }

    //Generates a system based off of the hex
    public static GenerationManager GenerateSystem(Hex hex){
        GenerationManager gm = new GenerationManager();
        if(hex is SystemHex){
            SystemHex sys = (SystemHex)hex;
            int numPlanets = sys.planets.Length;
            
            for(int i = 0; i < numPlanets; i++){
                gm = GeneratePlanet(sys.planets[i], gm);
            }
        }
        return gm;
    }

    //Generates a planet using a planet manager
    public static GenerationManager GeneratePlanet(Planet planet, GenerationManager gm){

        PlanetManager pm = new PlanetManager();
        //planetsGened++;
        pm.SetIndex(planetsGened);

        pm.Generate(planet);

        //planets.Add(planet);
        gm.planetRotations.Add(Random.Range(10.0f, 24.0f));
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
