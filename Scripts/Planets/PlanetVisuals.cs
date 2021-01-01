using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetVisuals : MonoBehaviour
{
    //The planet visual class handles all of our visual components of our planets

    //Gets a color depending on the planet inputted
    public static Color GetPlanetColorFromPlanet(Planet planet){
        if(planet is MoltenPlanet){
            return Color.red;
        }
        if(planet is ContinetalPlanet){
            return Color.green;
        }
        if(planet is ArcticPlanet){
            return Color.cyan;
        }
        if(planet is OceanPlanet){
            return Color.blue;
        }
        if(planet != null){
            Debug.LogError("You have not set up this planet's visuals yet!");
        }
        return new Color(0,0,0,0);
    }

    //Returns the texture of the planet based off it's type
    public static Texture2D GetPlanetTexture(Planet planet){
        if(planet is MoltenPlanet){
            return GetMoltenPlanetTexture();
        }
        if(planet is ContinetalPlanet){
            return GetContinentalPlanetTexture();
        }
        if(planet is ArcticPlanet){
            return GetArcticPlanetTexture();
        }
        if(planet is OceanPlanet){
            return GetOceanPlanetTexture();
        }
        return null;
    }

    //Returns the atmosphere color of the planet based off of its type
    public static Color GetPlanetAtmosphereColor(Planet planet){
        if(planet is MoltenPlanet){
            return GetMoltenPlanetAtmosphereColor();
        }
        if(planet is ContinetalPlanet){
            return GetContinentalPlanetAtmosphereColor();
        }
        if(planet is ArcticPlanet){
            return GetArcticPlanetAtmosphereColor();
        }
        if(planet is OceanPlanet){
            return GetOceanPlanetAtmosphereColor();
        }
        return Color.white;

    }

    //If the planet has clouds, returns the clouds based off of its type
    public static Texture2D GetPlanetCloudTexture(Planet planet){
        if(planet is MoltenPlanet){
            return null;
        }
        if(planet is ContinetalPlanet){
            return GetContinentalPlanetCloudTexture();
        }
        if(planet is ArcticPlanet){
            return null;
        }
        if(planet is OceanPlanet){
            return GetOceanPlanetCloudTexture();
        }
        return null;

    }

    //All of these functions just fetch thing from our TextureManager script, which should be set up
    public static Texture2D GetMoltenPlanetTexture(){
        int ran = Random.Range(1,TextureManager.moltenPlanetImages.Length) - 1;
        return TextureManager.moltenPlanetImages[ran];
    }

    public static Color GetMoltenPlanetAtmosphereColor(){
        return TextureManager.MoltenAtmosphereColor;
    }

    public static Texture2D GetOceanPlanetTexture(){
        int ran = Random.Range(1,TextureManager.OceanPlanetImages.Length) - 1;
        return TextureManager.OceanPlanetImages[ran];
    }

    public static Color GetOceanPlanetAtmosphereColor(){
        return TextureManager.OceanAtmosphereColor;
    }

    public static Texture2D GetOceanPlanetCloudTexture(){
        int ran = Random.Range(1,TextureManager.OceanCloudImages.Length) - 1;
        return TextureManager.OceanCloudImages[ran];
    }

    public static Texture2D GetArcticPlanetTexture(){
        int ran = Random.Range(1,TextureManager.arcticPlanetImages.Length) - 1;
        return TextureManager.arcticPlanetImages[ran];
    }

    public static Color GetArcticPlanetAtmosphereColor(){
        return TextureManager.arcticAtmosphereColor;
    }

    public static Texture2D GetContinentalPlanetTexture(){
        int ran = Random.Range(1,TextureManager.ContinentalPlanetImages.Length) - 1;
        return TextureManager.ContinentalPlanetImages[ran];
    }

    public static Color GetContinentalPlanetAtmosphereColor(){
        return TextureManager.ContinentalAtmosphereColor;
    }

    public static Texture2D GetContinentalPlanetCloudTexture(){
        int ran = Random.Range(1,TextureManager.ContinentalCloudImages.Length) - 1;
        return TextureManager.ContinentalCloudImages[ran];
    }
}
