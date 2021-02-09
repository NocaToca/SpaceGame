using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexGenerator{
    public static float[,] NoiseBoard;

    public static float[,] GenerateNoiseBoard(int width, int height, int seed, NoiseSettings settings, AnimationCurve heightCurve){
        float[,] noise = new float[height,width];

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
                noise[y,x] = 1.0f;
            }
        }

        NoiseBoard = ApplyParaboloid(width, height, noise);

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
                NoiseBoard[y,x] += Random.Range(-0.15f,0.15f);
            }
        }
        
        return NoiseBoard;
    }

    public static float[,] ApplyParaboloid(int width, int height, float[,] noise){
        float x = width;
        float z = height;
        float y = -width - height;
    
        float a = (x * x)/4;
        float b = (z * z)/4;
        float c = (y * y);

        float centerX = width/2.0f;
        float centerY = height/2.0f;
        float centerZ = y/2.0f;

        for(int k = 0; k < height; k++){
            for(int i = 0; i < width; i++){
                float val = Mathf.Pow(i - centerX,2)/a + Mathf.Pow(k - centerY,2)/b;//+ Mathf.Pow((-i-k) - centerZ,2)/c;
                //val -= .5f;
                val = -val;
                val += 1f;
                noise[k,i] = val;
            }
        }
        return noise;
    }

    //Using a seed and a settings struct, we reliably randomly create a random type of Hex
    public static Hex CreateNewHex(System.Random seed, HexSettings hexSettings, int width, int height){

        if(NoiseBoard == null){
            Debug.LogError("Please Generate the noise board before generating hexes!");
        }

        float noiseVal = NoiseBoard[height, width];

        noiseVal += .1f * Mathf.Sin(Random.Range(0, Mathf.PI * 2));
        if(noiseVal <= 0){
            return new EmptyHex();
        }

        if(noiseVal < hexSettings.lowerTilesHeight){
            return GenerateLowerTile(hexSettings);
        }
        if(noiseVal < hexSettings.middleTilesHeight){
            return GenerateMiddleTile(hexSettings);
        }
        return GenerateUpperTile(hexSettings);
        

    }

    private static Hex GenerateLowerTile(HexSettings hexSettings){
        float percent = Random.Range(0.0f, 1.0f);

        //This array is left like this for easier future implemtation of more types of hexes
        HexValues[] chances = new HexValues[2];
        chances[1] = hexSettings.EmptyHex;
        chances[0] = hexSettings.DeepSpace;

        //I'm sorting the array from least to greatest in terms of their chance
        for(int i = 1; i < chances.Length; i++){
            if(chances[i].chance > chances[i-1].chance){
                HexValues store = chances[i-1];
                chances[i-1] = chances[i];
                chances[i] = store;
                i = 1;
            }
        }
        //As now we can just run through an if statement for each and break once we find the first one that is true
        int index = 0;
        for(int i = 1; i < chances.Length; i++){
            if(percent > chances[i].chance){
                index = i;
                break;
            }
        }

        Hex returningHex = new Hex();

        if(chances[index].type == "Empty"){
            returningHex = new EmptyHex();
        } else
        if(chances[index].type == "Deep Space"){
            returningHex = new DeepSpace();
        }
        return returningHex;
    }

    private static Hex GenerateMiddleTile(HexSettings hexSettings){
        float percent = Random.Range(0.0f, 1.0f);

        //This array is left like this for easier future implemtation of more types of hexes
        HexValues[] chances = new HexValues[3];
        chances[1] = hexSettings.SpaceHex;
        chances[0] = hexSettings.SystemHex;
        chances[2] = hexSettings.AsteroidField;

        //I'm sorting the array from least to greatest in terms of their chance
        for(int i = 1; i < chances.Length; i++){
            if(chances[i].chance > chances[i-1].chance){
                HexValues store = chances[i-1];
                chances[i-1] = chances[i];
                chances[i] = store;
                i = 1;
            }
        }

        //As now we can just run through an if statement for each and break once we find the first one that is true
        int index = 0;
        for(int i = 1; i < chances.Length; i++){
            if(percent > chances[i].chance){
                index = i;
                break;
            }
        }

        Hex returningHex = new Hex();

        if(chances[index].type == "Space"){
            returningHex = new SpaceHex();
        } else
        if(chances[index].type == "System"){
            returningHex = new SystemHex();
        } else
        if(chances[index].type == "Asteroid Field"){
            returningHex = new AsteroidField();
        }
        return returningHex;
    }

    private static Hex GenerateUpperTile(HexSettings hexSettings){
        float percent = Random.Range(0.0f, 1.0f);

        //This array is left like this for easier future implemtation of more types of hexes
        HexValues[] chances = new HexValues[2];
        chances[1] = hexSettings.NeutronStar;
        chances[0] = hexSettings.QuantumAsteroidField;

        //I'm sorting the array from least to greatest in terms of their chance
        for(int i = 1; i < chances.Length; i++){
            if(chances[i].chance > chances[i-1].chance){
                HexValues store = chances[i-1];
                chances[i-1] = chances[i];
                chances[i] = store;
                i = 1;
            }
        }
        //As now we can just run through an if statement for each and break once we find the first one that is true
        int index = 0;
        for(int i = 1; i < chances.Length; i++){
            if(percent > chances[i].chance){
                index = i;
                break;
            }
        }

        Hex returningHex = new Hex();

        if(chances[index].type == "Quantum Asteroid Field"){
            returningHex = new QuantumAsteroidField();
        } else
        if(chances[index].type == "Neutron Star"){
            returningHex = new NeutronStar();
        }
        return returningHex;
    }
}
