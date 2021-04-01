using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    //The main controller class is responsible for interacting with the board

    //The player empire
    public Empire playerEmpire;

    //THe board and ui
    static Board board;
    public static CanvasController canvasController;

    //This bool is to control the functions started by mouse input to make sure that it doesnt activate again until the clicked is released
    bool release;

    //The current hex we are interacting with
    public static Hex displayingHex;

    int prevPlanetIndex;

    HexObject prevHexObject;

    //Varialbes for ship movement
    static bool choosingHex = false;
    static Fleet movingFleet = null;

    //Wether or not we can interact with the board
    static bool InteractionsEnabled = true;

    bool waitingForCompletion = false;

    static Vector3 previousWorldSpace = new Vector3(0,0,0);

    public static Vector3 GetWorldSpace(){
        return previousWorldSpace;
    }
    // Start is called before the first frame update
    void Start()
    {
        GetBoard();
        GetCanvasController();
        prevPlanetIndex = 0;
        canvasController.RequestRedisplayOfResources();
        Camera.main.transform.position = previousWorldSpace;
    }

    //Getting the board in the scene
    void GetBoard(){
        
        GameObject[] gettingBoards = GameObject.FindGameObjectsWithTag("Board");
        if(gettingBoards.Length == 0){
            Debug.LogError("Error! Please set up a board in the scene or we can't access the hexes!");
            return;
        } else
        if(gettingBoards.Length > 1){
            Debug.LogWarning("Warning: you seem to have multiple boards in the scene. Only one board will be used.");

        }
        board = gettingBoards[0].GetComponent<Board>();
    }

    //Getting the canvas controller in the scene
    void GetCanvasController(){
        GameObject[] gettingCanvasControllers = GameObject.FindGameObjectsWithTag("Canvas");
        if(gettingCanvasControllers.Length == 0){
            Debug.LogError("Error! Please set up a canvas controller in the scene or we can't access the UI!");
            return;
        } else
        if(gettingCanvasControllers.Length > 1){
            Debug.LogWarning("Warning: you seem to have multiple canvas controllers in the scene. Only one canvas controller will be used.");

        }
        canvasController = gettingCanvasControllers[0].GetComponent<CanvasController>();
    }

    
    private bool ScreenMoved(){
        Vector3 worldPos = Camera.main.gameObject.transform.position;
        if(Mathf.Abs(worldPos.x - previousWorldSpace.x) >= 0.1f || Mathf.Abs(worldPos.z - previousWorldSpace.z) >= 0.1f ){
            previousWorldSpace = worldPos;
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if(ScreenMoved()){
            canvasController.ShowUnitButtonsOnCanvas();
        }
        
        //Here, we want to make sure the board isnt null before doing what we wish
        if(board != null){
            //If we want to move a ship, our next click will determine where our ship moves
            if(choosingHex){
                if(Input.GetMouseButton(0)){
                    Hex hexToMoveTo = GetTileUnderMouse();
                    //If we click a hex, move to it, otherwise assume the user wants to cancel 
                    if(hexToMoveTo != null){
                        Board.RequestMovement(hexToMoveTo, movingFleet);
                        if(CanvasController.HexInfoDisplayed){
                            canvasController.GetRidOfHexInfo();
                            displayingHex = null;
                        }
                    } else {
                        if(CanvasController.HexInfoDisplayed){
                            canvasController.GetRidOfHexInfo();
                            displayingHex = null;
                        }
                    }
                    //Once we choose a hex, we don't want to choose another
                    choosingHex = false;
                    movingFleet = null;
                    
                }
            }
            if(!waitingForCompletion){
                StartCoroutine(WiatForInput());
                waitingForCompletion = true;
            }
            
        }
    }

    IEnumerator WiatForInput(){
        bool leftClick = Input.GetMouseButton(0) && InteractionsEnabled;
        bool spaceClick = Input.GetKeyDown(KeyCode.Space);
        yield return new WaitForSeconds(.1f);
        if(CanvasController.buttonPress){
            CanvasController.buttonPress = false;
            //return;
        } else
        //If our interactions are enabled and we click, we want to interact with what we clicked
        if (leftClick) {

            if(prevPlanetIndex != CanvasController.currentPlanetDisplayed){
                prevPlanetIndex = CanvasController.currentPlanetDisplayed;
                //return;
            }

            if(release){
                release = false;
                HandleInput();
                    
            }

            
        } else
        if(spaceClick){

            if(CanvasController.HexInfoDisplayed){
                canvasController.GetRidOfHexInfo();
                displayingHex = null;
                EnableInteractions();
            }
                
        } else {
            release = true;
        }
        waitingForCompletion = false;
        
        //CanvasController.buttonPress = false;
    }

    //Gets the tile under the mouse
    public static Hex GetTileUnderMouse(){
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
            Vector3 pos = hit.collider.gameObject.transform.position;
        
            Hex clickedHex = board.GetHexNearestToPos(pos);
            return clickedHex;
        }
        return null;
    }

    //Handles clicks; checks to see if what we clicked is a hex
    void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			TouchCell(hit);
		} else {
            if(CanvasController.HexInfoDisplayed && CanvasController.checkWasInteracted()){
                //canvasController.GetRidOfHexInfo();
                //displayingHex = null;

            }
        }
	}

    //If we touched a cell, this is the function that is called. As of right now, we just want to display the hex we clicked
    public void TouchCell (RaycastHit hit) {
        CanvasController.currentPlanetDisplayed = 0;
        Vector3 pos = hit.collider.gameObject.transform.position;
        
        
        Hex clickedHex = board.GetHexNearestToPos(pos);

        if(prevHexObject != null){
            prevHexObject = null;
        } else {
            prevHexObject = clickedHex.referenceObject;
        }

        displayingHex = clickedHex;

        if(clickedHex == null){
            Debug.LogError("Error retrieving hex; retrieved a null hex!");
            return;
        }

        if(CanvasController.HexInfoDisplayed){
            
        } 

        //Since we now have a better way to display a planet, we go over to that scene 
        SystemStorage.LoadScene = true;
        displayingHex = clickedHex;
        SceneManager.LoadScene("System");

    }

    //Request our canvas to update its values so it doesnt display old information
    public static void RequestHexRecall(){
        if(displayingHex != null){
            canvasController.DisplayHex(displayingHex);
        } else {
            Debug.LogError("How are you requesting to re-display a hex that you haven't interacted with?");
        }
        
    }   
    //A bridge function to colonize a planet, called by the canvas controller
    public static void Colonize(SystemHex hex, int index){
        Empire empire = Board.empires[0];
        
        Board.ColonizePlanet(empire, hex, index);
        //canvasController.DisplayHex(hex);
    }

    //Called when we request to move a ship
    public static void RequestMovement(Fleet fleet){
        movingFleet = fleet;
        choosingHex = true;
    }

    //Disables and enables interactions with the board
    public static void DisableInteractions(){
        InteractionsEnabled = false;
    }
    public static void EnableInteractions(){
        InteractionsEnabled = true;
    }

    //Clears the canvas controller
    public static void ClearCanvasController(){
        canvasController.GetRidOfHexInfo();
        canvasController.DisableBuildMenu();
        EnableInteractions();
    }
}
