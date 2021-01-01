using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The Texture Renderer class is used to generate noise textures then apply them to an object.
//Currently this class is obsolete
public class TextureRenderer : MonoBehaviour
{
    //NoiseSettings
    public int octaves = 1;
    public float persistance = .5f;
    public float lacunarity = 1.0f;
    public float scale = 1.0f;

    //Texture dimensions
    public int width = 512;
    public int height = 512;

    public float bumpiness = 1.0f;

    public AnimationCurve heightCurve;

    public float cloudLevel = .75f;
    public Color atmosphereColor = Color.cyan;
    public Color cloudDarkColor = Color.grey;
    public Color cloudColor = Color.white;

    public int seed = 0;

    //The colors of the terrian
    public TerrainType[] terrainRegions;

    public Renderer textureRender;

    [HideInInspector]
    public GameObject owner;
    void Awake(){
        owner = transform.gameObject;
    }

    //This method here simply creates our color map based off of our regions array
    private Color[] CreateColorMap(float[,] noiseMap){

        int height = noiseMap.GetLength(0);
        int width = noiseMap.GetLength(1);

        //Obviously creating a new color map
        Color[] colourMap = new Color[width * height];

        //Next, we set up a for loop to go through our noise map array
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				
				float currentHeight = noiseMap [x, y]; //Set the height value to the value we found before

                //Then we loop through the regions to find out which color that height should be
				for (int i = 0; i < terrainRegions.Length; i++) {
					if (currentHeight >= terrainRegions[i].height) {
						
                        if(i+1 < terrainRegions.Length){
                            float percent = (currentHeight - terrainRegions[i].height)/(terrainRegions[i+1].height - terrainRegions[i].height);
                            colourMap [y * height + x] = Color.Lerp(terrainRegions[i].colour, terrainRegions[i+1].colour, percent);
                        } else {
                            colourMap [y * height + x] = terrainRegions[i].colour;
                        }
                        //Debug.Log(i);
					} else {
						break;
					}
				}
			}
		}

        return colourMap;

    }

    //From our height map, we generate a noise texture
    public Texture2D TextureFromHeightMap(float[,] heightMap) {
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, heightMap [x, y]);
			}
		}

		return TextureFromColourMap (colourMap, width, height);
	}

    //Generates a texture from our colour map
    public Texture2D TextureFromColourMap(Color[] colourMap, int width, int height){
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    //Applies the texture and normal map to the material
    private void ApplyTextureToMaterial(Texture2D texture, Texture2D normalMap, Material mat, MeshRenderer mr){
        
		mr.sharedMaterial.mainTexture = texture;
		//mr.transform.localScale = new Vector3 (texture.width, 1, texture.height);
        //MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        mat.SetTexture("Main", texture);
        mat.SetTexture("NormalMap", normalMap);

    }
    // Start is called before the first frame update
    void Start(){
        
        //int width = (int)(gameObject.transform.position.x - gameObject.transform.localScale.x/2);
        //int height = (int)(gameObject.transform.position.y - gameObject.transform.localScale.y/2);

        //float[,] noiseMap = PerlinNoise.Generate(width, height, seed, new NoiseSettings(octaves, persistance, lacunarity, scale));
        //WorkWithMesh(this.gameObject);

        // foreach (float noise in noiseMap){
            
        //     Debug.Log(noise);
            
        // }

        //ApplyTextureToMaterial(TextureFromColourMap(CreateColorMap(PerlinNoise.Generate(width, height, seed, new NoiseSettings(octaves, persistance, lacunarity, scale))), width, height));
        //ApplyTextureToMaterial(TextureFromHeightMap(noiseMap));

    }

    //Generates a noise map for the mesh and then generates a normal map and texture based off of it for each material
    public void WorkWithMesh(GameObject go){
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        for(int i = 0; i < mr.materials.Length; i++){
            float[,] noiseMap = PerlinNoise.Generate(width, height, seed, new NoiseSettings(octaves, persistance, lacunarity, scale), heightCurve);
            Texture2D texture = TextureFromColourMap(CreateColorMap(noiseMap), width, height);
            Texture2D texture2 = HeightMapToNormalMap(noiseMap);
            ApplyTextureToMaterial(texture, texture2, mr.materials[i], mr);
            //mr.normals = GetNormals(noiseMap);
        }
    }

    //Using gradients of the change from black to white, we get the normal map for the height map
    private Texture2D HeightMapToNormalMap(float[,] noiseMap){
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
			}
		}
        Vector3[] normals;
        Vector3[,] gradients = new Vector3[height, width];
        Color[] normalMap = new Color[colourMap.Length];
        for(int y = 0, i = 0; y < height; y++){
            for(int x = 0; x < width; x++, i++){
                float changeInX = 0.0f;
                float changeInY = 0.0f;

                int values = 0;
                Vector3 v = new Vector3(0,0,1);
                float noiseHeight = noiseMap[y,x];
                float val;
                float exaggeration;
                if(x - 1 >= 0){
                    val = noiseMap[y, x - 1];
                    exaggeration = GetExaggeration(val, noiseHeight);
                    v.x +=  val * bumpiness * exaggeration;
                    values++;
                }
                if(x + 1 < width){
                    val = noiseMap[y, x + 1];
                    exaggeration = GetExaggeration(val, noiseHeight);
                    v.x += val * bumpiness * exaggeration;
                    values++;
                }
                v.x /= values;

                values = 0;
                if(y - 1 >= 0){
                    val = noiseMap[y - 1, x];
                    exaggeration = GetExaggeration(val, noiseHeight);
                    v.y += val * bumpiness * exaggeration;
                    values++;
                }
                if(y + 1 < height){
                    val = noiseMap[y + 1, x];
                    exaggeration = GetExaggeration(val, noiseHeight);
                    v.y += val * bumpiness * exaggeration;
                    values++;
                }
                v.y /= values;
            
                float mag = Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
                v.x /= mag;
                v.y /= mag;
                v.z /= mag;
                //v = Vector3.Normalize(v);
                gradients[y,x] = v;

                Vector4 color = new Vector4(v.x, v.y, v.z, 1.0f);

                normalMap[i] = color;
                //normals[i] = v;
            }
        }
        //Debug.Log(normalMap[1]);
        //Debug.Log(gradients[255,255]);
        //Debug.Log(gradients[255,256]);
        //Debug.Log(gradients[255,257]);
        //Debug.Log(gradients[256,255].y);
        //Debug.Log(gradients[256,256]);
        //Debug.Log(gradients[256,257]);
        //Debug.Log(gradients[257,255]);
        //Debug.Log(gradients[257,256]);
        //Debug.Log(gradients[257,257]);

        return TextureFromColourMap(normalMap, width, height);
    }

    //Exaggerates the rates of change to try and make things pop out more
    private float GetExaggeration(float val1, float val2){

        int indexVal1 = 0;
        for(int i = 0; i < terrainRegions.Length; i++){
            if(val1 >= terrainRegions[i].height){
                indexVal1 = i;
                break;
            }
        }
        int indexVal2 = 0;
        for(int i = 0; i < terrainRegions.Length; i++){
            if(val2 >= terrainRegions[i].height){
                indexVal2 = i;
                if(indexVal1 == indexVal2){
                    return 1.0f;
                }
                break;
            }
        }
        return (terrainRegions[indexVal1].height > terrainRegions[indexVal2].height) ? (val2 - terrainRegions[indexVal2].height)/(val1 - terrainRegions[indexVal1].height) : (val1 - terrainRegions[indexVal1].height)/(val2 - terrainRegions[indexVal2].height);


    }

    //Works with the atmosphere of a planet to get a texture for that
    public void WorkWithAtmosphere(GameObject go){
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        float[,] noiseMap = PerlinNoise.Generate(width, height, seed, new NoiseSettings(octaves, persistance, lacunarity, scale), heightCurve);
        Texture2D texture = CreateCloudTexture(noiseMap);
        ApplyTextureToMaterial(texture, mr);
    }

    private void ApplyTextureToMaterial(Texture2D texture, MeshRenderer mr){
        mr.sharedMaterial.mainTexture = texture;
        mr.castShadows = false;
        Material mat = mr.materials[0];
        mat.SetTexture("Clouds", texture);
    }

    //With more noise, creates a cloud texture
    private Texture2D CreateCloudTexture(float[,] noiseMap){
        int width = noiseMap.GetLength (1);
		int height = noiseMap.GetLength (0);

		Color[] colourMap = new Color[width * height];
		for (int y = 0, i = 0; y < height; y++) {
			for (int x = 0; x < width; x++, i++) {
				if(noiseMap[y, x] > cloudLevel){
                    float percent = (1-cloudLevel)/(1-noiseMap[y,x]);
                    colourMap[i] = Color.Lerp(cloudDarkColor, cloudColor, noiseMap[y,x]);
                } else {
                    colourMap[i] = atmosphereColor;
                }
			}
		}
        return TextureFromColourMap(colourMap, width, height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color colour;
    }
}
