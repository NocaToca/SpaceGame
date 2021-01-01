using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationSettings : MonoBehaviour
{
    //This class is basically the settings for our planet resource generation
    [Header("Arctic Planet Settings")]
    public float GoldCenterArctic = 5.0f;
    public float GoldRadArctic = 1.0f;

    public float FoodCenterArctic = 2.0f;
    public float FoodRadArctic = 1.5f;

    public float ProdCenterArctic = 3.0f;
    public float ProdRadArctic = 0.5f;
    
    public float ScienceCenterArctic = 4.0f;
    public float ScienceRadArctic = 1.0f;

    [Header("Continental Planet Settings")]
    public float GoldCenterContinental = 1.0f;
    public float GoldRadContinental = 0.5f;

    public float FoodCenterContinental = 6.0f;
    public float FoodRadContinental = 2.0f;

    public float ProdCenterContinental = 2.5f;
    public float ProdRadContinental = 1.25f;
    
    public float ScienceCenterContinental = 4.0f;
    public float ScienceRadContinental = 0.5f;

    [Header("Molten Planet Settings")]
    public float GoldCenterMolten = 2.5f;
    public float GoldRadMolten = 1.0f;

    public float FoodCenterMolten = 0.5f;
    public float FoodRadMolten = 0.4f;

    public float ProdCenterMolten = 8.0f;
    public float ProdRadMolten = 2.0f;
    
    public float ScienceCenterMolten = 2.0f;
    public float ScienceRadMolten = 0.75f;

    [Header("Ocean Planet Settings")]
    public float GoldCenterOcean = 1.5f;
    public float GoldRadOcean = 1.0f;

    public float FoodCenterOcean = 2.5f;
    public float FoodRadOcean = 0.5f;

    public float ProdCenterOcean = 3.0f;
    public float ProdRadOcean = 1.0f;

    public float ScienceCenterOcean = 6.0f;
    public float ScienceRadOcean = 1.5f;
    // Start is called before the first frame update

    /*
    This is basically a bunch of code to say "Set the base generation variables of each planet".
    It's organized so I can search through it easier, but it's basically all the same
    */

    void Awake()
    {
        SetGold();
        SetProd();
        SetFood();
        SetScience();
    }

    private void SetGold(){
        SetGoldArctic();
        SetGoldContinental();
        SetGoldMolten();
        SetGoldOcean();
    }
    private void SetGoldArctic(){
        ArcticPlanet.GoldCenter = GoldCenterArctic;
        ArcticPlanet.GoldRad = GoldRadArctic;
    }
    private void SetGoldContinental(){
        ContinetalPlanet.GoldCenter = GoldCenterContinental;
        ContinetalPlanet.GoldRad = GoldRadContinental;
    }
    private void SetGoldMolten(){
        MoltenPlanet.GoldCenter = GoldCenterMolten; 
        MoltenPlanet.GoldRad = GoldRadMolten; 
    }
    private void SetGoldOcean(){
        OceanPlanet.GoldCenter = GoldCenterOcean;
        OceanPlanet.GoldRad = GoldRadOcean;
    }

    private void SetProd(){
        SetProdArctic();
        SetProdContinental();
        SetProdMolten();
        SetProdOcean();
    }
    private void SetProdArctic(){
        ArcticPlanet.ProdCenter = ProdCenterArctic;
        ArcticPlanet.ProdRad = ProdRadArctic;
    }
    private void SetProdContinental(){
        ContinetalPlanet.ProdCenter = ProdCenterContinental;
        ContinetalPlanet.ProdRad = ProdRadContinental; 
    }
    private void SetProdMolten(){
        MoltenPlanet.ProdCenter = ProdCenterMolten; 
        MoltenPlanet.ProdRad = ProdRadMolten; 
    }
    private void SetProdOcean(){
        OceanPlanet.ProdCenter = ProdCenterOcean;
        OceanPlanet.ProdRad = ProdRadOcean;
    }

    private void SetFood(){
        SetFoodArctic();
        SetFoodContinental();
        SetFoodMolten();
        SetFoodOcean();
    }
    private void SetFoodArctic(){
        ArcticPlanet.FoodCenter = FoodCenterArctic;
        ArcticPlanet.FoodRad = FoodRadArctic;
    }
    private void SetFoodContinental(){
        ContinetalPlanet.FoodCenter = FoodCenterContinental; 
        ContinetalPlanet.FoodRad = FoodRadContinental; 
    }
    private void SetFoodMolten(){
        MoltenPlanet.FoodCenter = FoodCenterMolten;
        MoltenPlanet.FoodRad = FoodRadMolten;
    }
    private void SetFoodOcean(){
        OceanPlanet.FoodCenter = FoodCenterOcean;
        OceanPlanet.FoodRad = FoodRadOcean;
    }

    private void SetScience(){
        SetScienceArctic();
        SetScienceContinental();
        SetScienceMolten();
        SetScienceOcean();
    }
    private void SetScienceArctic(){
        ArcticPlanet.ScienceCenter = ScienceCenterArctic;
        ArcticPlanet.ScienceRad = ScienceRadArctic;
    }
    private void SetScienceContinental(){
        ContinetalPlanet.ScienceCenter = ScienceCenterContinental;
        ContinetalPlanet.ScienceRad = ScienceRadContinental;
    }
    private void SetScienceMolten(){
        MoltenPlanet.ScienceCenter = ScienceCenterMolten;
        MoltenPlanet.ScienceRad = ScienceRadMolten;
    }
    private void SetScienceOcean(){
        OceanPlanet.ScienceCenter = ScienceCenterOcean;
        OceanPlanet.ScienceRad = ScienceRadOcean;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
