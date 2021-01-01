using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The texture manager for all of our planet textures
public class TextureManager : MonoBehaviour
{
    [Header("Molten Planets")]
    public Texture2D[] moltenPlanets;
    public static Texture2D[] moltenPlanetImages;
    public Color MoltenAtmosphere;
    public static Color MoltenAtmosphereColor;

    [Header("Ocean Planets")]
    public Texture2D[] OceanPlanets;
    public static Texture2D[] OceanPlanetImages;
    public Color OceanAtmosphere;
    public static Color OceanAtmosphereColor;
    public Texture2D[] OceanClouds;
    public static Texture2D[] OceanCloudImages;

    [Header("Arctic Planets")]
    public Texture2D[] arcticPlanets;
    public static Texture2D[] arcticPlanetImages;
    public Color arcticAtmosphere;
    public static Color arcticAtmosphereColor;

    [Header("Continental Planets")]
    public Texture2D[] ContinentalPlanets;
    public static Texture2D[] ContinentalPlanetImages;
    public Color ContinentalAtmosphere;
    public static Color ContinentalAtmosphereColor;
    public Texture2D[] ContinentalClouds;
    public static Texture2D[] ContinentalCloudImages;

    void Awake(){
        moltenPlanetImages = moltenPlanets;
        
        MoltenAtmosphereColor = MoltenAtmosphere;

        OceanCloudImages = OceanClouds;
        OceanPlanetImages = OceanPlanets;
        OceanAtmosphereColor = OceanAtmosphere;

        arcticPlanetImages = arcticPlanets;
        arcticAtmosphereColor = arcticAtmosphere;

        ContinentalCloudImages = ContinentalClouds;
        ContinentalPlanetImages = ContinentalPlanets;
        ContinentalAtmosphereColor = ContinentalAtmosphere;
    }

    void Start(){
    }
}
