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
    public static int currentPlanetDisplayed;

    //The current unit index we are displaying (can be greater than the length of units in a system but not lower than 0)
    public static int currentUnitDisplayed;

    public static int currentFleetDisplayed;

    //Whether or not the canvas was interacted with
    public static bool wasInteractedWith = false;

    //Our base button prefab for our button scrollable lists
    public Button buttonPrefab;

    //Wether or not our build menu is displaying
    public static bool BuildMenuDisplaying = false;

    //The list of text displaying units, along with the text we base that text off of
    static List<Text> textList = new List<Text>();
    static List<Button> buttonList = new List<Button>();
    public Text unitTextBase;
    public Button unitButtonBase;

    private bool updateIndex = false;

    //Whether or not we're hiding the text
    public bool hideText = false;

    //Whether or not we're displaying the list of ships
    public bool shipListDisplayed = false;

    public Image ShipDisplay;

    public static bool buttonPress = false;
    
    public Button MoveButton;
    public Button CreateFleetButton;
    public Button FightButton;
    public Button ScrollRightInFleets;
    public Button ScrollLeftInFleets;
    public Text ShipNameText;
    
/********************************************************************Button Call Functions************************************************************/
    //These functions are called by our buttons on our UI

    //Ending the player's turn is done by the gamemode
    public void EndTurn(){
        GameMode.EndTurn(Board.GetPlayerEmpire());
        UpdateResourceDisplay();
        buttonPress = true;
    }

    //Request to redisplay the resources on our resource panel. Used so that when we go back to the board scene it isn't all 0
    public void RequestRedisplayOfResources(){
        buttonPress = true;
        UpdateResourceDisplay();
    }

    //Increments the planet index by one
    public void IncrementPlanetIndex(){
        buttonPress = true;
        currentPlanetDisplayed++;
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    //Decrements the planet index by one; resets to the top of the planet list when less than 0
    public void DecrementPlanetIndex(){
        currentPlanetDisplayed--;
        buttonPress = true;
        if(currentPlanetDisplayed < 0){
            SystemHex sys = (SystemHex)MainController.displayingHex;
            currentPlanetDisplayed = sys.GetPlanetsLength() - 1;
        }
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    //Increments the ship index
    public void IncrementShipIndex(){
        buttonPress = true;
        currentUnitDisplayed++;
        wasInteractedWith = true;
        MainController.RequestHexRecall();
    }

    //Decrements the ship index; resets to the top if the value is less than 0
    public void DecrementShipIndex(){
        buttonPress = true;
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
        
        List<Building> availableBuildings = Board.GetPlanet(MainController.displayingHex, currentPlanetDisplayed).availableBuildings;
        List<Unit> availableUnits = GameMode.GetUnitsAvailableToEmpire(Board.GetPlayerEmpire());

        //If the planet has no available buildings, it might not have been set! We need to refresh it just in case
        if(availableBuildings == null){
            Board.GetPlanet(MainController.displayingHex, currentPlanetDisplayed).RefreshAvailableBuildings();
            availableBuildings = Board.GetPlanet(MainController.displayingHex, currentPlanetDisplayed).availableBuildings;
        }

        //Ships can only be built if the planet has a starport, so we need to make sure it does!
        if(!Board.GetPlanet(MainController.displayingHex, currentPlanetDisplayed).HasStarport()){
            availableUnits = Unit.ClearShipsFromList(availableUnits);
        }

        int Length = availableBuildings.Count;
        Length += availableUnits.Count;
        for(int i = 0; i < Length; i++){
            buttons.Add(Instantiate(buttonPrefab));
            buttons[i].transform.SetParent(gameObject3.transform);
            buttons[i].gameObject.SetActive(true);
            GameObject gameObject4 = buttons[i].gameObject;
            Text text = gameObject4.GetComponentInChildren<Text>();
            if(i < availableBuildings.Count){
                text.text = availableBuildings[i].name;
                buttons[i].onClick.AddListener(availableBuildings[i].Build);
            } else {
                text.text = availableUnits[i - availableBuildings.Count].name;
                buttons[i].onClick.AddListener(availableUnits[i - availableBuildings.Count].Build);
            }
        }
        Image image = gameObject3.GetComponent<Image>();
        image.rectTransform.sizeDelta = new Vector2(80, Length * 20);
        Vector2 pos = image.rectTransform.anchoredPosition;
        pos.y += Length * 10;
        image.rectTransform.anchoredPosition = pos;
    }

    //Much like we generate the building buttons, we want to generate the buttons for our technology
    public void GenerateTechButtons(){
        GameObject gameObject1 = this.transform.GetChild(3).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(0).gameObject;

        List<Button> buttons = new List<Button>();

        List<Tech> techs = GameMode.GetTechsAvailableToEmpire(Board.GetPlayerEmpire());

        int Length = techs.Count;

        for(int i = 0; i < Length; i++){
            buttons.Add(Instantiate(buttonPrefab));
            buttons[i].transform.SetParent(gameObject3.transform);
            buttons[i].gameObject.SetActive(true);
            GameObject gameObject4 = buttons[i].gameObject;
            Text text = gameObject4.GetComponentInChildren<Text>();
            if(i < techs.Count){
                text.text = techs[i].name;
                buttons[i].onClick.AddListener(techs[i].AddToQueue);
            }
        }
        
        Image image = gameObject3.GetComponent<Image>();
        image.rectTransform.sizeDelta = new Vector2(80, Length * 20);
    }

    //Destroys all of the buttons in our building button list (this is also for the tech buttons)
    public void DestroyBuildingButtons(){
        GameObject gameObject1 = this.transform.GetChild(3).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(0).gameObject;

        Transform[] children = gameObject3.GetComponentsInChildren<Transform>();

        for(int i = 1; i < children.Length; i++){
            GameObject.Destroy(children[i].gameObject, 0.0f);
        }
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
        DisableOrEnableFightButton(hex);
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

    //Displays the natural Resource generated by the planet
    private void UpdateResourceDisplay(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(5).gameObject;
        GameObject gameObject4 = gameObject2.transform.GetChild(6).gameObject;

        if(hex is SystemHex){
            Resource planetResource = Board.GetPlanet(hex, currentPlanetDisplayed).GetResourceProduction();
            
            gameObject3.SetActive(true);
            gameObject4.SetActive(true);

            Text text = gameObject3.GetComponent<Text>();
            text.text = "Gold: " + planetResource.GoldToString();
            text = gameObject4.GetComponent<Text>();
            text.text = "Prod: " + planetResource.ProdToString();
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

    //Updating the Resource that the empire has
    private void UpdateResourceDisplay(){
        UpdateGold();
        UpdateProd();
        UpdateScience();
    }

    //Updates the gold display on our resource pannel
    private void UpdateGold(){
        GameObject gameObject1 = this.transform.GetChild(2).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;

        Text text = gameObject2.GetComponent<Text>();

        Resource empireRes = GameMode.GetPlayerResource();

        text.text = "Gold: " + empireRes.GoldToString();
    }

    //Updates the production display on our resource pannal
    private void UpdateProd(){
        GameObject gameObject1 = this.transform.GetChild(2).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(1).gameObject;

        Text text = gameObject2.GetComponent<Text>();

        Resource empireRes = GameMode.GetPlayerResource();

        text.text = "Prod: " + empireRes.ProdToString();
    }

    //Updates the science display on our resource pannel
    private void UpdateScience(){
        
        GameObject gameObject1 = this.transform.GetChild(2).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;

        Text text = gameObject2.GetComponent<Text>();

        Resource empireRes = GameMode.GetPlayerResource();
        
        text.text = "Science: " + empireRes.ScienceToString();
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

    //Shows the unit texts on our canvas
    public void ShowUnitTextOnCanvas(){
        //List<Ship> allShips = Board.GetAllShips();
        foreach(Text text in textList){
            //Due to us being able to switch scenes, we have to make sure that our text is still there
            if(text != null){
                GameObject.Destroy(text.gameObject);
            }
        }
        textList.Clear();
        if(!hideText){
            List<Hex> hexes = Board.GetHexesOnScreenWithShips();
            foreach(Hex hex in hexes){
                AddUnitTextAboveHex(hex);
            }
        }
    }

    public void ShowUnitButtonsOnCanvas(){
         foreach(Button button in buttonList){
            //Due to us being able to switch scenes, we have to make sure that our text is still there
            if(button != null){
                GameObject.Destroy(button.gameObject);
            }
        }
        buttonList.Clear();
        if(!hideText){
            List<Hex> hexes = Board.GetHexesOnScreenWithShips();
            foreach(Hex hex in hexes){
                AddUnitButtonAboveHex(hex);
            }
        }
    }

    private void AddUnitButtonAboveHex(Hex hex){
        Ship[] ships = Board.ShipsOnHex(hex);
        if(ships.Length >= 1){
            GameObject parentObject = this.transform.GetChild(4).gameObject;

            Button button = GameObject.Instantiate(unitButtonBase);
            button.transform.SetParent(parentObject.transform);

            Vector3 screenPos = Camera.main.WorldToScreenPoint(hex.referenceObject.transform.position);

            button.image.rectTransform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);

            if(Board.DoesHexHaveOpposingFleets(hex)){
                button.image.color = Color.white;
            } else
            if(Board.GetShipEmpire(ships[0]) != null){
                button.image.color = EmpireVisuals.GetEmpireColor(Board.GetShipEmpire(ships[0]));
            } else {
                button.image.color = new Color(.5f, .5f, .5f, 1f);
            }

            Text text = button.gameObject.GetComponentInChildren<Text>();
            text.text = "" + ships.Length;

            buttonList.Add(button);
        }
    }

    //Adds the unit text above the specific hex, if it even has a ship on it
    private void AddUnitTextAboveHex(Hex hex){
        Ship[] ships = Board.ShipsOnHex(hex);
        if(ships.Length >= 1){
            GameObject parentObject = this.transform.GetChild(4).gameObject;
        
            Text newText = GameObject.Instantiate(unitTextBase);
            newText.transform.SetParent(parentObject.transform);

            Vector3 screenPos = Camera.main.WorldToScreenPoint(hex.referenceObject.transform.position);

            newText.rectTransform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);

            textList.Add(newText);
            newText.text = (ships.Length > 1) ? ships.Length + " Ships": ships[0].name;
        }
    }


/************************************************************************Displaying Menus/Buttons************************************************************/
    //Displays the whole info tab
    public void DisplayHexInfo(){
        HexInfo.SetActive(true);
        HexInfoDisplayed = true;
        hideText = true;
    }

    //Gets rid of the whole info tab
    public void GetRidOfHexInfo(){
        HexInfo.SetActive(false);
        HexInfoDisplayed = false;
        hideText = false;
        DisableBuildMenu();
    }

    //Enables the build menu (list of items you can build on a planet)
    public void EnableBuildMenu(){
        GameObject gameObject1 = this.transform.GetChild(3).gameObject;
        gameObject1.SetActive(true);
        BuildMenuDisplaying = true;
        GenerateBuildingButtons();
    }

    public void EnableTechMenu(){
        GameObject gameObject1 = this.transform.GetChild(3).gameObject;
        gameObject1.SetActive(true);
        BuildMenuDisplaying = true;
        GenerateTechButtons();
    }

    //Disables the build menu
    public void DisableBuildMenu(){
        GameObject gameObject1 = this.transform.GetChild(3).gameObject;
        gameObject1.SetActive(false);
        BuildMenuDisplaying = false;
        DestroyBuildingButtons();
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

    //For the fight button, we just need to see if there is: 1. at least a ship and 2. the ships have different empires
    private void DisableOrEnableFightButton(Hex hex){
        GameObject gameObject1 = this.transform.GetChild(0).gameObject;
        GameObject gameObject2 = gameObject1.transform.GetChild(2).gameObject;
        GameObject gameObject3 = gameObject2.transform.GetChild(5).gameObject;

        Button button = gameObject3.GetComponent<Button>();

        Ship[] ships = Board.ShipsOnHex(hex);
        if(ships.Length == 0){
            button.interactable = false;
            return;
        }
        Empire empire = Board.GetShipEmpire(ships[0]);
        foreach(Ship ship in ships){
            if(Board.GetShipEmpire(ship) != empire){
                
                button.interactable = true;
                return;
            }
        }
        button.interactable = false;
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
            
            if(ships[currentUnitDisplayed%ships.Length].availableMovementPoints == 0 || Board.GetShipEmpire(ships[currentUnitDisplayed%ships.Length]) != Board.GetPlayerEmpire()){
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
        currentFleetDisplayed = 0;
    }
    public static bool checkWasInteracted(){
        if(wasInteractedWith){
            wasInteractedWith = false;
            return true;
        } else {
            return false;
        }
    }
    public static void Clear(){
        MainController.ClearCanvasController();

    }

    /*********************************************************************Interacting With Ships**********************************************************************************/
    //Almost all of these functions are self explanatory 
    public void MoveShip(){
        buttonPress = true;
        Ship[] ships = Board.ShipsOnHex(MainController.displayingHex);
        Ship ship = ships[currentUnitDisplayed % ships.Length];
        Fleet shipFleet = Board.GetShipFleet(ship);
        if(shipFleet == null){
            shipFleet = new Fleet(ship);
        }
        MainController.RequestMovement(shipFleet);
        MainController.EnableInteractions();
    }

    public void ColonizePlanet(){
        buttonPress = true;
        SystemHex sys = (SystemHex)MainController.displayingHex;
        if(sys != null){
            MainController.Colonize(sys, currentPlanetDisplayed);
        } else {
            Debug.LogError("Error! You have asked to colonize a planet on a tile that isnt a system! Is the button working properly?");
        }
    }

    public void BuildOnPlanet(){
        buttonPress = true;
        if(!BuildMenuDisplaying){
            EnableBuildMenu();
        } else {
            DisableBuildMenu();
        }
    }

    public void ShowAvailableTechs(){
        buttonPress = true;
        if(!BuildMenuDisplaying){
            EnableTechMenu();
        } else {
            DisableBuildMenu();
        }
    }

    public void Fight(){
        buttonPress = true;
        Board.Fight();
    }

    public void IncrementFleetIndex(){
        buttonPress = true;
        currentFleetDisplayed++;
        updateIndex = true;
        DisplayFleetInfo();
    }
    public void DecrementFleetIndex(){
        buttonPress = true;
        if(currentFleetDisplayed == 0){
            Hex hex = MainController.displayingHex;

            Ship[] ships = Board.ShipsOnHex(hex);
            //bool fleets = false;
            List<Fleet> fleets = new List<Fleet>();
            foreach(Ship ship in ships){
                Fleet fleet = Board.GetShipFleet(ship);
                if(fleet != null){
                    fleets.Add(new Fleet(ship));
                } else
                if(!fleets.Contains(fleet)){
                    fleets.Add(fleet);
                }
            }

            currentFleetDisplayed += fleets.Count;
        } else {
            currentFleetDisplayed--;
        }
        updateIndex = true;
        DisplayFleetInfo();
    }

    public void DisplayFleetInfo(){
        Hex hex;

        if(!updateIndex){
            hex = MainController.GetTileUnderMouse();
            MainController.displayingHex = hex;
            currentFleetDisplayed = 0;
        } else {
            updateIndex = false;
        }

        hex = MainController.displayingHex;
        buttonPress = true;

        Ship[] ships = Board.ShipsOnHex(hex);
        //bool fleets = false;
        List<Fleet> fleets = new List<Fleet>();
        foreach(Ship ship in ships){
            Fleet fleet = Board.GetShipFleet(ship);
            if(fleet == null){
                fleets.Add(new Fleet(ship));
            } else
            if(!fleets.Contains(fleet)){
                fleets.Add(fleet);
            }
        }
        currentFleetDisplayed %= fleets.Count;

        if(Board.DoesHexHaveOpposingFleets(hex)){
            FightButton.interactable = true;
        } else {
            FightButton.interactable = false;
        }
        if(fleets[currentFleetDisplayed].GetMovePoints() >= 1 && Board.GetPlayerEmpire() == Board.GetShipEmpire(fleets[currentFleetDisplayed].shipsInFleet[0])){
            MoveButton.interactable = true;
        } else {
            MoveButton.interactable = false;
        }
        if(fleets.Count > 1){
            CreateFleetButton.interactable = true;
            ScrollLeftInFleets.gameObject.SetActive(true);
            ScrollRightInFleets.gameObject.SetActive(true);
        } else {
            CreateFleetButton.interactable = false;
            ScrollRightInFleets.gameObject.SetActive(false);
            ScrollLeftInFleets.gameObject.SetActive(false);
        }

        ShipNameText.gameObject.SetActive(true);
        ShipNameText.text = fleets[currentFleetDisplayed%fleets.Count].shipsInFleet[0].name;
    }

}
