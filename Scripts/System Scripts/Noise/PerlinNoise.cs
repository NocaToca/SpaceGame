using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise
{
    //Generates perlin noise. This is a common function, so I don't feel like I need to explain it much
    public static float[,] Generate(int width, int height, int seed, NoiseSettings settings, AnimationCurve heightCurve){
        float[,] heightMap = new float[height, width];

        AnimationCurve noiseCurve = new AnimationCurve(heightCurve.keys);

        if(seed == 0){
            seed = Random.Range(-10000, 10000);
        }

        int octaves = settings.octaves;
        float persistance = settings.persistance;
        float lacunarity = settings.lacunarity;
        float scale = settings.scale;

        System.Random rand = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;

        float amplitude = 1;
        float frequency = 1;

        for(int i = 0; i < octaves; i++){
            float offsetX = rand.Next(-10000, 10000);
            float offsetY = rand.Next(-10000, 10000);

            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        float center = (width/2.0f + height/2.0f)/2.0f;
        
        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
                amplitude = 1;
                frequency = 1;

                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++){
                    float noiseX = (x-center + octaveOffsets[i].x)/scale * frequency;
                    float noiseY = (y-center + octaveOffsets[i].y)/scale * frequency;

                    float noiseValue = Mathf.PerlinNoise(noiseX, noiseY) * 2.0f - 1.0f;
                    noiseHeight += noiseValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                heightMap[y,x] = noiseHeight;
            }

        }

        float[,] noiseMap = new float[height,width];

        //now we need to go through each noise map value and then edit so its in the range of our height map
        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){

                float normalizedHeight = (heightMap[y,x] + 1)/(2.0f * maxPossibleHeight/1.75f);
                noiseMap[y,x] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                
                noiseMap[y,x] = noiseCurve.Evaluate(noiseMap[y,x]);

            }
        }


        return noiseMap;

        //return FinializeHeightMap(heightMap, falloffMap); //creating and returning the actual height map

    }
    //A simple function that just puts a value through a function for the height map
    private static float Evaluate(float value){
        float a = 3.0f;
        float b = 2.2f;

        return Mathf.Pow(value, a)/(Mathf.Pow(value, a) + Mathf.Pow(b - (b * value), a));
    }

    //The generation of the map once we have the two maps is simply making sure the value is still between 0 and 1 and subtracting them
    private static float[,] FinializeHeightMap(float[,] heightMap, float[,] falloffMap){

        int height = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);
        
        float[,] finalHeightMap = new float[height, width];

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
                finalHeightMap[y,x] = Mathf.Clamp01(heightMap[x,y] - falloffMap[x,y]);
            }
        }

        return finalHeightMap;

    }
}

    

public class NoiseSettings{
    public int octaves;
    public float persistance;
    public float lacunarity;
    public float scale;
    
    public NoiseSettings(int octaves, float persistance, float lacunarity, float scale){
        this.octaves = octaves;
        this.persistance = persistance;
        this.lacunarity = lacunarity;
        this.scale = scale;
    }
}