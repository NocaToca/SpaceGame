using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            GameObject system = GameObject.FindGameObjectsWithTag("SystemManager")[0];

            SceneManager.LoadScene("Hexes");
        }
    }
}
