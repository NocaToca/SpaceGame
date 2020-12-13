using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Our visual class for our hex object is very simple, all it does is hold color information, coordinates, and the hex it's associated with
public class HexObject : MonoBehaviour
{
    //
    public GameObject displayHex;

    public HexCoordinates coordinates;

    public HexMesh hexMesh;

    public bool hasBeenTouched = false;

    HexMesh display;

    public Hex hex;

    //On awake we just need to get the mesh maker so it knows what it's making
    void Awake(){
        hexMesh = GetComponentInChildren<HexMesh>();
        if(hexMesh == null){
            Debug.Log("Please attach a hex mesh to the hex prefab");
        }
    }
}
