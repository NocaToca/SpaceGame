using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public Empire playerEmpire;

    static Board board;
    public static CanvasController canvasController;

    bool release;

    public static Hex displayingHex;

    int prevPlanetIndex;

    HexObject prevHexObject;

    static bool choosingHex = false;
    static Ship movingShip = null;

    static bool InteractionsEnabled = true;


    // Start is called before the first frame update
    void Start()
    {
        GetBoard();
        GetCanvasController();
        prevPlanetIndex = 0;
    }

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
        if(board != null){
            if(choosingHex){
                if(Input.GetMouseButton(0)){
                    Hex hexToMoveTo = GetTileUnderMouse();
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
                    choosingHex = false;
                    movingShip = null;
                    
                }
            }
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

        if(clickedHex.Interact()){
            clickedHex.GiveOptions();
        }
        canvasController.DisplayHex(clickedHex);
        

    }

    public static void RequestHexRecall(){
        if(displayingHex != null){
            canvasController.DisplayHex(displayingHex);
        } else {
            Debug.LogError("How are you requesting to display a hex that you haven't interacted with?");
        }
        
    }

    public static void Colonize(SystemHex hex, int index){
        Empire empire = Board.empires[0];
        
        Board.ColonizePlanet(empire, hex, index);
        canvasController.DisplayHex(hex);
    }

    public static void RequestMovement(Ship ship){
        movingShip = ship;
        choosingHex = true;
    }

    public static void DisableInteractions(){
        InteractionsEnabled = false;
    }
    public static void EnableInteractions(){
        InteractionsEnabled = true;
    }
}
