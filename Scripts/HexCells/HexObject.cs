using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexObject : MonoBehaviour
{
    public GameObject displayHex;

    public HexCoordinates coordinates;

    public HexMesh hexMesh;

    public bool hasBeenTouched = false;

    HexMesh display;

    public Hex hex;

    void Awake(){
        hexMesh = GetComponentInChildren<HexMesh>();
        if(hexMesh == null){
            Debug.Log("Please attach a hex mesh to the hex prefab");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
