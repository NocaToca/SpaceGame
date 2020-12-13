using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    /*
    This whole class is responsible for interacting with our UI
    If something screws up in the UI, it is most definitely from this class
    As new things are getting added organization might change
    */

    //Whether or not our hex info is displayed
    public static bool HexInfoDisplayed = false;

    //Our Hexinfo object
    public GameObject HexInfo;

    //The current planet index we are displaying (can be greater than the length of planets in a system but not lower than 0)
    public int currentPlanetDisplayed;

    //The current unit index we are displaying (can be greater than the length of units in a system but not lower than 0)
    public int currentUnitDisplayed;

    //Whether or not the canvas was interacted with
    public static bool wasInteractedWith = false;

    //Our base button prefab for our button scrollable lists
    public Button buttonPrefab;
    
/********************************************************************Button Call Functions************************************************************/
    //These functions are called by our buttons on our UI

    //Ending the player's turn is done by the gamemode
    public void EndTurn(){
        GameMode.EndTurn(Board.GetPlayerEmpire());
        UpdateResourceDisplay();
    }

    //Increments the planet index by one
    public void IncrementPlanetIndex(){
        currentPlanetDisplayed++;
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    //Decrements the planet index by one; resets to the top of the planet list when less than 0
    public void DecrementPlanetIndex(){
        currentPlanetDisplayed--;
        if(currentPlanetDisplayed < 0){
            SystemHex sys = (SystemHex)MainController.displayingHex;
            currentPlanetDisplayed = sys.GetPlanetsLength() - 1;
        }
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    //Increments the ship index
    public void IncrementShipIndex(){
        currentUnitDisplayed++;
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    //Decrements the ship index; resets to the top if the value is less than 0
    public void DecrementShipIndex(){
        currentUnitDisplayed--;
        if(currentUnitDisplayed < 0){
            currentUnitDisplayed = Board.ShipsOnHex(MainController.displayingHex, Board.GetPlayerEmpire()).Length - 1;
        }
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    //Generates the scrollable list of buttons for building functionality. Sets them to be children of the content image
    public void GenerateBuildingButtons(){
        GameObject gameObject1 = this.transform.GetChild(3).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(0).gameObject;

        List<Button> buttons = new List<Button>();
        int Length = 10;
        for(int i = 0; i < Length; i++){
            buttons.Add(Instantiate(buttonPrefab));
            buttons[i].transform.SetParent(gameObject3.transform);
            buttons[i].gameObject.SetActive(true);
        }
        Image image = gameObject3.GetComponent<Image>();
        image.rectTransform.sizeDelta = new Vector2(80, Length * 20);
    }
    
/***************************************************************Initial Display Functions****************************************************/
    //The list of functions called when we first call for display, without all of the logical code

    public void DisplayHex(Hex clickedHex){
        UpdateHexInfo(clickedHex);
        UpdateEmpireInfo(clickedHex);
        UpdateUnitInfo(clickedHex);
        DisableOrEnableButtons(clickedHex);
        ShowHexCoords(clickedHex);
        MainController.DisableInteractions(); //We need to disable the main controller so we can interact with the UI
    }

    //Updates the display of the hex
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

    //Updates the information of the empire on the hex
    private void UpdateEmpireInfo(Hex hex){
        ChangeEmpireDisplayColor(hex);
        ChangeEmpireDisplayText(hex);
    }

    //Responsible for updating all of the cooresponding unit infor
    private void UpdateUnitInfo(Hex hex){
        ChangeShipDisplayText(hex);
        ChangeShipDisplayLetter(hex);
        UpdateShipList(hex);
        UpdateMoveButton(hex);
    }

    //Responsible for disabling/enabling most of the buttons on the interface
    private void DisableOrEnableButtons(Hex hex){
        bool b = false;
        if((hex is SystemHex)){
            SystemHex sys = (SystemHex)hex;
            b = sys.GetPlanetsLength() > 1;
        }
        DisableOrEnablePlanetRightButton(hex, b);
        DisableOrEnablePlanetLeftButton(hex, b);

        b = Board.ShipsOnHex(hex).Length != 0;
        DisableOrEnableUnitRightButton(hex, b);
        DisableOrEnableUnitLeftButton(hex, b);
    }

    //Shows the hex coordinates (x,y,z) of the hex
    private void ShowHexCoords(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(3).gameObject;

        Text text = gameObject2.GetComponent<Text>();
        HexCoordinates coords = Board.GetHexPosition(hex);
        text.text = coords.ToString();
    }

/**********************************************************************************Planet Display*********************************************************************/
    //Responsible for the planet tab

    //Sets the color of the planet image to the cooresponding color
    private void ChangePlanetDisplayedColor(Color color){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;

        Image image = gameObject2.GetComponent<Image>();
        image.color = color;
    }

    //Showing text information of the planet
    private void ShowPlanetInfoText(Hex hex){

        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        
        Text text = gameObject2.GetComponentInChildren<Text>();

        if(hex is SystemHex){
            SystemHex sysHex = (SystemHex)hex;
            text.text = sysHex.GetPlanetString(currentPlanetDisplayed);
        } else {
            text.text = "";
        }
    }

    //Displays the current planet we are displaying out of the available planets
    private void UpdatePlanetScrollText(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(3).gameObject;

        Text text = gameObject3.GetComponent<Text>();
        if(hex is SystemHex){
            SystemHex sysHex = (SystemHex)hex;
            text.text = "" + (currentPlanetDisplayed%sysHex.GetPlanetsLength()+1) + "/" + sysHex.GetPlanetsLength();
        } else {
            text.text = "";
        }
    }

    //Checks to see whether the planet is colonized; if it is we change the colonize button to a "build" button
    private void CheckIfColonizable(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(4).gameObject;

        Button button = gameObject3.GetComponent<Button>();
        if(hex is SystemHex){
            gameObject3.SetActive(true);
            SystemHex sys = (SystemHex)hex;
            ConvertButtonToColonizeButton(button);
            if(!sys.planets[currentPlanetDisplayed%sys.planets.Length].Colonized && Board.CheckForColonyShip(hex)){
                button.interactable = true;
                return;
            } else
            if(sys.planets[currentPlanetDisplayed%sys.planets.Length].Colonized){
                button.interactable = true;
                ConvertButtonToBuildButton(button);
                return;
            }
            button.interactable = false;
        } else {
            gameObject3.SetActive(false);
        }
    }
    private void ConvertButtonToColonizeButton(Button button){
        GameObject gameObject1 = button.gameObject.transform.GetChild(0).gameObject;
        Text text = gameObject1.GetComponent<Text>();
        
        text.text = "Colonize";

        //Remove all button functionality then add the one function we want
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ColonizePlanet);
    }
    private void ConvertButtonToBuildButton(Button button){
        GameObject gameObject1 = button.gameObject.transform.GetChild(0).gameObject;
        Text text = gameObject1.GetComponent<Text>();
        
        text.text = "Build";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(BuildOnPlanet);
    }

    //Displays the natural resources generated by the planet
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

/******************************************************************************Empire Display***************************************************************/

    //Changes the empire display color to the empire's color
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

    //Displays information on whether or not the tile is colonized
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

    //Updating the resources that the empire has
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

/***************************************************************************Unit Display*****************************************************************************/

    //Displays text information regarding the ship displayed on the tile
    private void ChangeShipDisplayText(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(0).gameObject;



        Text text = gameObject3.GetComponent<Text>();

        text.text = HexVisuals.GetShipText(hex, currentUnitDisplayed);
    }

    //Displays the letter cooresponding to the ship (Planning to make it an image)
    private void ChangeShipDisplayLetter(Hex hex){
        
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;

        Text text = gameObject2.GetComponent<Text>();

        text.text = HexVisuals.GetShipLetter(hex, currentUnitDisplayed);

    }

    //Updates the ship being displayed out of the available ships on the tile
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

/************************************************************************Displaying Menus/Buttons************************************************************/
    //Displays the whole info tab
    public void DisplayHexInfo(){
        HexInfo.SetActive(true);
        HexInfoDisplayed = true;
    }

    //Gets rid of the whole info tab
    public void GetRidOfHexInfo(){
        HexInfo.SetActive(false);
        HexInfoDisplayed = false;
        DisableBuildMenu();
    }

    //Enables the build menu (list of items you can build on a planet)
    private void EnableBuildMenu(){
        GameObject gameObject1 = this.transform.GetChild(3).gameObject;
        gameObject1.SetActive(true);
    }

    //Disables the build menu
    private void DisableBuildMenu(){
        GameObject gameObject1 = this.transform.GetChild(3).gameObject;
        gameObject1.SetActive(false);
    }

    //As the enabling or disabling a button off of a boolean
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

    //Updates the move button to see if a ship can or cannot move, or if there even is a ship on the tile
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
/**************************************************************************************Misc*************************************************************************************/
    void Start(){
        currentPlanetDisplayed = 0;
        currentUnitDisplayed = 0;
        GenerateBuildingButtons();
    }

    

    public static bool checkWasInteracted(){
        if(wasInteractedWith){
            wasInteractedWith = false;
            return true;
        } else {
            return false;
        }
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

    public void BuildOnPlanet(){

    }

}
