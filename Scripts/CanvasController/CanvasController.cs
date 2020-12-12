using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public static bool HexInfoDisplayed = false;

    public GameObject HexInfo;

    public int currentPlanetDisplayed;

    public int currentUnitDisplayed;

    public static bool wasInteractedWith = false;
    
    void Start(){
        currentPlanetDisplayed = 0;
        currentUnitDisplayed = 0;
    }

    public static bool checkWasInteracted(){
        if(wasInteractedWith){
            wasInteractedWith = false;
            return true;
        } else {
            return false;
        }
    }
    
    public void EndTurn(){
        GameMode.EndTurn(Board.GetPlayerEmpire());
        UpdateResourceDisplay();
    }
    
    public void DisplayHexInfo(){
        HexInfo.SetActive(true);
        HexInfoDisplayed = true;
    }

    public void GetRidOfHexInfo(){
        HexInfo.SetActive(false);
        HexInfoDisplayed = false;
    }

    public void IncrementPlanetIndex(){
        currentPlanetDisplayed++;
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    public void DecrementPlanetIndex(){
        currentPlanetDisplayed--;
        if(currentPlanetDisplayed < 0){
            currentPlanetDisplayed = MainController.displayingHex.GetPlanetsLength() - 1;
        }
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    public void IncrementShipIndex(){
        currentUnitDisplayed++;
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    public void DecrementShipIndex(){
        currentUnitDisplayed--;
        if(currentUnitDisplayed < 0){
            currentUnitDisplayed = MainController.displayingHex.ShipsOnHex.Count - 1;
        }
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    public void DisplayHex(Hex clickedHex){
        UpdateHexInfo(clickedHex);
        UpdateEmpireInfo(clickedHex);
        UpdateUnitInfo(clickedHex);
        DisableOrEnableButtons(clickedHex);
        ShowHexCoords(clickedHex);
        MainController.DisableInteractions();
    }

    private void ShowHexCoords(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(3).gameObject;

        Text text = gameObject2.GetComponent<Text>();
        HexCoordinates coords = Board.GetHexPosition(hex);
        text.text = coords.ToString();
    }

    private void UpdateUnitInfo(Hex hex){
        ChangeShipDisplayText(hex);
        ChangeShipDisplayLetter(hex);
        UpdateShipList(hex);
        UpdateMoveButton(hex);
    }

    private void UpdateEmpireInfo(Hex hex){
        ChangeEmpireDisplayColor(hex);
        ChangeEmpireDisplayText(hex);
    }

    private void UpdateHexInfo(Hex clickedHex){
        if(clickedHex is SystemHex){
            ChangePlanetDisplayedColor(PlanetVisuals.GetPlanetColorFromPlanet(Board.GetPlanet(clickedHex, currentPlanetDisplayed)));
        } else {
            ChangePlanetDisplayedColor(new Color(0,0,0,0));
        }

        ShowPlanetInfoText(clickedHex);
        UpdatePlanetScrollText(clickedHex);
        CheckIfColonizable(clickedHex);
        UpdateResourceDisplay(clickedHex);

    }

    private void UpdateResourceDisplay(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(5).gameObject;
        GameObject gameObject4 = gameObject2.transform.GetChild(6).gameObject;

        if(hex is SystemHex){
            Resources planetResources = Board.GetPlanet(hex, currentPlanetDisplayed).GetNaturalResources();
            
            gameObject3.SetActive(true);
            gameObject4.SetActive(true);

            Text text = gameObject3.GetComponent<Text>();
            text.text = "Gold: " + planetResources.GoldToString();
            text = gameObject4.GetComponent<Text>();
            text.text = "Prod: " + planetResources.ProdToString();
        } else {
            gameObject3.SetActive(false);
            gameObject4.SetActive(false);
        }

    }

    private void CheckIfColonizable(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(4).gameObject;

        Button button = gameObject3.GetComponent<Button>();
        if(hex is SystemHex){
            gameObject3.SetActive(true);
            SystemHex sys = (SystemHex)hex;
            if(!sys.planets[currentPlanetDisplayed%sys.planets.Length].Colonized && Board.CheckForColonyShip(hex)){
                button.interactable = true;
                return;
            }
            button.interactable = false;
        } else {
            gameObject3.SetActive(false);
        }
        
        
    }

    private void UpdatePlanetScrollText(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(3).gameObject;

        Text text = gameObject3.GetComponent<Text>();
        if(hex.GetType() == "System"){
            text.text = "" + (currentPlanetDisplayed%hex.GetPlanetsLength()+1) + "/" + hex.GetPlanetsLength();
        } else {
            text.text = "";
        }
        

        
    }

    private void ChangePlanetDisplayedColor(Color color){

        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;


        Image image = gameObject2.GetComponent<Image>();
        image.color = color;

    }

    private void ShowPlanetInfoText(Hex hex){

        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        
        Text text = gameObject2.GetComponentInChildren<Text>();

        text.text = hex.GetPlanetString(currentPlanetDisplayed);

    }

    private void ChangeEmpireDisplayColor(Hex hex){

        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(1).gameObject;

        Image image = gameObject2.GetComponent<Image>();
        if(Board.IsHexControlled(hex)){
            image.color = EmpireVisuals.GetEmpireColor(Board.GetEmpireThatControlsHex(hex));
        } else {
            image.color = new Color(0,0,0,0);
        }

    }

    private void ChangeEmpireDisplayText(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(1).gameObject;
        
        Text text = gameObject2.GetComponentInChildren<Text>();

        if(Board.IsHexControlled(hex)){
            text.text = EmpireVisuals.GetEmpireText(Board.GetEmpireThatControlsHex(hex));
        } else {
            text.text = Empire.GetDefaultText();
        }
    }

    private void ChangeShipDisplayText(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(0).gameObject;



        Text text = gameObject3.GetComponent<Text>();

        text.text = HexVisuals.GetShipText(hex, currentUnitDisplayed);
    }

    private void ChangeShipDisplayLetter(Hex hex){
        
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;

        Text text = gameObject2.GetComponent<Text>();

        text.text = HexVisuals.GetShipLetter(hex, currentUnitDisplayed);

    }

    private void UpdateShipList(Hex hex){
        
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(1).gameObject;

        Text text = gameObject3.GetComponent<Text>();
        if(Board.ShipsOnHex(hex).Length != 0){
            text.text = "" + (currentUnitDisplayed%Board.ShipsOnHex(hex).Length+1) + "/" + Board.ShipsOnHex(hex).Length;
        } else {
            text.text = "";
        }
    }

    private void DisableOrEnableButtons(Hex hex){
        bool b = hex.GetType() == "System" && hex.GetPlanetsLength() > 1;
        DisableOrEnablePlanetRightButton(hex, b);
        DisableOrEnablePlanetLeftButton(hex, b);

        b = Board.ShipsOnHex(hex).Length != 0;
        DisableOrEnableUnitRightButton(hex, b);
        DisableOrEnableUnitLeftButton(hex, b);
    }

    private void DisableOrEnablePlanetRightButton(Hex hex, bool b){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(1).gameObject;

        gameObject3.SetActive(b);
    }

    private void DisableOrEnablePlanetLeftButton(Hex hex, bool b){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(2).gameObject;

        gameObject3.SetActive(b);
    }

    private void DisableOrEnableUnitRightButton(Hex hex, bool b){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(2).gameObject;

        gameObject3.SetActive(b);
    }

    private void DisableOrEnableUnitLeftButton(Hex hex, bool b){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(3).gameObject;

        gameObject3.SetActive(b);
    }

    private void UpdateMoveButton(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(4).gameObject;

        Ship[] ships = Board.ShipsOnHex(hex);

        gameObject3.SetActive(ships.Length >= 1);

        Button button = gameObject3.GetComponent<Button>();

        if(ships.Length >= 1){
            
            if(ships[currentUnitDisplayed%ships.Length].availableMovementPoints == 0){
                button.interactable = false;
            } else {
                button.interactable = true;
            }
            
        }

    }

    private void UpdateResourceDisplay(){
        UpdateGold();
        UpdateProd();
    }

    private void UpdateGold(){
        GameObject gameObject1 = this.transform.GetChild(2).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;

        Text text = gameObject2.GetComponent<Text>();

        Resources empireRes = GameMode.GetPlayerResources();

        text.text = "Gold: " + empireRes.GoldToString();
    }

    private void UpdateProd(){
        GameObject gameObject1 = this.transform.GetChild(2).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(1).gameObject;

        Text text = gameObject2.GetComponent<Text>();

        Resources empireRes = GameMode.GetPlayerResources();

        text.text = "Prod: " + empireRes.ProdToString();
    }

    /*********************************************************************Interacting With Ships**********************************************************************************/
    public void MoveShip(){
        Ship[] ships = Board.ShipsOnHex(MainController.displayingHex);
        Ship ship = ships[currentUnitDisplayed % ships.Length];
        MainController.RequestMovement(ship);
        MainController.EnableInteractions();
    }

    public void ColonizePlanet(){
        SystemHex sys = (SystemHex)MainController.displayingHex;
        if(sys != null){
            MainController.Colonize(sys, currentPlanetDisplayed);
        } else {
            Debug.LogError("Error! You have asked to colonize a planet on a tile that isnt a system! Is the button working properly?");
        }
    }

}
