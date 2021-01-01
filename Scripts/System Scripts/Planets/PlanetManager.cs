using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Manages the planets but basically just getting the texture
public class PlanetManager
{
    public int index = 0;

    public HexCoordinates coords;

    CubeSphere body;
    CubeSphere clouds;
    CubeSphere atmosphere;

    public Texture2D planetTexture;
    public Color planetColor;
    public Texture2D cloudTexture;

    public PlanetManager(){

    }

    public void SetIndex(int index){
        this.index = index;
    }

    public void Generate(Planet planet){
        //body = this.transform.GetChild(0).gameObject.GetComponent<CubeSphere>();
        //atmosphere = this.transform.GetChild(1).gameObject.GetComponent<CubeSphere>();
        //clouds = this.transform.GetChild(2).gameObject.GetComponent<CubeSphere>();

        planetTexture = PlanetVisuals.GetPlanetTexture(planet);
        //body.SetTexture(planetTexture);

        planetColor = PlanetVisuals.GetPlanetAtmosphereColor(planet);
        //atmosphere.SetColor(planetColor);
        //clouds.gameObject.SetActive(false);

        cloudTexture = PlanetVisuals.GetPlanetCloudTexture(planet);
        if(cloudTexture != null){
            //clouds.SetClouds(cloudTexture);
        }
        
    }
}
