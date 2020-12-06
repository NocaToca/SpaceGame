using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public Empire playerEmpire;

    Board board;

    bool release;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] gettingBoards = GameObject.FindGameObjectsWithTag("Board");
        if(gettingBoards.Length == 0){
            Debug.LogError("Error! Please set up a board in the scene or we can't access the hexes!");
            return;
        }
        board = gettingBoards[0].GetComponent<Board>();
    }

    

    // Update is called once per frame
    void Update()
    {
        if(board != null){
            if (Input.GetMouseButton(0)) {

                //Debug.Log("bee");


                if(release){
                    release = false;
                    HandleInput();
                    
                }

            } else {
                release = true;
            }
        }
    }

    void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			TouchCell(hit);
		}
	}

    public void TouchCell (RaycastHit hit) {
        Vector3 pos = hit.collider.gameObject.transform.position;
        Hex clickedHex = board.GetHexNearestToPos(pos);
        if(clickedHex == null){
            Debug.LogError("Error retrieving hex; retrieved a null hex!");
            return;
        }
        if(clickedHex.Interact()){
            clickedHex.GiveOptions();
        }

    }
}
