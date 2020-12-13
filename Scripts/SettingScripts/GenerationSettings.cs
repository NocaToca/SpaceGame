using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationSettings : MonoBehaviour
{
    //This class is basically the settings for our planet resource generation

    public float GoldCenterArctic = 5.0f;
    public float GoldRadArctic = 1.0f;

    public float FoodCenterArctic = 2.0f;
    public float FoodRadArctic = 1.5f;

    public float ProdCenterArctic = 3.0f;
    public float ProdRadArctic = 0.5f;

    public float GoldCenterContinental = 1.0f;
    public float GoldRadContinental = 0.5f;

    public float FoodCenterContinental = 6.0f;
    public float FoodRadContinental = 2.0f;

    public float ProdCenterContinental = 2.5f;
    public float ProdRadContinental = 1.25f;

    public float GoldCenterMolten = 2.5f;
    public float GoldRadMolten = 1.0f;

    public float FoodCenterMolten = 0.5f;
    public float FoodRadMolten = 0.4f;

    public float ProdCenterMolten = 8.0f;
    public float ProdRadMolten = 2.0f;
    // Start is called before the first frame update
    void Awake()
    {
        ArcticPlanet.GoldCenter = GoldCenterArctic;
        ArcticPlanet.GoldRad = GoldRadArctic;

        ArcticPlanet.ProdCenter = ProdCenterArctic;
        ArcticPlanet.ProdRad = ProdRadArctic;

        ArcticPlanet.FoodCenter = FoodCenterArctic;
        ArcticPlanet.FoodRad = FoodRadArctic;

        ContinetalPlanet.GoldCenter = GoldCenterContinental;
        ContinetalPlanet.GoldRad = GoldRadContinental;

        ContinetalPlanet.ProdCenter = ProdCenterContinental;
        ContinetalPlanet.ProdRad = ProdRadContinental; 

        ContinetalPlanet.FoodCenter = FoodCenterContinental; 
        ContinetalPlanet.FoodRad = FoodRadContinental; 

        MoltenPlanet.GoldCenter = GoldCenterMolten; 
        MoltenPlanet.GoldRad = GoldRadMolten; 

        MoltenPlanet.ProdCenter = ProdCenterMolten; 
        MoltenPlanet.ProdRad = ProdRadMolten; 

        MoltenPlanet.FoodCenter = FoodCenterMolten;
        MoltenPlanet.FoodRad = FoodRadMolten;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
