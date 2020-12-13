using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    static Ship movingShip = null;

    //Wether or not we can interact with the board
    static bool InteractionsEnabled = true;


    // Start is called before the first frame update
    void Start()
    {
        GetBoard();
        GetCanvasController();
        prevPlanetIndex = 0;
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

    

    // Update is called once per frame
    void Update()
    {
        //Here, we want to make sure the board isnt null before doing what we wish
        if(board != null){
            //If we want to move a ship, our next click will determine where our ship moves
            if(choosingHex){
                if(Input.GetMouseButton(0)){
                    Hex hexToMoveTo = GetTileUnderMouse();
                    //If we click a hex, move to it, otherwise assume the user wants to cancel 
                    if(hexToMoveTo != null){
                        Board.RequestMovement(hexToMoveTo, movingShip);
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
                    movingShip = null;
                    
                }
            }

            //If our interactions are enabled and we click, we want to interact with what we clicked
            if (Input.GetMouseButton(0) && InteractionsEnabled) {

                //Debug.Log("bee");
                if(prevPlanetIndex != canvasController.currentPlanetDisplayed){
                    prevPlanetIndex = canvasController.currentPlanetDisplayed;
                    return;
                }

                if(release){
                    release = false;
                    HandleInput();
                    
                }

            
            } else
            if(Input.GetKeyDown(KeyCode.Space)){

                if(CanvasController.HexInfoDisplayed){
                    canvasController.GetRidOfHexInfo();
                    displayingHex = null;
                    EnableInteractions();
                }
                
            } else {
                release = true;
            }
        }
    }

    //Gets the tile under the mouse
    Hex GetTileUnderMouse(){
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
        canvasController.currentPlanetDisplayed = 0;
        Vector3 pos = hit.collider.gameObject.transform.position;
        
        
        Hex clickedHex = board.GetHexNearestToPos(pos);

        if(prevHexObject != null){
            //Debug.Log(HexBasedAStar.AStar(prevHexObject, clickedHex.referenceObject, board.GetHeight(), board.GetWidth()));
            prevHexObject = null;
        } else {
            prevHexObject = clickedHex.referenceObject;
            //Debug.Log("bee!");
        }

        displayingHex = clickedHex;

        if(clickedHex == null){
            Debug.LogError("Error retrieving hex; retrieved a null hex!");
            return;
        }

        if(CanvasController.HexInfoDisplayed){
            
        } 
        canvasController.DisplayHexInfo();
        canvasController.DisplayHex(clickedHex);
        

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
        canvasController.DisplayHex(hex);
    }

    //Called when we request to move a ship
    public static void RequestMovement(Ship ship){
        movingShip = ship;
        choosingHex = true;
    }

    //Disables and enables interactions with the board
    public static void DisableInteractions(){
        InteractionsEnabled = false;
    }
    public static void EnableInteractions(){
        InteractionsEnabled = true;
    }
}
