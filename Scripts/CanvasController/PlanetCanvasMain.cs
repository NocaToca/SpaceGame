using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetCanvasMain : MonoBehaviour
{
    [Header("Images")]
    public Image FoodImage;
    public Image ScienceImage;
    public Image ProductionImage;
    public Image GoldImage;
    public Image ContentImage;
    public Image TextContentImage;

    [Header("Resource Text")]
    public Text FoodText;
    public Text ScienceText;
    public Text ProductionText;
    public Text GoldText;
    public Text ColonizeText;
    public Text TemplateText;

    [Header("Buttons")]
    public Button ColonizeButton;
    public Button TemplateButton;

    private List<Button> buttons = new List<Button>();
    private List<Text> texts = new List<Text>();

    private SystemStorage storage;

    public void ShowPlanetInfo(Planet planet){
        if(planet == null){
            HideMenus();
            return;
        }
        DestroyListButton(buttons);
        DestroyListText(texts);
        ShowResources(planet);
        UpdateButtons(planet);
        ShowUnits();
        if(!planet.Colonized){
            return;
        }
        ShowBuildings(planet);
        ShowPops(planet);
    }

    private void HideMenus(){
        this.gameObject.SetActive(false);
    }

    private void DestroyListButton(List<Button> buttons){

        List<GameObject> gameObjects = new List<GameObject>();
        
        for(int i = 0; i < buttons.Count; i++){
            Button button = buttons[i];
            gameObjects.Add(button.gameObject);
        }
        foreach(GameObject gameObject in gameObjects){
            Destroy(gameObject, 0.0f);
        }
        this.buttons.Clear();
        
    }
    private void DestroyListText(List<Text> texts){

        List<GameObject> gameObjects = new List<GameObject>();
        
        for(int i = 0; i < texts.Count; i++){
            Text text = texts[i];
            gameObjects.Add(text.gameObject);
        }
        foreach(GameObject gameObject in gameObjects){
            Destroy(gameObject, 0.0f);
        }

        this.texts.Clear();
        
    }

    private void ShowBuildings(Planet planet){
        Empire empire;
        if(SystemStorage.workingScence){
            empire = Board.GetPlayerEmpire();
        } else {
            empire = Board.GetEmpireOwningPlanet(planet);
        }

        List<Building> buildings = planet.availableBuildings;
        if(buildings == null){
            planet.RefreshAvailableBuildings();
            buildings = planet.availableBuildings;
        }

        List<Unit> availableUnits = GameMode.GetUnitsAvailableToEmpire(Board.GetPlayerEmpire());

        if(!planet.HasStarport()){
            availableUnits = Unit.ClearShipsFromList(availableUnits);
        }

        int Length = buildings.Count;
        Length += availableUnits.Count;

        buttons.Clear();
        for(int i = 0; i < Length; i++){
            buttons.Add(Instantiate(TemplateButton));
            buttons[i].transform.SetParent(ContentImage.gameObject.transform);
            buttons[i].gameObject.SetActive(true);
            GameObject gameObject4 = buttons[i].gameObject;
            Text text = gameObject4.GetComponentInChildren<Text>();
            if(i < buildings.Count){
                text.text = buildings[i].name;
                buttons[i].onClick.AddListener(buildings[i].Build);
            } else {
                text.text = availableUnits[i - buildings.Count].name;
                buttons[i].onClick.AddListener(availableUnits[i - buildings.Count].Build);
            }
        }

        Image image = ContentImage;
        image.rectTransform.sizeDelta = new Vector2(80, Length * 20);
        Vector2 pos = image.rectTransform.anchoredPosition;
        pos.y += Length * 10;
        image.rectTransform.anchoredPosition = pos;
    }

    private void ShowPops(Planet planet){
        List<Pops> pops = planet.popsOnPlanet;

        int Length = pops.Count;
        for(int i = 0; i < pops.Count; i++){
            texts.Add(Instantiate(TemplateText));
            texts[i].transform.SetParent(TextContentImage.gameObject.transform);
            texts[i].gameObject.SetActive(true);
            Text newText = texts[i].gameObject.GetComponentInChildren<Text>();
            newText.text = pops[i].name;
        }

        Image image = TextContentImage;
        image.rectTransform.sizeDelta = new Vector2(80, Length * 20);
        Vector2 pos = image.rectTransform.anchoredPosition;
        pos.y += Length * 10;
        image.rectTransform.anchoredPosition = pos;
    }

    private void ShowResources(Planet planet){
        Resource planetResource = planet.GetResourceProduction();
        SetText(FoodText, planetResource.GoldToString());
        SetText(ScienceText, planetResource.ScienceToString());
        SetText(ProductionText, planetResource.ProdToString());
        SetText(GoldText, planetResource.GoldToString());
    }

    private void SetText(Text text, string newText){
        text.text = newText;
    }

    private void UpdateButtons(Planet planet){
        ColonizeButton.interactable = PCL.DeterminePlanetColonizable(planet);
    }

    private void ShowUnits(){
        if(MainController.displayingHex == null){
            return;
        }

        

    }

    public void SetRelativeStorageUnit(SystemStorage ss){
        storage = ss;
    }

}
