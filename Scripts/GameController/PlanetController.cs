using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetController : MonoBehaviour
{
    bool moving = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(moving){
            moving = !Move();
            if(!moving){
                SystemStorage.gm.SwapOrder();
                SystemStorage.ShowPlanetInfo();
            }
            return;
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            GameObject system = GameObject.FindGameObjectsWithTag("SystemManager")[0];

            SceneManager.LoadScene("Hexes");
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            SystemStorage.gm.movingAngle = 540 * Mathf.Deg2Rad;
            SystemStorage.gm.lerp = 0.0f;
            moving = true;
        }
    }

    bool Move(){
        return SystemStorage.gm.MovePlanets();
    }

    
}
